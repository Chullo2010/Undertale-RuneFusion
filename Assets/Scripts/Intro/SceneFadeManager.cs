using System.Collections;
using UnityEngine;

public class SceneFadeManager : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("Assign a GameObject with a SpriteRenderer (usually a black full-screen sprite).")]
    public SpriteRenderer blackFadeSprite;

    [Tooltip("How long it takes to fade in/out.")]
    public float fadeDuration = 1.5f;

    [Tooltip("Automatically fade from black on start.")]
    public bool fadeFromBlackOnStart = true;

    private Coroutine currentFade;

    private void Start()
    {
        if (blackFadeSprite != null)
        {
            // Ensure alpha starts correct
            Color color = blackFadeSprite.color;
            color.a = fadeFromBlackOnStart ? 1f : 0f;
            blackFadeSprite.color = color;

            if (fadeFromBlackOnStart)
                FadeOut();
        }
        else
        {
            Debug.LogWarning("SceneFadeManager: No SpriteRenderer assigned!");
        }
    }

    public void FadeIn()
    {
        if (blackFadeSprite == null) return;

        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(Fade(1f));
    }

    public void FadeOut()
    {
        if (blackFadeSprite == null) return;

        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = blackFadeSprite.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            Color color = blackFadeSprite.color;
            color.a = alpha;
            blackFadeSprite.color = color;

            yield return null;
        }

        Color finalColor = blackFadeSprite.color;
        finalColor.a = targetAlpha;
        blackFadeSprite.color = finalColor;
    }
}
