using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed;
    public float mouseSensitivity;

    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform; 
    [SerializeField] private ImputReader inputReader;

    [Header("Animaciones")]
    [SerializeField] private Animations animations;

    [Header("Components")]
    private Rigidbody myRBD;
    private Vector2 movement;
    private Vector2 lookInput;
    private float xRotation;
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
        myRBD = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void FixedUpdate()
    {
        ApplyPhysics();
    }
    private void Update()
    {
        HandleLookPlayer();
    }
    private void OnMovement(Vector2 movementInput)
    {
        if (canMove)
        {
            movement = movementInput;
            animations.UpdateMovementAnimation(movement);
        }
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
        Vector3 moveDirection = (transform.right * movement.x + transform.forward * movement.y).normalized;
        Vector3 velocity = moveDirection * walkSpeed;
        velocity.y = myRBD.linearVelocity.y; 
        myRBD.linearVelocity = velocity;
    }
}

