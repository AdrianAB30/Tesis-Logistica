using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private List<PedidosData> pedidos = new List<PedidosData>();

    private int currentOrderIndex = 0;

    private void Start()
    {
        StartFirstOrder();
    }

    private void StartFirstOrder()
    {
        if (pedidos.Count > 0)
        {
            gameEvents.OrderReceived(pedidos[currentOrderIndex]);
        }
    }
    public void CompleteOrder()
    {
        gameEvents.OrderCompleted();
        currentOrderIndex++;

        if (currentOrderIndex < pedidos.Count)
        {
            gameEvents.OrderReceived(pedidos[currentOrderIndex]);
        }
        else
        {
            Debug.Log("Todos los pedidos completados");
        }
    }
}
