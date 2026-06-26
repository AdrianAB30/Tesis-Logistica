using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class WarehouseMonitor : MonoBehaviour
{
    [Header("Pantalla")]
    [SerializeField] private TMP_Text tituloPantalla;
    [SerializeField] private TMP_Text contenidoPantalla;

    [Header("Eventos")]
    [SerializeField] private GameEvents gameEvents;

    private PedidosData pedidoActual;
    private Dictionary<string, int> itemsFaltantes = new Dictionary<string, int>();

    private void OnEnable()
    {
        gameEvents.OnOrderReceived += IniciarNuevoPedido;
        gameEvents.OnItemDelivered += ActualizarProgreso;
    }

    private void OnDisable()
    {
        gameEvents.OnOrderReceived -= IniciarNuevoPedido;
        gameEvents.OnItemDelivered -= ActualizarProgreso;
    }

    private void Start()
    {
        tituloPantalla.text = "SISTEMA ANDON LOGISTICO";
        contenidoPantalla.text = "Esperando la validacion de la orden actual.";
        contenidoPantalla.color = Color.white;
    }

    private void IniciarNuevoPedido(PedidosData pedido)
    {
        pedidoActual = pedido;
        itemsFaltantes.Clear();

        foreach (var item in pedido.items)
        {
            if (itemsFaltantes.ContainsKey(item.itemData.itemName))
                itemsFaltantes[item.itemData.itemName] += item.quantity;
            else
                itemsFaltantes.Add(item.itemData.itemName, item.quantity);
        }

        ActualizarTextoVisual();
    }

    private void ActualizarProgreso(string nombreDelItem)
    {
        if (itemsFaltantes.ContainsKey(nombreDelItem))
        {
            itemsFaltantes[nombreDelItem]--;
            if (itemsFaltantes[nombreDelItem] < 0) itemsFaltantes[nombreDelItem] = 0;

            ActualizarTextoVisual();

            bool todoCompleto = true;
            foreach (var kvp in itemsFaltantes)
            {
                if (kvp.Value > 0) todoCompleto = false;
            }

            if (todoCompleto)
            {
                tituloPantalla.text = "<color=green>¡ORDEN DESPACHADA!</color>";
                contenidoPantalla.text = "El camion de salida ha sido cargado exitosamente.";

                gameEvents.OrderCompleted();
            }
        }
    }

    private void ActualizarTextoVisual()
    {
        if (pedidoActual == null) return;

        tituloPantalla.text = $"<color=yellow>PEDIDO ACTIVO: {pedidoActual.orderName}</color>";
        string textoDinamico = "\n";

        foreach (var item in pedidoActual.items)
        {
            int faltantes = itemsFaltantes[item.itemData.itemName];

            if (faltantes > 0)
            {
                textoDinamico += $"{item.itemData.itemName} (Faltan: {faltantes})\n";
            }
            else
            {
                textoDinamico += $"<color=green>[X] <s>{item.itemData.itemName}</s> (OK)</color>\n";
            }
        }

        contenidoPantalla.text = textoDinamico;
    }
}