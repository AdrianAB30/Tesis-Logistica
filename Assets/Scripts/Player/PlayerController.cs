using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento e Interaccion")]
    public float walkSpeed;
    public float mouseSensitivity;
    private GameObject objectHeld = null;
    public float interactRange = 2f;
    public LayerMask interactableLayer;
    private float pickupCooldown = 1f;
    private float lastDropTime = -1f;
    [SerializeField] private Transform eyes;
    [SerializeField] private Transform holdPosition;

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

    // --- Referencias Modulares ---
    // Resaltado/Outline
    private IHighlightable currentHighlightable = null;
    // Zonas de Drop/Fantasma
    private IDropZone currentDropZone = null;
    // -----------------------------

    [Header("Game Events")]
    [SerializeField] private GameEvents gameEvents;

    private void OnEnable()
    {
        inputReader.OnMovementInput += OnMovement;
        inputReader.OnLookInput += OnLook;
        inputReader.OnInteractInput += OnInteract;
    }
    private void OnDisable()
    {
        inputReader.OnMovementInput -= OnMovement;
        inputReader.OnLookInput -= OnLook;
        inputReader.OnInteractInput -= OnInteract;
    }

    private void Awake()
    {
        myRBD = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetCanMove(false);
    }
    private void FixedUpdate()
    {
        ApplyPhysics();
    }
    private void Update()
    {
        HandleLookPlayer();
        HandleTargetDetection();
    }

    private void HandleTargetDetection()
    {
        Ray ray = new Ray(eyes.position, eyes.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            ClearTargetDetection();
            return;
        }

        if (objectHeld != null)
        {
            IHighlightable potentialHighlightable = hit.collider.GetComponent<IHighlightable>();
            if (potentialHighlightable != null && potentialHighlightable != currentHighlightable)
            {
                potentialHighlightable.Unhighlight();
            }

            IDropZone newDropZone = hit.collider.GetComponent<IDropZone>();

            if (newDropZone != null)
            {
                string heldItemTag = objectHeld.tag;

                if (newDropZone.CanDropItem(heldItemTag))
                {
                    if (currentDropZone != newDropZone)
                    {
                        if (currentDropZone != null) currentDropZone.HideGhost();
                        currentDropZone = newDropZone;
                        currentDropZone.ShowGhost();
                    }
                    return;
                }
            }

            if (currentDropZone != null)
            {
                currentDropZone.HideGhost();
                currentDropZone = null;
            }
        }
        else 
        {
            IHighlightable newHighlightable = hit.collider.GetComponent<IHighlightable>();

            if (newHighlightable != null)
            {
                if (newHighlightable != currentHighlightable)
                {
                    if (currentHighlightable != null) currentHighlightable.Unhighlight();
                    currentHighlightable = newHighlightable;
                    currentHighlightable.Highlight();
                }
            }
            else 
            {
                if (currentHighlightable != null) currentHighlightable.Unhighlight();
                currentHighlightable = null;
            }
        }
    }

    private void ClearTargetDetection()
    {
        if (currentDropZone != null)
        {
            currentDropZone.HideGhost();
            currentDropZone = null;
        }
        if (currentHighlightable != null)
        {
            currentHighlightable.Unhighlight();
            currentHighlightable = null;
        }
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

    private void OnInteract(bool isPressed)
    {
        animations.SetInteract(isPressed);

        if (!isPressed) return;

        if (Time.time - lastDropTime < pickupCooldown) return;

        if (objectHeld == null)
        {
            Ray ray = new Ray(eyes.position, eyes.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
            {
                if (hit.collider.GetComponent<DeliverableItem>() == null) return;

                objectHeld = hit.collider.gameObject;

                objectHeld.GetComponent<DeliverableItem>().ItemPicked();

                objectHeld.GetComponent<Rigidbody>().isKinematic = true;
                objectHeld.transform.SetParent(holdPosition);
                objectHeld.transform.localPosition = Vector3.zero;
                objectHeld.transform.localRotation = Quaternion.identity;

                ClearTargetDetection();
            }
        }
        else
        {
            if (currentDropZone != null)
            {
                currentDropZone.HideGhost();

                objectHeld.GetComponent<Rigidbody>().isKinematic = true;
                objectHeld.transform.SetParent(currentDropZone.DropTransform);
                objectHeld.transform.position = currentDropZone.DropTransform.position;
                objectHeld.transform.rotation = currentDropZone.DropTransform.rotation;

                objectHeld = null;
                currentDropZone = null;
                lastDropTime = Time.time;
                return;
            }

            objectHeld.GetComponent<Rigidbody>().isKinematic = false;
            objectHeld.transform.SetParent(null);
            objectHeld = null;

            lastDropTime = Time.time;
        }
    }

    public void SetCanMove(bool state)
    {
        canMove = state;
        movement = Vector2.zero;
    }
    private void OnDrawGizmos()
    {
        if (eyes != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(eyes.position, eyes.forward * interactRange);
        }
    }
}