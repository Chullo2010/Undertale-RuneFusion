using UnityEngine;

public class RoomEntryPoint : MonoBehaviour
{
    public string transitionID = "A"; // Match this with the one from the exit in the previous scene

    void Start()
    {
        string lastID = PlayerPrefs.GetString("LastTransitionID", "");

        // If the last transition matches this one, move the player here
        if (transitionID == lastID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = transform.position;
            }

            // Clear ID so it doesn't respawn here again next load
            PlayerPrefs.DeleteKey("LastTransitionID");
        }
    }
}
