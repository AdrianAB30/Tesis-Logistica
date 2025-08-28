using UnityEngine;

[CreateAssetMenu(fileName = "Pedidos", menuName = "Scriptable Objects/Pedidos", order = 1)]
public class PedidosData : ScriptableObject
{
    public string orderName;
    public Sprite orderIcon;
    public float timeLimit;
    public OrderItem[] items;
}

[System.Serializable]
public class OrderItem
{
    public string itemName;
    public int quantity;
    public Sprite icon;
}
