using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlaytimeManager : MonoBehaviour {
    public static PlaytimeManager Instance;
    private float playtime = 0f;   // Total time in seconds
    private bool isRunning = true; // Timer state
    public TextMeshProUGUI playtimeText;
    private Button toggleButton;

    private void Awake() {
        // Ensure only one instance exists
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            SceneManager.sceneLoaded += OnSceneLoaded; // Add scene load listener

        } else {
            Destroy(gameObject); // Destroy duplicate
        }
    }

    private void Start() {
        // Manually call OnSceneLoaded for the initial scene
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    // Called whenever a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // Find the TextMeshProUGUI component in the current scene
        playtimeText = GameObject.Find("PlaytimeText")?.GetComponent<TextMeshProUGUI>();

        // Find the Toggle Button and set up the OnClick event
        toggleButton = GameObject.Find("ToggleTimerButton")?.GetComponent<Button>();
        if (toggleButton != null) {
            // Clear previous listeners and add ToggleTimerVisibility
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(ToggleTimerVisibility);
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Remove listener when object is destroyed
    }

    // Update is called once per frame
    void Update() {
        if (isRunning) {
            playtime += Time.deltaTime;
            if (playtimeText != null) {
                playtimeText.text = GetFormattedPlaytime();
            }
        }
    }

    // Start or continue the timer
    public void StartTimer() {
        isRunning = true;
    }

    // Stop or pause the timer
    public void StopTimer() {
        isRunning = false;
    }

    // Reset the timer
    public void ResetTimer() {
        playtime = 0f;
        isRunning = false;
    }

    // Get the total playtime in seconds
    public float GetPlaytime() {
        return playtime;
    }

    // Format the playtime as HH:MM:SS
    public string GetFormattedPlaytime() {
        int hours = Mathf.FloorToInt(playtime / 3600);
        int minutes = Mathf.FloorToInt((playtime % 3600) / 60);
        int seconds = Mathf.FloorToInt(playtime % 60);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }

    // Toggle the visibility of the timer display
    public void ToggleTimerVisibility() {
        if (playtimeText != null) {
            playtimeText.enabled = !playtimeText.enabled;
        }
    }
}
