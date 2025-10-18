using UnityEngine;

/// <summary>
/// When the target text GameObject is visible, pressing one of the trigger keys
/// will enable/disable assigned objects and canvases. Also attempts to stop any
/// TypewriterTMP on disabled objects so text/sound won't keep running.
/// </summary>
public class EnableDisableOnInput : MonoBehaviour
{
    [Header("Trigger")]
    [Tooltip("The text GameObject that must be active before input works.")]
    public GameObject targetText;

    [Header("Enable / Disable Lists")]
    [Tooltip("GameObjects to ENABLE when key is pressed.")]
    public GameObject[] objectsToEnable;

    [Tooltip("GameObjects to DISABLE when key is pressed.")]
    public GameObject[] objectsToDisable;

    [Tooltip("Canvas components to ENABLE when key is pressed.")]
    public Canvas[] canvasesToEnable;

    [Tooltip("Canvas components to DISABLE when key is pressed.")]
    public Canvas[] canvasesToDisable;

    [Header("Input Settings")]
    public KeyCode[] triggerKeys = { KeyCode.Z, KeyCode.Return, KeyCode.Alpha2 };

    [Header("Extras")]
    [Tooltip("If true, the script will print debug lines to the Console.")]
    public bool debugLogs = true;

    bool hasActivated = false;

    private void Update()
    {
        if (hasActivated) return;

        // Make sure the text (trigger) exists and is visible in the scene
        if (targetText == null)
        {
            if (debugLogs) Debug.LogWarning("[EnableDisableOnInput] targetText is not assigned.");
            return;
        }

        if (!targetText.activeInHierarchy) return;

        // Check for any trigger key
        foreach (KeyCode key in triggerKeys)
        {
            if (Input.GetKeyDown(key))
            {
                if (debugLogs) Debug.Log($"[EnableDisableOnInput] Key pressed: {key}. Activating targets...");
                ActivateTargets();
                break;
            }
        }
    }

    private void ActivateTargets()
    {
        hasActivated = true;

        // Enable GameObjects
        if (objectsToEnable != null)
        {
            foreach (var obj in objectsToEnable)
            {
                if (obj == null) continue;
                obj.SetActive(true);
                if (debugLogs) Debug.Log($"[EnableDisableOnInput] Enabled GameObject: {obj.name}");
            }
        }

        // Enable Canvases (ensure their GameObject is active first, then enable component)
        if (canvasesToEnable != null)
        {
            foreach (var c in canvasesToEnable)
            {
                if (c == null) continue;
                if (!c.gameObject.activeSelf) c.gameObject.SetActive(true);
                c.enabled = true;
                if (debugLogs) Debug.Log($"[EnableDisableOnInput] Enabled Canvas: {c.gameObject.name}");
            }
        }

        // Disable GameObjects (and stop any TypewriterTMP on them)
        if (objectsToDisable != null)
        {
            foreach (var obj in objectsToDisable)
            {
                if (obj == null) continue;

                // If it has a TypewriterTMP, stop typing & clear
                var type = obj.GetComponentInChildren<TypewriterTMP>();
                if (type != null)
                {
                    type.StopTypingAndClear();
                    if (debugLogs) Debug.Log($"[EnableDisableOnInput] Stopped Typewriter on: {obj.name}");
                }

                obj.SetActive(false);
                if (debugLogs) Debug.Log($"[EnableDisableOnInput] Disabled GameObject: {obj.name}");
            }
        }

        // Disable Canvases (disable component first, then optionally deactivate GameObject)
        if (canvasesToDisable != null)
        {
            foreach (var c in canvasesToDisable)
            {
                if (c == null) continue;
                c.enabled = false;
                if (c.gameObject.activeSelf) c.gameObject.SetActive(false);
                if (debugLogs) Debug.Log($"[EnableDisableOnInput] Disabled Canvas: {c.gameObject.name}");
            }
        }

        // Finally, disable the trigger text itself (so it disappears)
        if (targetText != null)
        {
            // If the target text has a TypewriterTMP, stop it cleanly
            var tw = targetText.GetComponentInChildren<TypewriterTMP>();
            if (tw != null)
            {
                tw.StopTypingAndClear();
                if (debugLogs) Debug.Log($"[EnableDisableOnInput] Stopped Typewriter on triggerText: {targetText.name}");
            }

            targetText.SetActive(false);
            if (debugLogs) Debug.Log($"[EnableDisableOnInput] Disabled triggerText: {targetText.name}");
        }
    }
}
