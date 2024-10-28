using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class DialogueManager : MonoBehaviour {
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;  // Reference to the TextMeshPro component

    private Queue<DialogueLine> dialogueLines;  // Queue to hold the dialogue lines
    private DialogueData dialogueData;  // Store all the dialogues from JSON

    public GameObject leftImage;  // Reference to the left image GameObject (player's image)
    public GameObject rightImage;  // Reference to the right image GameObject (other character's image)


    void Start() {
        dialogueLines = new Queue<DialogueLine>();
        LoadDialoguesFromFile();  // Load all dialogues from the JSON file
        dialogueBox.SetActive(false);  // Initially hide the dialogue box at the start
        leftImage.SetActive(false);  // Hide both images at the start
        rightImage.SetActive(false);
        StartDialogue("quest_start");

    }

    void Update() {
        // Check if the player presses Enter and a sentence is not typing
        if (Input.GetButtonDown("Interact")) {
            DisplayNextSentence();
        }
    }

    // Load the entire JSON file containing multiple dialogues
    void LoadDialoguesFromFile() {
        string filePath = Path.Combine(Application.streamingAssetsPath, "dialogue.json");

        if (File.Exists(filePath)) {
            string dataAsJson = File.ReadAllText(filePath);
            dialogueData = JsonUtility.FromJson<DialogueData>(dataAsJson);  // Parse the JSON file

            if (dialogueData == null || dialogueData.dialogues == null) {
                Debug.LogError("Failed to load dialogues from JSON file.");
            }
        } else {
            Debug.LogError("Dialogue file not found at: " + filePath);
        }
    }

    // Start a specific dialogue by its name
    public void StartDialogue(string dialogueName) {
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
        if (dialogueLines.Count == 0) {
            EndDialogue();
            return;
        }

        DialogueLine dialogueLine = dialogueLines.Dequeue();
        StopAllCoroutines();  // Stop any previous typing coroutine
        UpdateSpeakerImage(dialogueLine.speaker); // Update image visibility based on speaker
        StartCoroutine(TypeSentence(dialogueLine));  // Display the dialogue line with a typing effect
    }
    void UpdateSpeakerImage(string speaker) {
        if (speaker == "Player") {
            leftImage.SetActive(true);  // Show player's image on the left
            rightImage.SetActive(false);  // Hide the other character's image
        } else {
            leftImage.SetActive(false);  // Hide player's image
            rightImage.SetActive(true);  // Show the other character's image on the right
        }
    }
    IEnumerator TypeSentence(DialogueLine dialogueLine) {
        // dialogueText.text = dialogueLine.speaker + ": ";  // Show speaker's name
        dialogueText.text = "";
        foreach (char letter in dialogueLine.sentence.ToCharArray()) {
            dialogueText.text += letter;  // Type each letter one by one
            yield return null;
        }
    }

    void EndDialogue() {
        Debug.Log("End of dialogue");
        dialogueBox.SetActive(false);   // Hide the dialogue box when dialogue ends
        leftImage.SetActive(false);  // Hide both images when dialogue ends
        rightImage.SetActive(false);
    }
}