using UnityEngine;

[CreateAssetMenu(fileName = "NuevoPedido", menuName = "Scriptable Objects/Pedidos/Pedidos", order = 1)]
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
    public ItemData itemData;
    public int quantity;
}