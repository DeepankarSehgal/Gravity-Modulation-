using UnityEngine;

/// <summary>
/// Third-person orbit camera that stays aligned with current gravity (Physics.gravity).
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;            // player

    [Header("Distance")]
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomSpeed = 2f;

    [Header("Orbit")]
    public float mouseSensitivity = 3f;
    public float minPitch = -40f;
    public float maxPitch = 70f;
    public bool requireRightMouseButton = true;

    [Header("Smoothing")]
    public float followSpeed = 10f;

    private float _yaw;
    private float _pitch;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("ThirdPersonCamera: No target assigned.");
            return;
        }

        // Initialize yaw/pitch from current orientation
        Vector3 toCamera = (transform.position - target.position).normalized;

        _yaw = Mathf.Atan2(toCamera.x, toCamera.z) * Mathf.Rad2Deg;
        _pitch = Mathf.Asin(toCamera.y) * Mathf.Rad2Deg;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        HandleZoom();
        HandleOrbitInput();

        // Use Physics.gravity to define up
        Vector3 gravityDown = Physics.gravity.sqrMagnitude > 0.0001f
            ? Physics.gravity.normalized
            : Vector3.down;
        Vector3 gravityUp = -gravityDown;

        // Build right/forward relative to gravity up
        Vector3 right = Vector3.Cross(Vector3.forward, gravityUp);
        if (right.sqrMagnitude < 0.001f)
        {
            right = Vector3.Cross(Vector3.right, gravityUp);
        }
        right.Normalize();
        Vector3 forward = Vector3.Cross(gravityUp, right).normalized;

        // Clamp pitch
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        // Orientation from yaw & pitch
        Quaternion yawRot = Quaternion.AngleAxis(_yaw, gravityUp);
        Quaternion pitchRot = Quaternion.AngleAxis(_pitch, right);
        Quaternion orbitRotation = yawRot * pitchRot;

        // Desired camera position
        Vector3 desiredOffset = orbitRotation * (-forward * distance);
        Vector3 desiredPosition = target.position + desiredOffset;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Look at target, using gravityUp as up
        Vector3 lookTarget = target.position + gravityUp * 1.5f;
        Vector3 lookDir = (lookTarget - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(lookDir, gravityUp);
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    private void HandleOrbitInput()
    {
        if (requireRightMouseButton && !Input.GetMouseButton(1))
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _yaw += mouseX * mouseSensitivity;
        _pitch -= -mouseY * mouseSensitivity;
    }
}
