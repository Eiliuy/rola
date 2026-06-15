using UnityEngine;

/// <summary>
/// 简单的相机跟随，带平滑插值与边界限制
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("边界限制")]
    public bool useBounds = false;
    public Bounds cameraBounds;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (useBounds)
            desiredPosition = ClampCameraPosition(desiredPosition);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    Vector3 ClampCameraPosition(Vector3 position)
    {
        if (cam == null) cam = GetComponent<Camera>();

        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        float minX = cameraBounds.min.x + width;
        float maxX = cameraBounds.max.x - width;
        float minY = cameraBounds.min.y + height;
        float maxY = cameraBounds.max.y - height;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }

    void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(cameraBounds.center, cameraBounds.size);
    }
}
