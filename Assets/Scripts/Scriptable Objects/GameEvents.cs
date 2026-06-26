using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Scriptable Objects/Observer Pattern/GameEvents", order = 2)]
public class GameEvents : ScriptableObject
{
    public Action<PedidosData> OnOrderReceived;
    public Action OnOrderCompleted;

    public Action<string> OnItemStored;
    public Action<string> OnItemPicked;
    public event Action<string> OnItemLabeled;
    public Action<string> OnItemDelivered;
    public Action<float> OnTimeUpdated;
    public Action<string> OnItemPacked;
    public event Action OnPackingStarted;
    public event Action OnPrinterStarted;
    public event Action OnMistakeMade;
    public event Action<int> OnStarLost;

    public void OrderReceived(PedidosData order) => OnOrderReceived?.Invoke(order);
    public void OrderCompleted() => OnOrderCompleted?.Invoke();

    public void ItemStored(string itemName) => OnItemStored?.Invoke(itemName);

    public void ItemPicked(string itemName) => OnItemPicked?.Invoke(itemName);
    public void ItemDelivered(string itemName) => OnItemDelivered?.Invoke(itemName);
    public void ItemLabeled(string itemName) => OnItemLabeled?.Invoke(itemName);
    public void TimeUpdated(float time) => OnTimeUpdated?.Invoke(time);
    public void ItemPacked(string itemName) => OnItemPacked?.Invoke(itemName);
    public void PackingStarted() => OnPackingStarted?.Invoke();
    public void PrinterStarted() => OnPrinterStarted?.Invoke();
    public void MistakeMade() => OnMistakeMade?.Invoke();
    public void StarLost(int remainingStars) => OnStarLost?.Invoke(remainingStars);

}