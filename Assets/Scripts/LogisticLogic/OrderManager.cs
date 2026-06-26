using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private List<PedidosData> pedidos = new List<PedidosData>();

    private int currentOrderIndex = 0;

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
            levelManager.CompleteLevel();
        }
    }
}