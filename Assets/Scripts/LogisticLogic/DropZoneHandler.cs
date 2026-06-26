using UnityEngine;
using System;

public class DropZoneHandler : MonoBehaviour, IDropZone
{
    public enum ZoneRole { Estante_Picking, Mesa_Empaque, Mesa_Etiquetado, Mesa_Despacho }

    [Header("Rol Logístico de la Zona")]
    public ZoneRole zoneRole;

    public Transform DropTransform => transform;

    [Header("Estado Interno")]
    public bool isOccupied = false;

    [Header("Referencias")]
    [SerializeField] private GameObject ghostObject;
    [SerializeField] private GameEvents gameEvents;

    public event Action<DeliverableItem> OnItemDroppedHere;

    private Mesh mallaPorDefecto;
    private Vector3 escalaPorDefecto;

    private void Start()
    {
        if (ghostObject == null) Debug.LogError("Falta Ghost en " + gameObject.name);
        else
        {
            MeshFilter mf = ghostObject.GetComponent<MeshFilter>();
            if (mf != null) mallaPorDefecto = mf.sharedMesh;
            escalaPorDefecto = ghostObject.transform.localScale;
        }

        if (GetComponentInChildren<DeliverableItem>() != null)
        {
            isOccupied = true;
            Debug.Log($"[LOG] {gameObject.name} detectó caja hija, ocupado=true");
        }
        else
        {
            isOccupied = false;
        }

        HideGhost();
    }

    public void ShowGhost()
    {
        ShowGhost(null);
    }

    public void ShowGhost(DeliverableItem heldItem)
    {
        if (ghostObject != null)
        {
            ghostObject.SetActive(true);

            ghostObject.transform.localScale = escalaPorDefecto;
            ghostObject.transform.localRotation = Quaternion.identity;

            MeshFilter ghostMesh = ghostObject.GetComponent<MeshFilter>();
            if (ghostMesh != null)
            {
                if (heldItem != null && !heldItem.isPacked && heldItem.GetComponent<BoxLogic>() == null)
                {
                    MeshFilter itemMesh = heldItem.GetComponentInChildren<MeshFilter>();
                    if (itemMesh != null)
                    {
                        ghostMesh.sharedMesh = itemMesh.sharedMesh;
                        ghostObject.transform.localScale = itemMesh.transform.localScale;
                        ghostObject.transform.localRotation = Quaternion.identity;
                    }
                }
                else
                {
                    ghostMesh.sharedMesh = mallaPorDefecto;
                    ghostObject.transform.localScale = escalaPorDefecto;
                    ghostObject.transform.localRotation = Quaternion.identity;
                }
            }
        }
    }

    public void HideGhost()
    {
        if (ghostObject != null) ghostObject.SetActive(false);
    }

    public bool CanDropItem(DeliverableItem item)
    {
        if (isOccupied) return false;
        if (item == null) return false;

        if (zoneRole == ZoneRole.Mesa_Empaque)
        {
            return !item.isPacked && item.isStored;
        }
        else if (zoneRole == ZoneRole.Mesa_Etiquetado) 
        {
            return item.isPacked && !item.isLabeled;
        }
        else if (zoneRole == ZoneRole.Mesa_Despacho) 
        {
            return item.isPacked && item.isLabeled;
        }
        else if (zoneRole == ZoneRole.Estante_Picking)
        {
            return true;
        }

        return true;
    }

    public void ReceiveItem(DeliverableItem item)
    {
        isOccupied = true;
        OnItemDroppedHere?.Invoke(item);

        if (zoneRole == ZoneRole.Estante_Picking)
        {
            item.isStored = true;
            Debug.Log($"[LOGÍSTICA] {item.itemData.itemName} registrado en inventario.");
        }
    }


    public void FreeSlot()
    {
        isOccupied = false;
    }

    public void SetGhost(GameObject newGhost)
    {
        ghostObject = newGhost;

        MeshFilter mf = ghostObject.GetComponent<MeshFilter>();
        if (mf != null) mallaPorDefecto = mf.sharedMesh;
        escalaPorDefecto = ghostObject.transform.localScale;

        HideGhost();
    }

    public void SetGameEvents(GameEvents events)
    {
        gameEvents = events;
    }
    private void OnTriggerExit(Collider other)
    {
        if (isOccupied)
        {
            DeliverableItem itemSaliente = other.GetComponent<DeliverableItem>();

            if (itemSaliente != null)
            {
                FreeSlot();
                Debug.Log($"[LOGÍSTICA] Slot {gameObject.name} liberado automáticamente.");
            }
        }
    }
}