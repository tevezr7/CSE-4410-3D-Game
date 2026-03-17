using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [Header("Music")]
    public AudioClip backgroundMusic;

    [Header("Audio Mixer")]
    public AudioMixerGroup audioMixerGroup;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    // The AudioSource that plays the music
    public AudioSource audioSource;

    void Awake()
    {
        // Use existing AudioSource if attached, otherwise add one
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = musicVolume;
        audioSource.playOnAwake = false;

        // Assign the mixer group so master volume slider works
        if (audioMixerGroup != null)
            audioSource.outputAudioMixerGroup = audioMixerGroup;
    }

    void Start()
    {
        if (backgroundMusic != null)
            audioSource.Play();
        else
            Debug.LogWarning("MusicManager: No background music assigned!");
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}