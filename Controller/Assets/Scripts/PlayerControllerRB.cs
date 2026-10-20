using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerRB : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;

    private Rigidbody rb;
    private Camera mcam;
    private Vector2 moveVector = Vector2.zero;
    private float currentSpeed = 0.0f;
    private bool jumpRequested;

    [SerializeField] private float groundCheckExtra = 0.1f;
    private CapsuleCollider capsule;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    private InputAction moveAction, runAction, jumpAction, lookAction;
    [SerializeField] private float lookAngle, lookSensivity, lookLimit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mcam = GetComponentInChildren<Camera>();
        rb.freezeRotation = true;

        capsule = GetComponent<CapsuleCollider>();

        moveAction = InputSystem.actions.FindAction("Move");
        runAction = InputSystem.actions.FindAction("Run");
        jumpAction = InputSystem.actions.FindAction("Jump");
        lookAction = InputSystem.actions.FindAction("Look");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = walkSpeed;
    }

    void Update()
    {
        moveVector = moveAction.ReadValue<Vector2>();
        currentSpeed = runAction.IsPressed() ? runSpeed : walkSpeed;
        if(jumpAction.WasPressedThisFrame())
        {
            jumpRequested = true;
        }

        Vector2 lookVector = lookAction.ReadValue<Vector2>();
        HandleCamera(lookVector);
    }

    void FixedUpdate()
    {
        Vector3 move = (transform.forward * moveVector.y + transform.right * moveVector.x) * currentSpeed;
        float verticalSpeed = rb.linearVelocity.y;

        float distance = capsule.height / 2f + groundCheckExtra;
        Vector3 origin = transform.TransformPoint(capsule.center);
        isGrounded = Physics.Raycast(origin, Vector3.down, distance, groundMask);
        if(jumpRequested && isGrounded)
        {
            verticalSpeed = jumpForce;
        }
        jumpRequested = false;

        rb.linearVelocity = new Vector3(move.x, verticalSpeed, move.z);
    }

    private void HandleCamera(Vector2 lookVector)
    {
        lookAngle += -lookVector.y * lookSensivity;
        lookAngle = Mathf.Clamp(lookAngle, -lookLimit, lookLimit);
        mcam.transform.localRotation = Quaternion.Euler(lookAngle, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookVector.x * lookSensivity, 0);
    }
}
