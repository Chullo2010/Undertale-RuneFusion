using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public string spawnTag = "MarkA"; // Tag of the mark where Frisk should spawn

    void Start()
    {
        // Find the mark in the scene with the assigned tag
        GameObject mark = GameObject.FindGameObjectWithTag(spawnTag);
        if (mark != null)
        {
            transform.position = mark.transform.position;
        }
        else
        {
            Debug.LogWarning("No mark found with tag: " + spawnTag + ". Using current position.");
        }
    }
}
