using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TextMenuSelector : MonoBehaviour
{
    [Header("Menu Settings")]
    public GameObject[] canvasesToEnable;       // Each menu canvas
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public float inputDelay = 0.2f; // Delay between inputs
    public bool useVertical = true; // Up/Down or Left/Right navigation

    private int currentIndex = 0;
    private float inputTimer;
    private int activeCanvasIndex = -1;
    private List<TextMeshProUGUI> activeOptions = new List<TextMeshProUGUI>();

    void Start()
    {
        // Disable all canvases on start
        foreach (GameObject canvas in canvasesToEnable)
        {
            if (canvas != null)
                canvas.SetActive(false);
        }

        // Start with none active
        activeCanvasIndex = -1;
    }

    void Update()
    {
        if (activeCanvasIndex == -1 || canvasesToEnable[activeCanvasIndex] == null || !canvasesToEnable[activeCanvasIndex].activeSelf)
            return; // no active canvas = ignore input

        inputTimer -= Time.deltaTime;

        if (inputTimer <= 0)
        {
            if (useVertical)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    currentIndex = (currentIndex - 1 + activeOptions.Count) % activeOptions.Count;
                    UpdateSelection();
                    inputTimer = inputDelay;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    currentIndex = (currentIndex + 1) % activeOptions.Count;
                    UpdateSelection();
                    inputTimer = inputDelay;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentIndex = (currentIndex - 1 + activeOptions.Count) % activeOptions.Count;
                    UpdateSelection();
                    inputTimer = inputDelay;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    currentIndex = (currentIndex + 1) % activeOptions.Count;
                    UpdateSelection();
                    inputTimer = inputDelay;
                }
            }
        }

        // Confirm selection
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            ActivateOption(currentIndex);
        }

        // Optional: go back with X or Escape
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape))
        {
            GoBackToPrevious();
        }
    }

    // Enables one canvas and grabs its TextMeshProUGUI buttons
    public void SetActiveCanvas(int index)
    {
        if (index < 0 || index >= canvasesToEnable.Length)
            return;

        // Disable all canvases
        foreach (GameObject canvas in canvasesToEnable)
        {
            if (canvas != null)
                canvas.SetActive(false);
        }

        // Enable target canvas
        canvasesToEnable[index].SetActive(true);
        activeCanvasIndex = index;

        // Find selectable text buttons inside it
        activeOptions.Clear();
        TextMeshProUGUI[] allTexts = canvasesToEnable[index].GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (var t in allTexts)
        {
            // Only include ones that aren’t titles, etc.
            if (t.gameObject.CompareTag("MenuOption"))
                activeOptions.Add(t);
        }

        if (activeOptions.Count > 0)
        {
            currentIndex = 0;
            UpdateSelection();
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < activeOptions.Count; i++)
        {
            activeOptions[i].color = (i == currentIndex) ? selectedColor : normalColor;
        }
    }

    void ActivateOption(int index)
    {
        if (index < 0 || index >= canvasesToEnable.Length)
            return;

        Debug.Log("Selected: " + activeOptions[index].text);

        // If this menu option corresponds to another canvas, enable it
        if (index < canvasesToEnable.Length && canvasesToEnable[index] != null)
        {
            SetActiveCanvas(index);
        }
    }

    void GoBackToPrevious()
    {
        // Disable current, enable previous if exists
        if (activeCanvasIndex > 0)
        {
            SetActiveCanvas(activeCanvasIndex - 1);
        }
    }
}
