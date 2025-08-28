using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Scriptable Objects/GameEvents", order = 2)]
public class GameEvents : ScriptableObject
{
    public Action<PedidosData> OnOrderReceived;
    public Action OnOrderCompleted;
    public Action<string> OnItemPicked;
    public Action<string> OnItemDelivered;
    public Action<float> OnTimeUpdated;

    public void OrderReceived(PedidosData order) => OnOrderReceived?.Invoke(order);
    public void OrderCompleted() => OnOrderCompleted?.Invoke();
    public void ItemPicked(string itemName) => OnItemPicked?.Invoke(itemName);
    public void ItemDelivered(string itemName) => OnItemDelivered?.Invoke(itemName);
    public void TimeUpdated(float time) => OnTimeUpdated?.Invoke(time);
}
