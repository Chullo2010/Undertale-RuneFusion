using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCutManager : MonoBehaviour
{
    [System.Serializable]
    public class SceneCut
    {
        public GameObject sceneObject;
        public TypewriterTMP typewriterText;
    }

    public SceneCut[] scenes;
    public SpriteRenderer blackFade;
    public float fadeSpeed = 1f;
    public float delayBetweenScenes = 1f;
    public string nextSceneName;

    private int currentSceneIndex = -1;
    private bool skipRequested = false;
    private Coroutine mainRoutine;

    private void Start()
    {
        // Disable all scenes and text at the start
        foreach (var cut in scenes)
        {
            if (cut.sceneObject != null)
                cut.sceneObject.SetActive(false);

            if (cut.typewriterText != null && cut.typewriterText.textUI != null)
                cut.typewriterText.textUI.gameObject.SetActive(false);
        }

        if (blackFade != null)
        {
            Color c = blackFade.color;
            c.a = Mathf.Clamp01(c.a);
            blackFade.color = c;

            mainRoutine = StartCoroutine(RunSceneCuts());
        }
        else
        {
            Debug.LogWarning("⚠️ BlackFade not assigned!");
        }
    }

    private void Update()
    {
        // Skip cutscene if player presses Z
        if (Input.GetKeyDown(KeyCode.Z) && !skipRequested)
        {
            skipRequested = true;

            if (mainRoutine != null)
                StopCoroutine(mainRoutine);

            // Disable all current text & scene objects
            foreach (var cut in scenes)
            {
                if (cut.sceneObject != null)
                    cut.sceneObject.SetActive(false);

                if (cut.typewriterText != null && cut.typewriterText.textUI != null)
                    cut.typewriterText.textUI.gameObject.SetActive(false);
            }

            StartCoroutine(SkipToNextScene());
        }
    }

    private IEnumerator RunSceneCuts()
    {
        yield return StartCoroutine(FadeIn());

        for (int i = 0; i < scenes.Length; i++)
        {
            if (skipRequested) yield break;

            currentSceneIndex = i;
            var currentCut = scenes[i];

            if (currentCut.sceneObject != null)
                currentCut.sceneObject.SetActive(true);

            if (currentCut.typewriterText != null && currentCut.typewriterText.textUI != null)
                currentCut.typewriterText.textUI.gameObject.SetActive(true);

            if (currentCut.typewriterText != null)
            {
                currentCut.typewriterText.StartTyping();
                while (currentCut.typewriterText.IsTyping)
                {
                    if (skipRequested) yield break;
                    yield return null;
                }
            }

            yield return new WaitForSeconds(delayBetweenScenes);

            yield return StartCoroutine(FadeOut());

            if (currentCut.sceneObject != null)
                currentCut.sceneObject.SetActive(false);

            if (currentCut.typewriterText != null && currentCut.typewriterText.textUI != null)
                currentCut.typewriterText.textUI.gameObject.SetActive(false);

            if (i < scenes.Length - 1)
                yield return StartCoroutine(FadeIn());
        }

        yield return new WaitForSeconds(1f);
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeIn()
    {
        if (blackFade == null) yield break;

        float startAlpha = blackFade.color.a;
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            float a = Mathf.Lerp(startAlpha, 0f, elapsed);
            Color c = blackFade.color;
            c.a = a;
            blackFade.color = c;
            yield return null;
        }
        Color final = blackFade.color;
        final.a = 0f;
        blackFade.color = final;
    }

    private IEnumerator FadeOut()
    {
        if (blackFade == null) yield break;

        float startAlpha = blackFade.color.a;
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            float a = Mathf.Lerp(startAlpha, 1f, elapsed);
            Color c = blackFade.color;
            c.a = a;
            blackFade.color = c;
            yield return null;
        }
        Color final = blackFade.color;
        final.a = 1f;
        blackFade.color = final;
    }

    private IEnumerator SkipToNextScene()
    {
        // Fade to black before loading next scene
        if (blackFade != null)
            yield return StartCoroutine(FadeOut());

        yield return new WaitForSeconds(0.25f);

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
