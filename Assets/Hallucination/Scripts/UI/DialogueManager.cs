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
    [Serializable]
    public class DialogueBox {
        public GameObject dialogueBox;
        public GameObject bearImage, darkBearImage;
        public TextMeshProUGUI dialogueText;
    }

    [SerializeField] DialogueBox normalDialogueBox, inGameDialogueBox;
    DialogueBox currentDialogueBox;

    private Queue<DialogueLine> dialogueLines;
    private DialogueData dialogueData;

    public static DialogueManager Instance { get; private set; }
    bool isTyping = false;
    bool isLoaded = false;
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
        Instance = this;
        dialogueLines = new Queue<DialogueLine>();
        LoadDialoguesFromFile();  // Load all dialogues from the JSON file
        normalDialogueBox.dialogueBox.SetActive(false);
        inGameDialogueBox.dialogueBox.SetActive(false);
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
        switch (Util.CurrentLanguage()) {
            case Language.English:
                filePath = Path.Combine(Application.streamingAssetsPath, "Dialogue/English/dialogue.json");
                break;
            case Language.Chinese:
                filePath = Path.Combine(Application.streamingAssetsPath, "Dialogue/Chinese/dialogue.json");
                break;
        }

        UnityWebRequest request = UnityWebRequest.Get("file://" + filePath);
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone) {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success) {
            // Debug.Log(request.downloadHandler.text);
        } else {
            Debug.LogError("Cannot load file at " + filePath);
        }

        string dataAsJson = request.downloadHandler.text;
        dialogueData = JsonUtility.FromJson<DialogueData>(dataAsJson);  // Parse the JSON file
        if (dialogueData == null || dialogueData.dialogues == null) {
            Debug.LogError("Failed to load dialogues from JSON file.");
        }
        isLoaded = true;
    }

    /// <summary>
    /// Starts a specific dialogue by its name without a callback.
    /// This function is useful for triggering dialogues from inspector buttons.
    /// </summary>
    /// <param name="dialogueName">The name of the dialogue to start.</param>
    public void StartDialogue(string dialogueName) => StartDialogueWithCallback(dialogueName);

    public async void StartDialogueWithCallback(string dialogueName, Action callback = null) {
        while (!isLoaded) await Task.Yield();
        StopAllCoroutines();
        if (GameStateManager.Instance.CurrentGamePlayState == GamePlayState.SwitchingWorld) return;
        gameObject.SetActive(true);
        callbackAfterDialogue = callback;
        // Debug.Log("Start dialogue " + dialogueName);
        if (dialogueData != null) {
            DialogueCollection dialogueCollection = FindDialogueByName(dialogueName);
            if (dialogueCollection != null) {
                if (dialogueCollection.pauseGame) {
                    GameStateManager.Instance.CurrentGamePlayState = GamePlayState.DialogueActive;
                    WorldSwitchManager.Instance.Fairy.GetComponent<Character>()?.StopMotion();
                    WorldSwitchManager.Instance.Bear.GetComponent<Character>()?.StopMotion();
                    currentDialogueBox = normalDialogueBox;
                    normalDialogueBox.dialogueBox.SetActive(true);
                    inGameDialogueBox.dialogueBox.SetActive(false);
                } else {
                    currentDialogueBox = inGameDialogueBox;
                    normalDialogueBox.dialogueBox.gameObject.SetActive(false);
                    inGameDialogueBox.dialogueBox.gameObject.SetActive(true);
                }
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
        
        // If dialogue is still typing and the game is paused, show the full sentence immediately
        if (isTyping && GameStateManager.Instance.CurrentGamePlayState == GamePlayState.DialogueActive) {
            StopAllCoroutines();
            currentDialogueBox.dialogueText.text = currentSentence;
            TMP_TextInfo textInfo = currentDialogueBox.dialogueText.textInfo;
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
        StartCoroutine(TypeSentence(dialogueLine, 2));
    }
    void UpdateSpeakerImage(string speaker) {
        if (speaker == "Bear") {
            currentDialogueBox.bearImage.SetActive(true);
            currentDialogueBox.darkBearImage.SetActive(false);
        } else if (speaker == "Dark Bear") {
            currentDialogueBox.bearImage.SetActive(false);
            currentDialogueBox.darkBearImage.SetActive(true);
        } else {
            Debug.LogError("Speaker not recognized: " + speaker);
        }
    }
    IEnumerator TypeSentence(DialogueLine dialogueLine, float speed = 1f) {
        var dialogueText = currentDialogueBox.dialogueText;
        isTyping = true;
        dialogueText.text = dialogueLine.sentence;
        TMP_TextInfo textInfo = dialogueText.textInfo;
        // Debug.Log(currentDialogueBox.dialogueText.text);
        SetDialogueAlpha(0, textInfo);
        dialogueText.ForceMeshUpdate();
        // Gradually reveal characters
        for (int i = 0; i < textInfo.characterCount; i++) {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
            float alpha = 0;
            
            while (alpha < 1) {
                
                alpha += Time.deltaTime * 10f * speed;
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
        var dialogueText = currentDialogueBox.dialogueText;
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
        StopAllCoroutines();
        isTyping = false;
        gameObject.SetActive(false);
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
        callbackAfterDialogue?.Invoke();
    }
}