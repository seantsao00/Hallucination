[System.Serializable]
public class DialogueLine {
    public string speaker;
    public string sentence;
}

// Represents a named dialogue (a collection of lines with a unique name)
[System.Serializable]
public class DialogueCollection {
    /// <summary>
    /// The name of the dialogue.
    /// </summary>
    public string name;
    /// <summary>
    /// Whether the game should be paused while the dialogue is active.
    /// </summary>
    public bool pauseGame;
    /// <summary>
    /// The actual lines of dialogue.
    /// </summary>
    public DialogueLine[] lines;
}

[System.Serializable]
public class DialogueData {
    public DialogueCollection[] dialogues;  // Array of named dialogues
}
