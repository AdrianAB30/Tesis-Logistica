using UnityEngine;

public class DeliverableItem : MonoBehaviour, IInteractable
{
    [Header("Identidad del Item")]
    public ItemData itemData;

    [Header("Corrección Visual (En Reposo)")]
    public Vector3 rotacionReposo = Vector3.zero;
    public Vector3 offsetReposo = Vector3.zero;

    [Header("Corrección Visual (En Manos)")]
    public Vector3 rotacionEnManos = Vector3.zero;
    public Vector3 offsetEnManos = Vector3.zero;

    public enum BoxState { InWarehouse, InHands, OnDropTable }

    public BoxState currentState = BoxState.InWarehouse;

    public bool isPacked = false;
    public bool isStored = false;
    public bool isLabeled = false;
    public bool isDelivered = false;

    private void Awake()
    {
        isStored = false;
        isPacked = false;
        isLabeled = false;
        isDelivered = false;
    }

    public void ChangeBoxState(BoxState nuevoEstado)
    {
        currentState = nuevoEstado;
    }

    public void SetPackedState(bool state)
    {
        isPacked = state;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (interactor.objectHeld == null && currentState != BoxState.InHands)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f);
            foreach (Collider col in colliders)
            {
                DropZoneHandler zona = col.GetComponent<DropZoneHandler>();
                if (zona != null && zona.isOccupied)
                {
                    zona.FreeSlot(); 
                }
            }

            transform.SetParent(null);

            interactor.PickUpDeliverable(this);
        }
    }
}