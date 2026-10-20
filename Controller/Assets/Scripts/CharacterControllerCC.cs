using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerCC : MonoBehaviour
{
    private CharacterController cc;
    private Camera mcam;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;

    private InputAction moveAction, runAction, jumpAction, lookAction;

    private float currentMoveSpeed, verticalSpeed;
    [SerializeField] private float lookAngle, lookSensivity, lookLimit;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentMoveSpeed = walkSpeed;
        cc = GetComponent<CharacterController>();
        mcam = GetComponentInChildren<Camera>();

        moveAction = InputSystem.actions.FindAction("Move");
        runAction = InputSystem.actions.FindAction("Run");
        jumpAction = InputSystem.actions.FindAction("Jump");
        lookAction = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
        Vector2 moveVector = moveAction.ReadValue<Vector2>();
        currentMoveSpeed = runAction.IsPressed() ? runSpeed : walkSpeed;
        HandleMovement(moveVector);

        Vector2 lookVector = lookAction.ReadValue<Vector2>();
        HandleCamera(lookVector);
    }

    private void HandleCamera(Vector2 lookVector)
    {
        lookAngle += -lookVector.y * lookSensivity;
        lookAngle = Mathf.Clamp(lookAngle, -lookLimit, lookLimit);
        mcam.transform.localRotation = Quaternion.Euler(lookAngle, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookVector.x * lookSensivity, 0);
    }

    private void HandleMovement(Vector2 moveVector)
    {
        Vector3 moveDirection = (transform.forward * moveVector.y + transform.right * moveVector.x) * currentMoveSpeed;

        if(cc.isGrounded)
        {
            if(jumpAction.WasPressedThisFrame())
                verticalSpeed = jumpForce;
            else
                verticalSpeed = -2f;
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }

        moveDirection.y = verticalSpeed;
        cc.Move(moveDirection * Time.deltaTime);
    }
}
