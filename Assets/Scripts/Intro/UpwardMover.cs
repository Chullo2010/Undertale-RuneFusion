using UnityEngine;

public class UpwardMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;         // Units per second
    public float startDelay = 1f;        // Delay before movement starts
    public float moveDistance = 5f;      // How far to move upward

    [Header("Activation Settings")]
    public GameObject triggerObject;     // Object that controls activation

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool hasStarted = false;
    private float delayTimer = 0f;

    private void Awake()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * moveDistance;
    }

    private void Update()
    {
        if (triggerObject == null)
        {
            // No trigger? Always active and moving
            HandleMovement();
            return;
        }

        // If trigger object is active
        if (triggerObject.activeInHierarchy)
        {
            if (!gameObject.activeSelf)
            {
                // Reactivate this object when trigger turns on
                gameObject.SetActive(true);
                ResetMovement();
            }

            HandleMovement();
        }
        else
        {
            if (gameObject.activeSelf)
            {
                // Disable this object when trigger turns off
                gameObject.SetActive(false);
                hasStarted = false;
                delayTimer = 0f;
            }
        }
    }

    private void ResetMovement()
    {
        transform.position = startPosition;
        hasStarted = false;
        delayTimer = 0f;
    }

    private void HandleMovement()
    {
        if (!hasStarted)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= startDelay)
                hasStarted = true;
        }

        if (hasStarted && Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}
