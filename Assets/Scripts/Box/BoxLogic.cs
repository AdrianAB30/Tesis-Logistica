using UnityEngine;
using DG.Tweening;
using TMPro; 
using UnityEngine.UI;

public class BoxLogic : MonoBehaviour, IInteractable
{
    [Header("Configuración del Dispensador")]
    [SerializeField] private ItemData datosDelProducto;
    [SerializeField] private GameObject prefabEngranaje;
    [SerializeField] private GameEvents gameEvents;

    [Header("Animación de Apertura")]
    [SerializeField] private Transform[] tapasCaja;
    [SerializeField] private float tiempoApertura = 0.5f;

    [Header("Inventario y Holograma UI")]
    public int cantidadDisponible = 10;
    [SerializeField] private Canvas uiHolograma;
    [SerializeField] private TextMeshProUGUI textoCantidad;
    [SerializeField] private Image imagenHolograma;

    private bool isOpen = false;
    private DeliverableItem deliverableComponent;

    private void Awake()
    {
        deliverableComponent = GetComponent<DeliverableItem>();
        if (uiHolograma != null) uiHolograma.gameObject.SetActive(false);

        if (datosDelProducto != null)
        {
            cantidadDisponible = datosDelProducto.quantity;
            imagenHolograma.sprite = datosDelProducto.icono;
        }
    }

    public void AbrirCaja()
    {
        if (isOpen) return;
        isOpen = true;

        foreach (Transform tapa in tapasCaja)
        {
            if (tapa != null) tapa.DOLocalRotate(Vector3.zero, tiempoApertura).SetEase(Ease.OutBack);
        }

        if (deliverableComponent != null) deliverableComponent.enabled = false;
        gameObject.tag = "Interactable";

        if (uiHolograma != null) uiHolograma.gameObject.SetActive(true);
        ActualizarUI();

        if (gameEvents != null) gameEvents.ItemStored("CajaMaestra");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.audioDB.sfxWoosh);
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!isOpen || cantidadDisponible <= 0 || interactor.objectHeld != null) return;

        GameObject nuevoProducto = Instantiate(prefabEngranaje, transform.position + (Vector3.up * 0.2f), Quaternion.identity);

        cantidadDisponible--;
        ActualizarUI();

        DeliverableItem itemParaAgarrar = nuevoProducto.GetComponent<DeliverableItem>();
        if (itemParaAgarrar != null)
        {
            if (deliverableComponent != null)
            {
                itemParaAgarrar.isStored = deliverableComponent.isStored;
            }
            interactor.PickUpDeliverable(itemParaAgarrar);
        }

        if (gameEvents != null) gameEvents.ItemPicked("Engranaje");
    }

    private void ActualizarUI()
    {
        if (textoCantidad != null)
        {
            textoCantidad.text = "X " + cantidadDisponible.ToString();
        }
    }
}