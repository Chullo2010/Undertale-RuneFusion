using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class RoomTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public string targetSceneName;  // Scene to load
    public string targetMarkTag = "MarkA"; // Where to spawn in next scene
    public Image fadeImage;         // Fade UI Image
    public float fadeDuration = 1f; // Fade in/out speed
    public float cooldownAfterLoad = 0.5f; // Prevent instant re-trigger

    private static bool isTransitioning = false;
    private static string nextSpawnTag = "MarkA";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(DoTransition());
        }
    }

    private IEnumerator DoTransition()
    {
        isTransitioning = true;

        // Fade out
        if (fadeImage != null)
            yield return StartCoroutine(Fade(1));

        // Save where to spawn next
        nextSpawnTag = targetMarkTag;

        // Hook into the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(targetSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find player and target mark
        GameObject player = GameObject.FindWithTag("Player");
        GameObject mark = GameObject.FindWithTag(nextSpawnTag);

        if (player != null && mark != null)
        {
            player.transform.position = mark.transform.position;
        }

        // Get fade image from the new scene
        GameObject fadeObj = GameObject.Find("FadeImage");
        Image newFade = fadeObj ? fadeObj.GetComponent<Image>() : null;

        // Run fade-in coroutine safely on the player, not this (which will be destroyed)
        if (player != null)
        {
            player.GetComponent<MonoBehaviour>().StartCoroutine(FadeInAfterLoad(newFade));
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeImage == null) yield break;

        Color c = fadeImage.color;
        float startAlpha = c.a;
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        fadeImage.color = c;
    }

    private IEnumerator FadeInAfterLoad(Image fadeImg)
    {
        yield return new WaitForSeconds(0.1f);

        if (fadeImg != null)
        {
            Color c = fadeImg.color;
            float startAlpha = c.a;
            float t = 0;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(startAlpha, 0, t / fadeDuration);
                fadeImg.color = c;
                yield return null;
            }

            c.a = 0;
            fadeImg.color = c;
        }

        yield return new WaitForSeconds(cooldownAfterLoad);
        isTransitioning = false;
    }
}
