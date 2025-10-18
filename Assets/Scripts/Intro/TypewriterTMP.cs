using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterTMP : MonoBehaviour
{
    public TMP_Text textUI;
    [TextArea] public string fullText;
    public float typingSpeed = 0.05f;
    public AudioSource textAudio;

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    // Toggle to skip sound for spaces/punctuation
    public bool skipSilentCharacters = true;

    // Start typing with the assigned fullText
    public void StartTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    // Start typing with custom text
    public void StartTyping(string newText)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        fullText = newText;
        typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string textToType)
    {
        if (textUI == null)
            yield break;

        isTyping = true;
        textUI.text = "";

        for (int i = 0; i < textToType.Length; i++)
        {
            // Handle color codes like @C1
            if (textToType[i] == '@' && i + 2 < textToType.Length && textToType[i + 1] == 'C')
            {
                char colorCode = textToType[i + 2];
                string colorHex = GetColorFromCode(colorCode);
                textUI.text += $"<color={colorHex}>";
                i += 2;
                continue;
            }

            // Add character
            textUI.text += textToType[i];

            // Play sound for visible characters
            if (textAudio != null && textAudio.clip != null)
            {
                char c = textToType[i];
                if (!skipSilentCharacters || char.IsLetterOrDigit(c))
                {
                    textAudio.pitch = Random.Range(0.95f, 1.05f);
                    textAudio.PlayOneShot(textAudio.clip);
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void StopTypingAndClear()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (textAudio != null && textAudio.isPlaying)
            textAudio.Stop();

        if (textUI != null)
            textUI.text = "";

        isTyping = false;
    }

    private string GetColorFromCode(char code)
    {
        switch (code)
        {
            case '1': return "#FFFF00"; // Yellow
            case '2': return "#FF0000"; // Red
            case '3': return "#00BFFF"; // Blue
            case '4': return "#00FF00"; // Green
            default: return "#FFFFFF";  // White
        }
    }

    // ✅ So SceneCutManager can check typing state
    public bool IsTyping
    {
        get { return isTyping; }
    }
}
