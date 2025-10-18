using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // assign Frisk

    [Header("Follow")]
    public bool followX = true;
    public bool followY = true;
    [Tooltip("How quickly the camera catches up. Larger = snappier.")]
    public float smoothTime = 0.12f;

    [Header("Bounds (world coordinates)")]
    public bool useBounds = false;
    public Vector2 minPosition; // bottom-left of room (world units)
    public Vector2 maxPosition; // top-right of room (world units)

    // Internals
    Camera cam;
    Vector3 velocity = Vector3.zero;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            // don't spam logs every frame — just a single warning
            if (!Application.isPlaying) return;
            Debug.LogWarning("CameraFollow2D: target is not assigned.");
            return;
        }

        // 1) desired position follows target on selected axes
        Vector3 desired = transform.position;
        Vector3 targetPos = target.position;

        if (followX) desired.x = targetPos.x;
        if (followY) desired.y = targetPos.y;

        // keep camera z
        desired.z = transform.position.z;

        // 2) apply bounds if requested (account for camera viewport size)
        if (useBounds)
        {
            // compute camera half extents in world units
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = camHalfHeight * cam.aspect;

            // compute the clamping min/max for camera center
            float minX = minPosition.x + camHalfWidth;
            float maxX = maxPosition.x - camHalfWidth;
            float minY = minPosition.y + camHalfHeight;
            float maxY = maxPosition.y - camHalfHeight;

            // If room is smaller than camera width on an axis, center camera on room center on that axis
            if (minX > maxX)
            {
                float centerX = (minPosition.x + maxPosition.x) * 0.5f;
                desired.x = centerX;
            }
            else
            {
                desired.x = Mathf.Clamp(desired.x, minX, maxX);
            }

            if (minY > maxY)
            {
                float centerY = (minPosition.y + maxPosition.y) * 0.5f;
                desired.y = centerY;
            }
            else
            {
                desired.y = Mathf.Clamp(desired.y, minY, maxY);
            }
        }

        // 3) smooth move the camera to desired position
        Vector3 smooth = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        transform.position = smooth;
    }

    // Helper: set bounds from a Rect
    public void SetBounds(Rect bounds)
    {
        useBounds = true;
        minPosition = bounds.min;
        maxPosition = bounds.max;
    }

    // Helper: enable/disable follow axes at runtime
    public void SetFollowAxes(bool x, bool y)
    {
        followX = x;
        followY = y;
    }
}
