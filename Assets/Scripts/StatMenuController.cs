using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class STATMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject statCanvas;         // Main stat menu canvas (kept visible when sub-menu opens)
    public GameObject itemCanvas;         // Sub-canvas for item/stat details (appears on Z)
    public RectTransform soul;            // Soul UI (RectTransform) — should be under same canvas as stat buttons

    [Header("Options (order matters)")]
    [Tooltip("Assign the RectTransforms of the selectable buttons in order (e.g. ITEM, STAT, ...).")]
    public RectTransform[] optionRects;   // Assign each menu option's RectTransform in Inspector
    public string[] optionNames;          // Optional: names for logic (same order as optionRects)

    [Header("Option Enabling")]
    public bool itemEnabled = false;      // If false, ITEM is grayed and not selectable

    [Header("Audio")]
    public AudioSource menuAudioSource;   // Game Audio (use Frisk's Game Audio or UI audio source)
    public AudioClip moveSound;
    public AudioClip selectSound;
    public AudioClip denySound;

    [Header("Soul Positioning")]
    public Vector2 soulOffset = new Vector2(-40f, 0f); // local offset from target button anchoredPosition

    [Header("Input")]
    public float inputCooldown = 0.12f;

    // internal
    int selectedIndex = 0;
    bool isOpen = false;
    bool canPress = true;

    void Start()
    {
        if (statCanvas == null) Debug.LogError("STATMenu: statCanvas not assigned.");
        if (soul == null) Debug.LogError("STATMenu: soul RectTransform not assigned.");
        if (optionRects == null || optionRects.Length == 0)
            Debug.LogWarning("STATMenu: optionRects not assigned. Will try to find option Transforms by optionNames.");

        // Hide both at start
        statCanvas.SetActive(false);
        if (itemCanvas) itemCanvas.SetActive(false);

        // Gray out ITEM text if disabled (attempt to find an Image/Text component if present)
        ApplyOptionVisuals();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && canPress)
        {
            ToggleMenu();
            StartCoroutine(TempInputDelay());
        }

        if (!isOpen) return;

        HandleNavigation();
    }

    void ToggleMenu()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            // disable player movement
            FriskMovement.CanMove = false;

            statCanvas.SetActive(true);
            if (itemCanvas) itemCanvas.SetActive(false);
            selectedIndex = 0;
            UpdateSoulPositionImmediate();
        }
        else
        {
            // re-enable player movement
            FriskMovement.CanMove = true;

            statCanvas.SetActive(false);
            if (itemCanvas) itemCanvas.SetActive(false);
        }
    }

    void HandleNavigation()
    {
        if (!canPress) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);
            PlayMove();
            UpdateSoulPositionImmediate();
            StartCoroutine(TempInputDelay());
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = Mathf.Min(GetOptionCount() - 1, selectedIndex + 1);
            PlayMove();
            UpdateSoulPositionImmediate();
            StartCoroutine(TempInputDelay());
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            OnConfirm();
            StartCoroutine(TempInputDelay());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            // close sub menu if open, otherwise close whole menu
            if (itemCanvas != null && itemCanvas.activeSelf)
            {
                itemCanvas.SetActive(false);
                PlayMove(); // use move sound as feedback
            }
            else
            {
                ToggleMenu();
            }
            StartCoroutine(TempInputDelay());
        }
    }

    void OnConfirm()
    {
        int idx = selectedIndex;
        string optName = GetOptionName(idx);

        // If option is ITEM and disabled -> play deny
        if (IsItemAtIndex(idx) && !itemEnabled)
        {
            PlayDeny();
            return;
        }

        // Play select sound
        if (menuAudioSource != null && selectSound != null)
            menuAudioSource.PlayOneShot(selectSound);

        // Open sub-canvas for that option if available
        if (optName != null && optName.ToUpper().Contains("ITEM"))
        {
            if (itemCanvas != null)
            {
                itemCanvas.SetActive(true);
                // do item setup here (inventory not implemented yet)
            }
        }
        else if (optName != null && optName.ToUpper().Contains("STAT"))
        {
            if (itemCanvas != null)
            {
                itemCanvas.SetActive(true);
                // populate STAT details in itemCanvas if desired
            }
        }
        else
        {
            // fallback: open itemCanvas if exists
            if (itemCanvas != null)
                itemCanvas.SetActive(true);
        }
    }

    // Immediately place soul at target option (no smoothing)
    void UpdateSoulPositionImmediate()
    {
        RectTransform target = GetOptionRect(selectedIndex);
        if (target == null) return;

        // We expect the soul to be under the same canvas as the target.
        // Use anchoredPosition to align in canvas-local space.
        Vector2 anchored = target.anchoredPosition;
        soul.anchoredPosition = anchored + soulOffset;
    }

    // Helpers
    RectTransform GetOptionRect(int index)
    {
        if (optionRects != null && index >= 0 && index < optionRects.Length && optionRects[index] != null)
            return optionRects[index];

        // fallback: try to find by name under statCanvas using optionNames
        if (optionNames != null && index < optionNames.Length)
        {
            Transform candidate = statCanvas.transform.Find(optionNames[index]);
            if (candidate != null) return candidate.GetComponent<RectTransform>();
        }

        return null;
    }

    string GetOptionName(int index)
    {
        if (optionNames != null && index < optionNames.Length) return optionNames[index];
        RectTransform r = GetOptionRect(index);
        if (r != null) return r.gameObject.name;
        return null;
    }

    int GetOptionCount()
    {
        if (optionRects != null && optionRects.Length > 0) return optionRects.Length;
        if (optionNames != null && optionNames.Length > 0) return optionNames.Length;
        return 0;
    }

    bool IsItemAtIndex(int idx)
    {
        // look for option named "ITEM" (case-insensitive) or index 0 commonly
        string name = GetOptionName(idx);
        if (string.IsNullOrEmpty(name)) return false;
        return name.ToUpper().Contains("ITEM");
    }

    void ApplyOptionVisuals()
    {
        // gray out ITEM if disabled (try to find a Text or TMP or Image component)
        for (int i = 0; i < GetOptionCount(); i++)
        {
            if (IsItemAtIndex(i))
            {
                RectTransform r = GetOptionRect(i);
                if (r == null) continue;

                // Attempt to find Text, TMP, or Image on the button
                Text uiText = r.GetComponentInChildren<Text>();
                if (uiText != null)
                {
                    uiText.color = itemEnabled ? Color.white : Color.gray;
                    continue;
                }

                // Try TMPro (if used)
#if TMP_PRESENT
                TMPro.TextMeshProUGUI tmp = r.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.color = itemEnabled ? Color.white : Color.gray;
                    continue;
                }
#endif
            }
        }
    }

    void PlayMove()
    {
        if (menuAudioSource != null && moveSound != null)
            menuAudioSource.PlayOneShot(moveSound);
    }

    void PlayDeny()
    {
        if (menuAudioSource != null && denySound != null)
            menuAudioSource.PlayOneShot(denySound);
    }

    IEnumerator TempInputDelay()
    {
        canPress = false;
        yield return new WaitForSeconds(inputCooldown);
        canPress = true;
    }

    // Public helper so other scripts can enable/disable ITEM at runtime
    public void SetItemEnabled(bool enabled)
    {
        itemEnabled = enabled;
        ApplyOptionVisuals();
    }
}
