using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraRig : MonoBehaviour
{
    [Header("Target & Framing")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private float distance = 5f;
    [SerializeField] private float defaultPitch = 15f;

    [Header("Input Actions")]
    [SerializeField] private InputAction cameraPan;

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivityX = 0.14f;
    [SerializeField] private float mouseSensitivityY = 0.14f;
    [SerializeField] private float gamepadSensitivityX = 160f;
    [SerializeField] private float gamepadSensitivityY = 120f;
    [SerializeField] private bool invertY = false;

    [Header("Controller Stick")]
    [SerializeField] private float stickDeadzone = 0.15f;

    [Header("Pitch Limits & Smoothing")]
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField] private float orbitSmoothSpeed = 20f;

    [Header("Target Follow")]
    [SerializeField] private float followPositionSmooth = 10f;

    [Header("Cursor")]
    [SerializeField] private bool lockCursorOnEnable = true;

    [Header("Obstacle Avoidance")]
    [SerializeField] private LayerMask collisionMask = ~0;
    [SerializeField] private float cameraRadius = 0.25f;
    [SerializeField] private float collisionBuffer = 0.15f;

    private float _yaw;
    private float _pitch;
    private float _currentYaw;
    private float _currentPitch;
    private Vector3 _currentPivot;

    private void Start()
    {
        if (target != null)
        {
            _currentPivot = target.position + pivotOffset;
            _yaw = target.eulerAngles.y;
        }
        else
        {
            _currentPivot = transform.position;
            _yaw = transform.eulerAngles.y;
        }

        _pitch = defaultPitch;
        _currentYaw = _yaw;
        _currentPitch = _pitch;
    }

    private void OnEnable()
    {
        cameraPan.Enable();
        if (lockCursorOnEnable)
        {
            SetCursorLocked(true);
        }
    }

    private void OnDisable()
    {
        cameraPan.Disable();
        SetCursorLocked(false);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector2 lookDelta = cameraPan.ReadValue<Vector2>();
        bool isPointerInput = cameraPan.activeControl != null && cameraPan.activeControl.device is Pointer;

        UpdateFreelookAngles(lookDelta, isPointerInput);

        _currentYaw = Mathf.LerpAngle(_currentYaw, _yaw, orbitSmoothSpeed * Time.deltaTime);
        _currentPitch = Mathf.Lerp(_currentPitch, _pitch, orbitSmoothSpeed * Time.deltaTime);

        Vector3 desiredPivot = target.position + pivotOffset;
        _currentPivot = Vector3.Lerp(_currentPivot, desiredPivot, followPositionSmooth * Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        Vector3 desiredPosition = _currentPivot - (rotation * Vector3.forward * distance);
        Vector3 finalPosition = ResolveObstacles(_currentPivot, desiredPosition);

        transform.SetPositionAndRotation(finalPosition, rotation);
    }

    private void UpdateFreelookAngles(Vector2 lookDelta, bool isPointerInput)
    {
        float ySign = invertY ? 1f : -1f;

        if (isPointerInput)
        {
            _yaw += lookDelta.x * mouseSensitivityX;
            _pitch += lookDelta.y * mouseSensitivityY * ySign;
        }
        else
        {
            Vector2 scaledStick = ApplyRadialDeadzone(lookDelta, stickDeadzone);
            _yaw += scaledStick.x * gamepadSensitivityX * Time.deltaTime;
            _pitch += scaledStick.y * gamepadSensitivityY * ySign * Time.deltaTime;
        }

        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
    }

    private static Vector2 ApplyRadialDeadzone(Vector2 input, float deadzone)
    {
        float mag = input.magnitude;
        if (mag <= deadzone) return Vector2.zero;

        float rescaled = Mathf.Clamp01((mag - deadzone) / (1f - deadzone));
        return input.normalized * rescaled;
    }

    private Vector3 ResolveObstacles(Vector3 pivot, Vector3 desiredPos)
    {
        Vector3 direction = desiredPos - pivot;
        float maxDist = direction.magnitude;
        if (maxDist <= 0.001f) return desiredPos;

        direction /= maxDist;

        if (Physics.SphereCast(
                pivot,
                cameraRadius,
                direction,
                out RaycastHit hit,
                maxDist,
                collisionMask,
                QueryTriggerInteraction.Ignore))
        {
            float safeDist = Mathf.Max(0.2f, hit.distance - collisionBuffer);
            return pivot + direction * safeDist;
        }

        return desiredPos;
    }

    private void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}