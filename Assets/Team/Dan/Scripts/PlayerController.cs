using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Input Actions")]
    [SerializeField] InputAction moveInput;
    [SerializeField] InputAction jumpInput;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float groundAcceleration = 18f;
    [SerializeField] private float airAcceleration = 6f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.6f;
    [SerializeField] private float gravity = -24f;
    [SerializeField] private float groundedStickForce = -2f;

    private CharacterController _controller;
    private Vector3 _horizontalVelocity;
    private float _verticalVelocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        moveInput.Enable();
        jumpInput.Enable();
    }

    private void OnDisable()
    {
        moveInput.Disable();
        jumpInput.Disable();
    }

    private void Update()
    {
        Vector2 rawMove = moveInput.ReadValue<Vector2>();

        if (rawMove.sqrMagnitude > 1f)
        {
            rawMove.Normalize();
        }

        bool jumpPressed = jumpInput.WasPressedThisFrame();

        MoveCharacter(rawMove, jumpPressed);
    }

    private void MoveCharacter(Vector2 input, bool jumpPressed)
    {
        bool isGrounded = _controller.isGrounded;

        if (isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = groundedStickForce;
        }

        Vector3 camForward = Vector3.forward;
        Vector3 camRight = Vector3.right;

        if (cameraTransform != null)
        {
            camForward = cameraTransform.forward;
            camForward.y = 0f;
            if (camForward.sqrMagnitude > 0.0001f)
            {
                camForward.Normalize();
            }
            else
            {
                camForward = Vector3.forward;
            }

            camRight = Vector3.Cross(Vector3.up, camForward);
        }

        Vector3 desiredVelocityDir = camRight * input.x + camForward * input.y;
        Vector3 targetHorizontalVel = desiredVelocityDir * moveSpeed;

        float accel = isGrounded ? groundAcceleration : airAcceleration;
        _horizontalVelocity = Vector3.MoveTowards(
            _horizontalVelocity,
            targetHorizontalVel,
            accel * Time.deltaTime
        );

        if (desiredVelocityDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredVelocityDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        if (jumpPressed && isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        _verticalVelocity += gravity * Time.deltaTime;

        Vector3 totalDisplacement = (_horizontalVelocity + Vector3.up * _verticalVelocity) * Time.deltaTime;
        _controller.Move(totalDisplacement);
    }
}