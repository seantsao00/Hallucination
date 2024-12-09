using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Music Tracks")]
    public AudioClip[] musicClips;
    public float[] musicVolumes = {0.5f, 0.25f};
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    int currentTrackIndex = -1;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = false;
        audioSource.volume = musicVolume;
    }

    public void PlayMusic(int trackIndex) {
        if (trackIndex < 0 || trackIndex >= musicClips.Length) return;

        if (currentTrackIndex == trackIndex && audioSource.isPlaying) return;

        StartCoroutine(HandleTrackChange(trackIndex));
    }

    IEnumerator HandleTrackChange(int trackIndex) {
        if (currentTrackIndex != -1) yield return StartCoroutine(MusicFadeOut());
        audioSource.clip = musicClips[trackIndex];
        audioSource.loop = true;
        audioSource.Play();

        currentTrackIndex = trackIndex;
        StartCoroutine(MusicFadeIn());
    }

    public void StopMusic()
    {
        audioSource.Stop();
        currentTrackIndex = -1;
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        audioSource.volume = musicVolume;
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }
    IEnumerator MusicFadeIn() {
        float fadeTimer = 0;
        while (fadeTimer < 3) {
            fadeTimer += Time.deltaTime;
            SetVolume(musicVolumes[currentTrackIndex] * (fadeTimer / 2f)); 
            yield return null;
        }
    }

    IEnumerator MusicFadeOut() {
        float fadeTimer = 0;
        while (fadeTimer < 3) {
            fadeTimer += Time.deltaTime;
            SetVolume(musicVolumes[currentTrackIndex] * ((2f - fadeTimer) / 2f)); 
            yield return null;
        }
    }
}

