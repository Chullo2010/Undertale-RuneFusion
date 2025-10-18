using UnityEngine;
using System.Collections;


public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    public void ChangeMusic(string musicName)
    {
        if (string.IsNullOrEmpty(musicName))
            return; // No new track set, keep playing the current one

        AudioClip newClip = Resources.Load<AudioClip>(musicName);
        if (newClip == null)
        {
            Debug.LogWarning($"Music '{musicName}' not found in Resources!");
            return;
        }

        if (audioSource.clip == newClip)
            return; // Already playing this track

        StartCoroutine(FadeToNewTrack(newClip));
    }

    private IEnumerator FadeToNewTrack(AudioClip newClip)
    {
        // Fade out
        float startVolume = audioSource.volume;
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t);
            yield return null;
        }
    }
}
