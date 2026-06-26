using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/Pedidos/ItemData", order = 5)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int quantity;
    public Sprite icono;

    [Header("Físicas para Empaque")]
    public float volumenCm3; 
    public float pesoKg;     
}