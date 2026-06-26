using UnityEngine;
using TMPro;
using DG.Tweening;

public class DocumentValidator : MonoBehaviour, IInteractable
{
    [Header("UI Diegética (Textos 3D en el Portapapeles)")]
    [SerializeField] private TextMeshPro textoOrdenCompra;
    [SerializeField] private TextMeshPro textoGuiaRemision;
    [SerializeField] private TextMeshPro textoEstado;

    [Header("Base de Datos (Pedido del Tutorial)")]
    [SerializeField] private PedidosData pedidoTutorial;
    [SerializeField] private GameEvents gameEvents;

    [Header("Animación (DOTween)")]
    [SerializeField] private DOTweenConfig dtConfig; 

    private bool isValidado = false;

    private void Start()
    {
        if (pedidoTutorial != null && pedidoTutorial.items.Length > 0)
        {
            string infoProducto = $"{pedidoTutorial.items[0].quantity}x {pedidoTutorial.items[0].itemData.itemName}";
            textoOrdenCompra.text = $"Orden de Compra:\n{infoProducto}";
            textoGuiaRemision.text = $"Guía de Remisión:\n{infoProducto}";
        }
        textoEstado.text = "ESTADO: PENDIENTE";
        textoEstado.color = Color.yellow;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (isValidado) return;
        isValidado = true;

        // AHORA USA EL CEREBRO DE DOTWEEN
        transform.DOPunchPosition(dtConfig.stampPunchVector, dtConfig.stampPunchTime, dtConfig.stampPunchVibrato, dtConfig.stampPunchElasticity);

        textoEstado.text = "ESTADO: APROBADO\n(Orden de Internamiento Generada)";
        textoEstado.color = Color.green;
        Debug.Log("[Flujo Logístico] Etapa 1 completada: Proceso Documentario OK.");

        gameEvents.OrderReceived(pedidoTutorial);
    }
}