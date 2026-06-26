using DG.Tweening; 
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interacción")]
    public float interactRange = 2f;
    public LayerMask interactableLayer;
    private float pickupCooldown = 0.5f; 
    private float lastDropTime = -1f;

    [Header("Posiciones")]
    [SerializeField] private Transform eyes;
    [SerializeField] private Transform holdPosition;

    [Header("Referencias")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private DOTweenConfig dtAnims;

    [Header("UI de Mira")]
    [SerializeField] private UnityEngine.UI.Image crosshairImage;
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorInteraccion = Color.green;

    public GameObject objectHeld = null;
    private IHighlightable currentHighlightable = null;

    private LogisticsZone zonaActual = null;

    private void OnEnable()
    {
        inputReader.OnInteractInput += OnInteract;
    }

    private void OnDisable()
    {
        inputReader.OnInteractInput -= OnInteract;
    }

    private void Update()
    {
        HandleTargetDetection();
    }

    private void HandleTargetDetection()
    {
        Ray ray = new Ray(eyes.position, eyes.forward);
        bool detectado = false;

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
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
                detectado = true;
            }
            else if (currentHighlightable != null)
            {
                currentHighlightable.Unhighlight();
                currentHighlightable = null;
            }

            if (hit.collider.GetComponent<IInteractable>() != null || hit.collider.GetComponent<DeliverableItem>() != null)
            {
                detectado = true;
            }
        }
        else if (currentHighlightable != null)
        {
            currentHighlightable.Unhighlight();
            currentHighlightable = null;
        }

        if (crosshairImage != null)
            crosshairImage.color = detectado ? colorInteraccion : colorNormal;
    }

    private void OnInteract(bool isPressed)
    {
        if (!isPressed) return;
        if (Time.time - lastDropTime < pickupCooldown) return;

        Ray ray = new Ray(eyes.position, eyes.forward);

        if (objectHeld == null)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, interactRange, interactableLayer, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                DeliverableItem item = GetDeliverableFromCollider(hit.collider);
                if (item != null && item.enabled && item.isPacked) 
                {
                    PickUpDeliverable(item);
                    return;
                }
            }

            foreach (var hit in hits)
            {
                IInteractable[] interactables = hit.collider.GetComponentsInParent<IInteractable>();
                foreach (var interactable in interactables)
                {
                    if (interactable is MonoBehaviour mb && mb.enabled)
                    {
                        interactable.Interact(this);
                        return; 
                    }
                }
            }

            foreach (var hit in hits)
            {
                DeliverableItem item = GetDeliverableFromCollider(hit.collider);
                if (item != null && item.enabled && !item.isPacked)
                {
                    PickUpDeliverable(item);
                    return;
                }
            }

            Debug.Log("[INTERACCIÓN] No se pudo recoger nada ni interactuar.");
        }
        else
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, interactRange, interactableLayer, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                GarageDoorController botonGaraje = hit.collider.GetComponent<GarageDoorController>();
                if (botonGaraje == null) botonGaraje = hit.collider.GetComponentInParent<GarageDoorController>();

                if (botonGaraje != null && botonGaraje.enabled)
                {
                    botonGaraje.Interact(this);
                    return;
                }

                TruckDispatchManager camionDespacho = hit.collider.GetComponent<TruckDispatchManager>();
                if (camionDespacho == null) camionDespacho = hit.collider.GetComponentInParent<TruckDispatchManager>();

                if (camionDespacho != null && camionDespacho.enabled)
                {
                    camionDespacho.Interact(this);
                    return;
                }
            }
            TryDropItem();
        }
    }
    public void PickUpDeliverable(DeliverableItem deliverableItem)
    {
        deliverableItem.transform.DOKill();

        DropZoneHandler zonaPadre = deliverableItem.GetComponentInParent<DropZoneHandler>();
        if (zonaPadre != null) zonaPadre.FreeSlot();

        objectHeld = deliverableItem.gameObject;
        deliverableItem.ChangeBoxState(DeliverableItem.BoxState.InHands);

        Rigidbody rb = objectHeld.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        Collider[] todosLosColliders = objectHeld.GetComponentsInChildren<Collider>();
        foreach (Collider col in todosLosColliders)
        {
            col.enabled = false; 
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        objectHeld.transform.SetParent(holdPosition);
        objectHeld.transform.DOLocalMove(deliverableItem.offsetEnManos, dtAnims.objectMoveTime);
        objectHeld.transform.DOLocalRotate(deliverableItem.rotacionEnManos, dtAnims.objectRotateTime);

        if (zonaActual != null)
        {
            zonaActual.ShowAvailableGhosts(objectHeld.GetComponent<DeliverableItem>());
        }
    }

    private void TryDropItem()
    {
        if (zonaActual == null || objectHeld == null) return;

        DeliverableItem itemToDrop = objectHeld.GetComponent<DeliverableItem>();
        DropZoneHandler slotDestino = null;
        Ray ray = new Ray(eyes.position, eyes.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            DropZoneHandler hitSlot = hit.collider.GetComponent<DropZoneHandler>();
            if (hitSlot != null && !hitSlot.isOccupied && hitSlot.CanDropItem(itemToDrop))
            {
                slotDestino = hitSlot;
            }
        }

        if (slotDestino == null)
        {
            DropZoneHandler autoSlot = zonaActual.GetAvailableSlot();
            if (autoSlot != null && autoSlot.CanDropItem(itemToDrop))
            {
                slotDestino = autoSlot;
            }
        }

        if (slotDestino != null)
        {
            ThrowBoxes(objectHeld, slotDestino);
            objectHeld = null;
            lastDropTime = Time.time;
            zonaActual.ShowAvailableGhosts(null);
        }
        else
        {
            Debug.Log("[JUGADOR] Movimiento ilegal. Ese objeto no va en esta mesa aún.");
        }
    }
    private void ThrowBoxes(GameObject box, DropZoneHandler slot)
    {
        slot.isOccupied = true;
        slot.HideGhost();

        DeliverableItem itemInfo = box.GetComponent<DeliverableItem>();
        itemInfo.ChangeBoxState(DeliverableItem.BoxState.OnDropTable);

        box.transform.SetParent(slot.DropTransform);

        box.transform.DOJump(slot.DropTransform.position, dtAnims.objectMovePower, dtAnims.objectNumberMoves, dtAnims.objectMoveTime)
            .SetEase(dtAnims.objectMoveEase)
            .OnComplete(() =>
            {
                Collider[] todosLosColliders = box.GetComponentsInChildren<Collider>();
                foreach (Collider col in todosLosColliders) col.enabled = true;

                Rigidbody rb = box.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                box.transform.localPosition = itemInfo.offsetReposo;
                box.transform.localRotation = Quaternion.Euler(itemInfo.rotacionReposo);
                slot.ReceiveItem(itemInfo);

                BoxLogic logicaCaja = box.GetComponent<BoxLogic>();
                if (logicaCaja != null && slot.zoneRole == DropZoneHandler.ZoneRole.Estante_Picking)
                {
                    logicaCaja.enabled = true;
                    logicaCaja.AbrirCaja();
                }
            });
    }

    private void OnTriggerEnter(Collider other)
    {
        LogisticsZone zonaDetectada = other.GetComponentInParent<LogisticsZone>();

        if (zonaDetectada != null)
        {
            zonaActual = zonaDetectada;

            if (objectHeld != null)
            {
                zonaActual.ShowAvailableGhosts(objectHeld.GetComponent<DeliverableItem>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LogisticsZone zonaDetectada = other.GetComponentInParent<LogisticsZone>();

        if (zonaDetectada != null && zonaDetectada == zonaActual)
        {
            zonaActual.HideGhosts();
            zonaActual = null;
        }
    }
    private void OnDrawGizmos()
    {
        if (eyes != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(eyes.position, eyes.forward * interactRange);
        }
    }
    private DeliverableItem GetDeliverableFromCollider(Collider col)
    {
        DeliverableItem item = col.GetComponent<DeliverableItem>();
        if (item == null) item = col.GetComponentInParent<DeliverableItem>();
        if (item == null) item = col.GetComponentInChildren<DeliverableItem>();
        return item;
    }
    public void ClearHand()
    {
        objectHeld = null;
        if (zonaActual != null) zonaActual.ShowAvailableGhosts(null);
    }
}