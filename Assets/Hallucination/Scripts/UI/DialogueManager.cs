using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using TMPro;


public class DialogueManager : MonoBehaviour {
    CanvasGroup canvasGroup;
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;  // Reference to the TextMeshPro component

    private Queue<DialogueLine> dialogueLines;  // Queue to hold the dialogue lines
    private DialogueData dialogueData;  // Store all the dialogues from JSON
    
    public GameObject leftImage;  // Reference to the left image GameObject (player's image)
    public GameObject rightImage;  // Reference to the right image GameObject (other character's image)
    public static DialogueManager Instance { get; private set; }
    bool isTyping = false;
    string currentSentence;

    Action callbackAfterDialogue;

    void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("DialogueManager: " +
            "Duplicate instance detected and removed. Only one instance of DialogueManager is allowed.");
            Destroy(Instance);
            return;
        }
        isTyping = false;
        canvasGroup = GetComponent<CanvasGroup>();
        Instance = this;
        dialogueLines = new Queue<DialogueLine>();
        LoadDialoguesFromFile();  // Load all dialogues from the JSON file
    }

    void Start() {
        gameObject.SetActive(false);
    }

    void OnEnable() {
        InputManager.Control.Dialogue.Next.performed += NextDialogue;
    }
    void OnDisable() {
        InputManager.Control.Dialogue.Next.performed -= NextDialogue;
    }

    void NextDialogue(InputAction.CallbackContext context) {
        DisplayNextSentence();
    }

    // Load the entire JSON file containing multiple dialogues
    async void LoadDialoguesFromFile() {
        string filePath = Path.Combine(Application.streamingAssetsPath, "dialogue.json");

        UnityWebRequest request = UnityWebRequest.Get(filePath);
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone) {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success) {
            Debug.Log(request.downloadHandler.text);
        } else {
            Debug.LogError("Cannot load file at " + filePath);
        }

        string dataAsJson = request.downloadHandler.text;
        dialogueData = JsonUtility.FromJson<DialogueData>(dataAsJson);  // Parse the JSON file
        if (dialogueData == null || dialogueData.dialogues == null) {
            Debug.LogError("Failed to load dialogues from JSON file.");
        }
    }

    // Start a specific dialogue by its name
    public void StartDialogue(string dialogueName, Action callback = null) {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        callbackAfterDialogue = callback;
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.DialogueActive;
        if (dialogueData != null) {
            dialogueBox.SetActive(true);  // Show the dialogue box when dialogue starts
            // Find the correct dialogue by name
            DialogueCollection dialogueCollection = FindDialogueByName(dialogueName);

            if (dialogueCollection != null) {
                dialogueLines.Clear();  // Clear any previously loaded lines

                // Enqueue each line of the specified dialogue
                foreach (DialogueLine dialogueLine in dialogueCollection.lines) {
                    dialogueLines.Enqueue(dialogueLine);
                }

                DisplayNextSentence();  // Display the first line of the dialogue
            } else {
                Debug.LogError("Dialogue not found: " + dialogueName);
            }
        }
    }

    // Helper method to find the dialogue by name
    private DialogueCollection FindDialogueByName(string dialogueName) {
        foreach (DialogueCollection collection in dialogueData.dialogues) {
            if (collection.name == dialogueName) {
                return collection;
            }
        }
        return null;  // Return null if not found
    }

    public void DisplayNextSentence() {
        // If dialogue is still typing, show the full sentence immediately
        if (isTyping) {
            StopAllCoroutines();
            dialogueText.text = currentSentence;
            TMP_TextInfo textInfo = dialogueText.textInfo;
            SetDialogueAlpha(255, textInfo);
            isTyping = false;
            return;
        }

        // If no more dialogue lines, end dialogue
        if (dialogueLines.Count == 0) {
            EndDialogue();
            return;
        }

        // Start typing the next dialogue
        DialogueLine dialogueLine = dialogueLines.Dequeue();
        StopAllCoroutines();
        UpdateSpeakerImage(dialogueLine.speaker);
        currentSentence = dialogueLine.sentence;
        StartCoroutine(TypeSentence(dialogueLine));
    }
    void UpdateSpeakerImage(string speaker) {
        if (speaker == "Fairy") {
            leftImage.SetActive(true);
            rightImage.SetActive(false);
        } else {
            leftImage.SetActive(false);
            rightImage.SetActive(true);
        }
    }
    IEnumerator TypeSentence(DialogueLine dialogueLine) {
        isTyping = true;
        dialogueText.text = dialogueLine.sentence;
        TMP_TextInfo textInfo = dialogueText.textInfo;
        
        SetDialogueAlpha(0, textInfo);
        // Gradually reveal characters
        for (int i = 0; i < textInfo.characterCount; i++) {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
            float alpha = 0;
            while (alpha < 1) {
                alpha += Time.deltaTime * 20f;
                alpha = Math.Min(alpha, 1);
                byte newAlpha = (byte)(alpha * 255);

                for (int j = 0; j < 4; j++) {
                    vertexColors[vertexIndex + j].a = newAlpha;
                }

                dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null;
            }
        }
        isTyping = false;
    }

    void SetDialogueAlpha(byte alpha, TMP_TextInfo textInfo) {
        dialogueText.ForceMeshUpdate();
        for (int i = 0; i < textInfo.characterCount; i++) {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

            for (int j = 0; j < 4; j++) {
                vertexColors[vertexIndex + j].a = (byte)alpha;
            }
        }
        dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    void EndDialogue() {
        // Debug.Log("End of dialogue");
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
        callbackAfterDialogue?.Invoke();
    }
}