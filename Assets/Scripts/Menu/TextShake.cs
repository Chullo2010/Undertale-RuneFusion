using UnityEngine;
using TMPro;

public class TextShake : MonoBehaviour
{
    public float intensity = 1f;     // How strong the shake is
    public float speed = 10f;        // How fast the shake moves
    public bool shake = true;        // If true, shaking is active

    private TextMeshProUGUI textMesh;
    private Vector3 originalPosition;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("TextShake: No TextMeshProUGUI found on this object!");
            enabled = false;
            return;
        }

        originalPosition = textMesh.rectTransform.localPosition;
    }

    void Update()
    {
        if (shake)
        {
            float offsetX = Mathf.Sin(Time.time * speed) * intensity;
            float offsetY = Mathf.Cos(Time.time * speed * 0.8f) * intensity * 0.5f;

            textMesh.rectTransform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
        }
        else
        {
            textMesh.rectTransform.localPosition = originalPosition;
        }
    }
}
