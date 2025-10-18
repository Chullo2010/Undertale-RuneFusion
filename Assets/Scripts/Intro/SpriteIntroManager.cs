using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SpriteIntroManager : MonoBehaviour
{
    [Header("Setup")]
    public SpriteRenderer[] scenes;  // All your intro sprites (Scene1, Scene2, etc.)
    public SpriteRenderer fadeSprite; // The black overlay sprite

    [Header("Settings")]
    public float fadeSpeed = 1.5f;
    public float sceneDuration = 3f;
    public bool skipWithKey = true;
    public KeyCode skipKey = KeyCode.Space;

    private void Start()
    {
        // Hide all scenes at start
        foreach (var s in scenes)
            s.color = new Color(1, 1, 1, 0);

        fadeSprite.color = Color.black;
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            // Fade in next scene
            yield return StartCoroutine(FadeScene(scenes[i], true));

            // Wait or skip
            float elapsed = 0f;
            while (elapsed < sceneDuration)
            {
                if (skipWithKey && Input.GetKeyDown(skipKey))
                    break;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Fade out to black
            yield return StartCoroutine(FadeScene(scenes[i], false));
        }

        // Fade to black at the end
        yield return StartCoroutine(FadeToBlack());
        // You can load your next scene here if you want
        // SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator FadeScene(SpriteRenderer scene, bool fadeIn)
    {
        float t = 0f;

        // Fade in
        if (fadeIn)
        {
            // Start black, fade to visible
            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;

                // Fade sprite in
                Color c = scene.color;
                c.a = Mathf.Lerp(0, 1, t);
                scene.color = c;

                // Fade black sprite out
                Color f = fadeSprite.color;
                f.a = Mathf.Lerp(1, 0, t);
                fadeSprite.color = f;

                yield return null;
            }
        }
        else // Fade out
        {
            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;

                // Fade sprite out
                Color c = scene.color;
                c.a = Mathf.Lerp(1, 0, t);
                scene.color = c;

                // Fade black sprite in
                Color f = fadeSprite.color;
                f.a = Mathf.Lerp(0, 1, t);
                fadeSprite.color = f;

                yield return null;
            }
        }
    }

    private IEnumerator FadeToBlack()
    {
        float t = 0f;
        Color f = fadeSprite.color;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            f.a = Mathf.Lerp(0, 1, t);
            fadeSprite.color = f;
            yield return null;
        }
    }
}
