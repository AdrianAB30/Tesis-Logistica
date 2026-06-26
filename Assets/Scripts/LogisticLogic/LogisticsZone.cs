using UnityEngine;
using System.Collections.Generic;

public class LogisticsZone : MonoBehaviour
{
    public enum StationType { Empaque, Despacho, Estante, Etiquetado }

    [Header("Configuración de la Estación")]
    public StationType stationType;

    [Header("Ranuras / Slots Visuales")]
    [SerializeField] private List<DropZoneHandler> dropZones;

    [Header("--- GENERADOR AUTOMÁTICO (Grid) ---")]
    [Tooltip("El modelo semitransparente genérico para el fantasma")]
    [SerializeField] private GameObject prefabFantasmaBase;
    [SerializeField] private DropZoneHandler.ZoneRole rolParaEstosSlots;
    [SerializeField] private Vector3Int dimensionesGrid = new Vector3Int(3, 1, 1); // X: Columnas, Y: Niveles, Z: Profundidad
    [SerializeField] private Vector3 separacion = new Vector3(0.5f, 0.4f, 0f);
    [SerializeField] private Vector3 offsetInicial = Vector3.zero;

    [Header("Eventos de Juego")]
    [SerializeField] private GameEvents gameEvents;

    [Header("Conexión con Mesa")]
    [SerializeField] private PackingTableManager tableManager;
    [SerializeField] private PrinterTableManager printerManager;
    [SerializeField] private TruckDispatchManager dispatchManager;

    private void OnEnable()
    {
        if (gameEvents != null)
        {
            gameEvents.OnOrderReceived += PrepareTableForOrder;
        }

        foreach (var zone in dropZones)
        {
            if (zone != null) zone.OnItemDroppedHere += ProcessBox;
        }
    }

    private void OnDisable()
    {
        if (gameEvents != null)
        {
            gameEvents.OnOrderReceived -= PrepareTableForOrder;
        }

        foreach (var zona in dropZones)
        {
            if (zona != null) zona.OnItemDroppedHere -= ProcessBox;
        }
    }

    [ContextMenu("Auto-Generar Slots")]
    public void GenerarSlotsAutomaticamente()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("SlotAuto_"))
            {
                DestroyImmediate(child.gameObject);
            }
        }
        dropZones.Clear();

        if (prefabFantasmaBase == null)
        {
            Debug.LogError("¡Asigna un Prefab para el fantasma antes de generar!");
            return;
        }

        for (int y = 0; y < dimensionesGrid.y; y++) 
        {
            for (int x = 0; x < dimensionesGrid.x; x++) 
            {
                for (int z = 0; z < dimensionesGrid.z; z++) 
                {
                    Vector3 posicionLocal = offsetInicial + new Vector3(x * separacion.x, y * separacion.y, z * separacion.z);

                    GameObject nuevoSlot = new GameObject($"SlotAuto_{x}_{y}_{z}");
                    nuevoSlot.transform.SetParent(this.transform);
                    nuevoSlot.transform.localPosition = posicionLocal;

                    nuevoSlot.layer = LayerMask.NameToLayer("Interactable");

                    BoxCollider col = nuevoSlot.AddComponent<BoxCollider>();
                    col.isTrigger = true;
                    col.size = new Vector3(0.4f, 0.3f, 0.4f); 

                    DropZoneHandler handler = nuevoSlot.AddComponent<DropZoneHandler>();
                    handler.zoneRole = rolParaEstosSlots;

                    handler.SetGameEvents(this.gameEvents);

                    GameObject fantasma = Instantiate(prefabFantasmaBase, nuevoSlot.transform);
                    fantasma.transform.localPosition = Vector3.zero;
                    handler.SetGhost(fantasma);

                    dropZones.Add(handler);
                }
            }
        }
        Debug.Log($"[ÉXITO] {dropZones.Count} slots generados en {gameObject.name}");
    }

    private void PrepareTableForOrder(PedidosData pedido)
    {
        if (stationType == StationType.Estante || stationType == StationType.Etiquetado || stationType == StationType.Despacho)
        {
            foreach (var zona in dropZones)
            {
                if (zona != null) zona.gameObject.SetActive(true);
            }
            return; 
        }

        foreach (var zona in dropZones)
        {
            if (zona != null)
            {
                zona.gameObject.SetActive(false);
                zona.FreeSlot();
            }
        }

        int necesaryItems = 0;
        foreach (var item in pedido.items) necesaryItems += item.quantity;

        for (int i = 0; i < necesaryItems; i++)
        {
            if (i < dropZones.Count && dropZones[i] != null)
            {
                dropZones[i].gameObject.SetActive(true);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<DeliverableItem>(out var box))
        {
            ProcessBox(box);
        }

        PlayerInteractor player = other.GetComponent<PlayerInteractor>();
        if (player != null && player.objectHeld != null)
        {
            DeliverableItem cajaEnMano = player.objectHeld.GetComponent<DeliverableItem>();
            if (stationType == StationType.Despacho && cajaEnMano != null && cajaEnMano.isPacked && cajaEnMano.isLabeled)
            {
                if (dispatchManager != null) dispatchManager.MostrarFParaMano(cajaEnMano);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerInteractor player = other.GetComponent<PlayerInteractor>();
        if (player != null && stationType == StationType.Despacho)
        {
            if (dispatchManager != null) dispatchManager.OcultarFParaMano();
        }   
    }

    private void ProcessBox(DeliverableItem box)
    {
        if (box == null || box.currentState == DeliverableItem.BoxState.InHands) return;

        if ((stationType == StationType.Empaque || stationType == StationType.Etiquetado) && !box.isStored)
        {
            Debug.LogWarning($"[LOGÍSTICA] {box.itemData.itemName} rechazado: ¡Debes registrarlo en el estante primero!");
            return;
        }

        if (stationType == StationType.Empaque)
        {
            if (!box.isPacked)
            {
                if (tableManager != null) tableManager.PrepararMesaParaEmpaque(box);
            }
        }
        else if (stationType == StationType.Etiquetado)
        {
            if (box.isPacked && !box.isLabeled)
            {
                if (printerManager != null) printerManager.PrepararMesaParaEtiquetado(box);
            }
        }
        else if (stationType == StationType.Despacho)
        {
            if (box.isPacked && box.isLabeled && !box.isDelivered)
            {
                box.isDelivered = true;

                gameEvents.ItemDelivered(box.itemData.itemName);

                if (dispatchManager != null)
                {
                    dispatchManager.PrepararDespacho(box);
                }
                else
                {
                    box.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ShowAvailableGhosts(DeliverableItem heldItem = null)
    {
        foreach (var zona in dropZones)
        {
            if (zona == null || !zona.gameObject.activeSelf) continue;

            if (heldItem == null)
            {
                if (!zona.isOccupied) zona.ShowGhost();
            }
            else
            {
                if (zona.CanDropItem(heldItem))
                {
                    zona.ShowGhost(heldItem);
                }
            }
        }
    }

    public void HideGhosts()
    {
        foreach (var zona in dropZones)
        {
            if (zona != null) zona.HideGhost();
        }
    }

    public DropZoneHandler GetAvailableSlot()
    {
        foreach (var zona in dropZones)
        {
            if (zona != null && !zona.isOccupied && zona.gameObject.activeSelf)
            {
                return zona;
            }
        }
        return null;
    }
}