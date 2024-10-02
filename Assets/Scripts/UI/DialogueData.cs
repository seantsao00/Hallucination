using System.Collections.Generic;

// Represents a single line of dialogue with a speaker and a sentence
[System.Serializable]
public class DialogueLine {
    public string speaker;
    public string sentence;
}

// Represents a named dialogue (a collection of lines with a unique name)
[System.Serializable]
public class DialogueCollection {
    public string name;  // The name of the dialogue (e.g., "introduction", "quest_start")
    public DialogueLine[] lines;  // The actual lines of dialogue
}

// Represents the entire set of dialogues, stored as an array of DialogueCollection
[System.Serializable]
public class DialogueData {
    public DialogueCollection[] dialogues;  // Array of named dialogues
}
