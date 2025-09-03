using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OrderItemUI : MonoBehaviour
{
    [SerializeField] private List<Image> itemIcon;
    [SerializeField] private List<TMP_Text> itemText;
    [SerializeField] private List<GameObject> cross;

    private List<string> itemNames = new List<string>();
    private List<int> itemQuantities = new List<int>();

    public event Action OnOrderCompleted;
    public void Setup(List<OrderItem> items)
    {
        itemNames.Clear();
        itemQuantities.Clear();

        for (int i = 0; i < itemIcon.Count; i++)
        {
            if (i < items.Count)
            {
                itemIcon[i].gameObject.SetActive(true);
                itemText[i].gameObject.SetActive(true);
                cross[i].SetActive(false);

                itemIcon[i].sprite = items[i].icon;
                itemText[i].text = $"{items[i].itemName} x{items[i].quantity}";

                itemNames.Add(items[i].itemName);
                itemQuantities.Add(items[i].quantity);
            }
            else
            {
                itemIcon[i].gameObject.SetActive(false);
                itemText[i].gameObject.SetActive(false);
                cross[i].SetActive(false);
            }
        }
    }
    public void MarkAsCollected(string collectedItemName)
    {
        for (int i = 0; i < itemNames.Count; i++)
        {
            if (itemNames[i] == collectedItemName && itemQuantities[i] > 0)
            {
                itemQuantities[i]--;

                if (itemQuantities[i] > 0)
                {
                    itemText[i].text = $"{itemNames[i]} x{itemQuantities[i]}";
                }
                else
                {
                    itemText[i].text = $"{itemNames[i]} x0";
                    cross[i].SetActive(true);
                }

                if (IsOrderComplete())
                {
                    OnOrderCompleted?.Invoke();
                }
                break;
            }
        }
    }
    private bool IsOrderComplete()
    {
        foreach (var qua in itemQuantities)
        {
            if (qua > 0) return false;
        }
        return true;
    }
}
