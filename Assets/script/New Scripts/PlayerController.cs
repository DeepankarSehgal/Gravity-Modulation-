using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCOntroller : MonoBehaviour
{
    [Header("Pause / Win UI")]
    [SerializeField] private GameObject PauseMenu;

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform; // assign Main Camera here

    [Header("Animations and game object")]
    public Animator Characters;
    public GameObject shadow;
    public GameObject playCameraAnimation;

    [Header("Local state")]
    private bool a = true;
    private bool shouldRotate = false;

    // arrow / scroll flags
    private bool isUpArrowPressed = false;
    private bool isDownArrowPressed = false;
    private bool isRightArrowPressed = false;
    private bool isLeftArrowPressed = false;
    private bool isScrollUp = false;
    private bool isScrollDown = false;

    // gravity interpolation
    private Vector3 currentGravity;
    private Vector3 targetGravity;
    private Quaternion targetRotation;

    private Rigidbody rb;

    private void PlayerCamAnim()
    {
        if (playCameraAnimation != null)
            playCameraAnimation.SetActive(true);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Initial gravity
        Physics.gravity = new Vector3(0, -9.8f, 0);
        currentGravity = Physics.gravity;
        targetGravity = currentGravity;

        if (Characters == null)
        {
            Characters = GetComponent<Animator>();
        }

        Invoke(nameof(PlayerCamAnim), 1.45f);
    }

    private void FixedUpdate()
    {
        HandleMovement();

        // Out-of-bounds check (same as before)
        if (transform.position.x > 5 || transform.position.x < -5 ||
            transform.position.y > 20 || transform.position.y < -16 ||
            transform.position.z > 25 || transform.position.z < -5)
        {
            Debug.Log("Out of bound");
            if (PauseMenu != null)
            {
                PauseMenu.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    private void Update()
    {
        // Movement animation (same as before)
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
                        Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        Characters.SetBool("NotIdeal", isMoving);

        // Gravity preview / confirm
        UpKey();
        EnterKeyUp();

        DownKey();
        EnterKeyDown();

        RightKey();
        EnterKeyRight();

        LeftKey();
        EnterKeyLeft();

        ScrollGravity();

        // Smooth gravity rotation if needed
        if (shouldRotate)
        {
            RotateTowardsTargetGravity();
        }
    }

    private void HandleMovement()
    {
        float forwardInput = Input.GetAxis("Vertical");   // W/S
        float sideInput = Input.GetAxis("Horizontal");    // A/D

        if (Mathf.Abs(forwardInput) < 0.001f && Mathf.Abs(sideInput) < 0.001f)
            return;

        Vector3 gravityDown = GetGravityDown();
        Vector3 gravityUp = -gravityDown;

        // camera-relative movement on the current surface
        Vector3 camForward = (cameraTransform != null ? cameraTransform.forward : transform.forward);
        camForward = Vector3.ProjectOnPlane(camForward, gravityUp).normalized;

        if (camForward.sqrMagnitude < 0.0001f)
        {
            camForward = Vector3.ProjectOnPlane(transform.forward, gravityUp).normalized;
        }

        Vector3 camRight = Vector3.Cross(gravityUp, camForward).normalized;

        Vector3 moveDir = (camForward * -forwardInput + camRight * -sideInput);
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        // move in world space so it stays camera-relative
        transform.position += moveDir * moveSpeed * Time.fixedDeltaTime;
    }

    private Vector3 GetGravityDown()
    {
        if (Physics.gravity.sqrMagnitude < 0.0001f)
            return Vector3.down;
        return Physics.gravity.normalized;
    }

    /// <summary>
    /// Starts a gravity change towards the given "down" direction.
    /// </summary>
    private void StartGravityChange(Vector3 newDownDir)
    {
        if (newDownDir.sqrMagnitude < 0.0001f)
            return;

        newDownDir = newDownDir.normalized;

        Vector3 oldDown = currentGravity.normalized;
        Vector3 oldUp = -oldDown;
        Vector3 newUp = -newDownDir;

        // rotation that aligns oldUp → newUp
        Quaternion fromTo = Quaternion.FromToRotation(oldUp, newUp);
        targetRotation = fromTo * transform.rotation;

        // store final gravity vector
        targetGravity = newDownDir * 9.8f;

        // stop gravity temporarily during rotation
        Physics.gravity = Vector3.zero;
        shouldRotate = true;

        // turn off shadow
        if (shadow != null)
            shadow.SetActive(false);
    }

    private void RotateTowardsTargetGravity()
    {
        // Smooth rotate towards targetRotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4f);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
        {
            transform.rotation = targetRotation;

            // apply new gravity
            Physics.gravity = targetGravity;
            currentGravity = targetGravity;

            shouldRotate = false;

            // small impulse away from surface
            if (rb != null)
            {
                Vector3 jumpDir = -currentGravity.normalized;
                rb.AddForce(jumpDir * 0.1f, ForceMode.Impulse);
            }
        }
    }

    // Helper to get "surface" basis using camera + gravity
    private void GetSurfaceBasis(out Vector3 up, out Vector3 forward, out Vector3 right)
    {
        Vector3 gravityDown = GetGravityDown();
        up = -gravityDown;

        Vector3 camForward = (cameraTransform != null ? cameraTransform.forward : transform.forward);
        forward = Vector3.ProjectOnPlane(camForward, up).normalized;
        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.ProjectOnPlane(transform.forward, up).normalized;

        right = Vector3.Cross(up, forward).normalized;
    }

    private void SetShadowPreview(Vector3 newDownDir)
    {
        if (shadow == null) return;

        shadow.SetActive(true);

        Vector3 newUp = -newDownDir;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, newUp).normalized;
        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.ProjectOnPlane(Vector3.forward, newUp).normalized;

        shadow.transform.rotation = Quaternion.LookRotation(forward, newUp);
    }

    // Up Arrow (gravity "in front" of camera/player)
    private void UpKey()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GetSurfaceBasis(out var up, out var forward, out var right);
            Vector3 newDown = forward; // fall "forward" relative to camera/player
            isUpArrowPressed = true;
            SetShadowPreview(newDown);
        }
    }

    private void EnterKeyUp()
    {
        if (isUpArrowPressed && Input.GetKeyDown(KeyCode.Return))
        {
            GetSurfaceBasis(out var up, out var forward, out var right);
            Vector3 newDown = forward;
            isUpArrowPressed = false;
            StartGravityChange(newDown);
        }
    }

    //  Down Arrow (gravity "behind" camera/player)   
    private void DownKey()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            GetSurfaceBasis(out var up, out var forward, out var right);
            Vector3 newDown = -forward;
            isDownArrowPressed = true;
            SetShadowPreview(newDown);
        }
    }

    private void EnterKeyDown()
    {
        if (isDownArrowPressed && Input.GetKeyDown(KeyCode.Return))
        {
            GetSurfaceBasis(out var up, out var forward, out var right);
            Vector3 newDown = -forward;
            isDownArrowPressed = false;
            StartGravityChange(newDown);
        }
    }

    //  Right Arrow (gravity to camera/player right) 
    private void RightKey()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GetSurfaceBasis(out var up, out var forward, out var right);
            Vector3 newDown = right;
            isRightArrowPressed = true;
            SetShadowPreview(newDown);
        }
    }

    private void EnterKeyRight()
    {
        if (isRightArrowPressed && Input.GetKeyDown(KeyCode.Return))
        {
            GetSurfaceBasis(out var up, out var forward, out var right);
            Vector3 newDown = right;
            isRightArrowPressed = false;
            StartGravityChange(newDown);
        }
    }

    //  Left Arrow (gravity to camera/player left) 
    private void LeftKey()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GetSurfaceBasis(out var up, out var forward, out var right);
            Vector3 newDown = -right;
            isLeftArrowPressed = true;
            SetShadowPreview(newDown);
        }
    }

    private void EnterKeyLeft()
    {
        if (isLeftArrowPressed && Input.GetKeyDown(KeyCode.Return))
        {
            GetSurfaceBasis(out var up, out var forward, out var right);
            Vector3 newDown = -right;
            isLeftArrowPressed = false;
            StartGravityChange(newDown);
        }
    }

    //  Scroll wheel (gravity to ceiling / back to floor) 
    private void ScrollGravity()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 gravityDown = GetGravityDown();
        Vector3 gravityUp = -gravityDown;

        if (scroll > 0f)
        {
            // go to ceiling
            isScrollUp = true;
            isScrollDown = false;
            SetShadowPreview(gravityUp); // down = up → ceiling
        }
        else if (scroll < 0f)
        {
            // go to floor
            isScrollDown = true;
            isScrollUp = false;
            SetShadowPreview(gravityDown);
        }

        if (isScrollUp && Input.GetKeyDown(KeyCode.Return))
        {
            isScrollUp = false;
            StartGravityChange(gravityUp);
        }
        else if (isScrollDown && Input.GetKeyDown(KeyCode.Return))
        {
            isScrollDown = false;
            StartGravityChange(gravityDown);
        }
    }

    // COLLISIONS / COLLECTIBLES

    private void OnCollisionEnter(Collision Other)
    {
        if (Other.gameObject.CompareTag("TouchedGround"))
        {
            Debug.Log(" touching ground");
            Characters.SetBool("GroundTouched", a);
        }

        if (Other.gameObject.CompareTag("Collectabe"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCollectiblePicked(Other.gameObject);
            }
            else
            {
                // fallback if GameManager not set
                Destroy(Other.gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("TouchedGround"))
        {
            Characters.SetBool("GroundTouched", false);
            Debug.Log("Left ground");
            Characters.SetTrigger("InAir"); // Play jump/floating animation
        }
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
