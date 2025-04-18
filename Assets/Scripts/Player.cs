using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed;
    public float mouseSensitivity;

    [Header("Head Bobbing")]
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;

    [Header("Referencias")]
    [SerializeField] private Transform cameraHolder; 
    [SerializeField] private Transform cameraTransform; 
    [SerializeField] private ImputReader inputReader;

    [Header("Components")]
    private Rigidbody rb;
    private Vector2 movement;
    private Vector2 lookInput;
    private float xRotation;
    private float bobTimer;
    private float defaultCamY;
    public bool canMove = true;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultCamY = cameraTransform.localPosition.y;
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void Update()
    {
        HandleMouseLook();
        HandleHeadBobbing();
    }

    private void OnMovement(Vector2 input)
    {
        movement = input;
    }

    private void OnLook(Vector2 input)
    {
        lookInput = input;
    }

    private void MovePlayer()
    {
        if (!canMove) return;

        Vector3 moveDirection = transform.right * movement.x + transform.forward * movement.y;
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    private void HandleMouseLook()
    {
        if (!canMove) return;

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotar todo el cameraHolder en X e Y
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        cameraHolder.Rotate(Vector3.up * mouseX, Space.World);
    }

    private void HandleHeadBobbing()
    {
        if (movement.magnitude > 0.1f && rb.linearVelocity.magnitude > 0.1f)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmount;
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                defaultCamY + bobOffset,
                cameraTransform.localPosition.z
            );
        }
        else
        {
            bobTimer = 0f;
            Vector3 camPos = cameraTransform.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, defaultCamY, Time.deltaTime * bobSpeed);
            cameraTransform.localPosition = camPos;
        }
    }
}

