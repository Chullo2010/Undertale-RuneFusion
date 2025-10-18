using System.Collections;
using UnityEngine;

public class MenuAppear : MonoBehaviour
{
    [Header("Target Settings")]
    public GameObject targetMenu;        // The menu GameObject to enable
    public float delayBeforeAppear = 1f; // Delay before it shows
    public AudioSource appearSound;      // Optional sound to play

    private void Start()
    {
        if (targetMenu != null)
        {
            // Make sure the menu starts hidden
            targetMenu.SetActive(false);
            // Start the coroutine safely
            StartCoroutine(ShowMenuAfterDelay());
        }
        else
        {
            Debug.LogWarning("MenuAppear: No targetMenu assigned!");
        }
    }

    private IEnumerator ShowMenuAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeAppear);

        // Enable the menu object
        targetMenu.SetActive(true);

        // Play sound
        if (appearSound != null)
        {
            appearSound.Play();
        }
    }
}
