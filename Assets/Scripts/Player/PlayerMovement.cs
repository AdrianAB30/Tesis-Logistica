using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento y Vista")]
    public float walkSpeed = 5f;
    public float mouseSensitivity = 100f;

    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputReader inputReader;

    [Header("Game Feel - Head Bobbing")]
    public float bobSpeed = 14f;
    public float bobAmount = 0.05f;
    private float defaultCameraY = 0f;
    private float bobTimer = 0f;

    private Rigidbody myRBD;
    private Vector2 movement;
    private Vector2 lookInput;
    private float xRotation;

    private void Awake()
    {
        myRBD = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputReader.OnMovementInput += OnMovement;
        inputReader.OnLookInput += OnLook;
    }

    private void OnDisable()
    {
        inputReader.OnMovementInput -= OnMovement;
        inputReader.OnLookInput -= OnLook;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform != null)
            defaultCameraY = cameraTransform.localPosition.y;
    }

    private void FixedUpdate()
    {
        ApplyPhysics();
    }

    private void Update()
    {
        HandleLookPlayer();
        HandleHeadBob(); 
    }

    private void HandleHeadBob()
    {
        if (cameraTransform == null) return;

        if (Mathf.Abs(movement.x) > 0.1f || Mathf.Abs(movement.y) > 0.1f)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                defaultCameraY + Mathf.Sin(bobTimer) * bobAmount,
                cameraTransform.localPosition.z
            );
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                Mathf.Lerp(cameraTransform.localPosition.y, defaultCameraY, Time.deltaTime * bobSpeed),
                cameraTransform.localPosition.z
            );
        }
    }
    private void OnMovement(Vector2 movementInput)
    {
        movement = movementInput;
    }

    private void OnLook(Vector2 inputLook)
    {
        lookInput = inputLook;
    }

    private void HandleLookPlayer()
    {
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity * Time.deltaTime);

        xRotation -= lookInput.y * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void ApplyPhysics()
    {
        Vector3 moveDirection =
            (transform.right * movement.x + transform.forward * movement.y).normalized;

        Vector3 velocity = moveDirection * walkSpeed;
        velocity.y = myRBD.linearVelocity.y;

        myRBD.linearVelocity = velocity;
    }   
}