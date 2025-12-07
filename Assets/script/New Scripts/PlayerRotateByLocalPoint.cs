using UnityEngine;

public class PlayerRotateByLocalPoint : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // assign Main Camera
    [SerializeField] private float rotateSpeed = 25f;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");   // W/S
        float sideInput    = Input.GetAxis("Horizontal"); // A/D

        if (Mathf.Abs(forwardInput) < 0.001f && Mathf.Abs(sideInput) < 0.001f)
            return;

        Vector3 gravityDown = Physics.gravity.sqrMagnitude > 0.0001f
            ? Physics.gravity.normalized
            : Vector3.down;
        Vector3 gravityUp = -gravityDown;

        // same surface basis as movement
        Vector3 camForward = (cameraTransform != null ? cameraTransform.forward : transform.forward);
        camForward = Vector3.ProjectOnPlane(camForward, gravityUp).normalized;
        if (camForward.sqrMagnitude < 0.0001f)
            camForward = Vector3.ProjectOnPlane(transform.forward, gravityUp).normalized;

        Vector3 camRight = Vector3.Cross(gravityUp, camForward).normalized;

        Vector3 moveDir = (camForward * -forwardInput + camRight * -sideInput);
        if (moveDir.sqrMagnitude < 0.001f) return;
        moveDir.Normalize();

        // Rotate this visual object to face movement direction
        Quaternion targetRot = Quaternion.LookRotation(moveDir, gravityUp);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }
}
