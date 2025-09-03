using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class UiManager : MonoBehaviour
{
    [Header("Fade inicial")]
    [SerializeField] private MMF_Player blinkEffects;

    [Header("Pop-ups")]
    [SerializeField] private RectTransform[] popUp;
    [SerializeField] private TMP_Text popUpText;
    private bool activePopUps;

    [Header("Player Control")]
    [SerializeField] private PlayerController playerController;

    [Header("DOTween")]
    [SerializeField] private DOTweenConfig dgConfig;
    [SerializeField] private Vector3[] targetPositions;
    [SerializeField] private Vector3[] originalPositions;

    [Header("Eventos")]
    [SerializeField] private GameEvents gameEvents;

    [Header("UI Pedido")]
    [SerializeField] private GameObject orderPanel;
    [SerializeField] private Transform orderItemsParent;
    [SerializeField] private GameObject orderItemPrefab;
    [SerializeField] private TMP_Text orderNameText;
    [SerializeField] private Image orderIconImage;
    [SerializeField] private OrderItemUI orderUI;


    private void OnEnable()
    {
        gameEvents.OnOrderReceived += ShowOrderUI;
        gameEvents.OnItemDelivered += HandleItemDelivered;
    }

    private void OnDisable()
    {
        gameEvents.OnOrderReceived -= ShowOrderUI;
        gameEvents.OnItemDelivered -= HandleItemDelivered;

    }
    private void Start()
    {
        blinkEffects.PlayFeedbacks();
        blinkEffects.Events.OnComplete.AddListener(OnBlinkComplete);
        ActivatePopUps(false);
    }

    private void OnBlinkComplete()
    {
        StartCoroutine(ShowPopUps());
    }

    private IEnumerator ShowPopUps()
    {
        yield return new WaitForSeconds(0.5f);

        ActivatePopUps(true);

        DOTween.Sequence()
            .Append(popUp[0].DOAnchorPos(targetPositions[0], dgConfig.popUpTime)
            .SetEase(dgConfig.popUpEase))
            .AppendInterval(2f)
            .Append(popUp[0].DOAnchorPos(originalPositions[0], dgConfig.popUpTime)
            .SetEase(dgConfig.popUpEase))
            .AppendCallback(() =>
            {

                popUpText.text = "Start moving using " + " W,A,S,D";
            })
            .AppendInterval(2f)
            .Append(popUp[0].DOAnchorPos(targetPositions[0], dgConfig.popUpTime)
            .SetEase(dgConfig.popUpEase))
            .AppendInterval(2f)
            .Append(popUp[0].DOAnchorPos(originalPositions[0], dgConfig.popUpTime)
            .SetEase(dgConfig.popUpEase))
             .AppendCallback(() =>
             {
                 popUpText.text = "You have received an order, you have to go and serve it.";
             })
            .AppendInterval(2f)
            .Append(popUp[0].DOAnchorPos(targetPositions[0], dgConfig.popUpTime)
            .SetEase(dgConfig.popUpEase))
            .AppendInterval(2.5f)
            .Append(popUp[0].DOAnchorPos(originalPositions[0], dgConfig.popUpTime)
            .SetEase(dgConfig.popUpEase))
             .OnComplete(() =>
             {
                 playerController.SetCanMove(true);

                 popUp[1].DOAnchorPos(targetPositions[1], dgConfig.popUpTime)
                 .SetEase(dgConfig.popUpEase);
             });
    }
    public void ActivatePopUps(bool status)
    {
        activePopUps = status;

        if (status)
        {
            for (int i = 0; i < popUp.Length; i++)
            {
                popUp[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < popUp.Length; i++)
            {
                popUp[i].gameObject.SetActive(false);
            }

        }
    }
    private void ShowOrderUI(PedidosData pedido)
    {
        orderNameText.text = pedido.orderName;
        orderIconImage.sprite = pedido.orderIcon;

        foreach (Transform child in orderItemsParent)
        {
            Destroy(child.gameObject);
        }

        GameObject obj = Instantiate(orderItemPrefab, orderItemsParent);
        OrderItemUI itemUI = obj.GetComponent<OrderItemUI>();
        if (itemUI != null)
        {
            itemUI.Setup(new List<OrderItem>(pedido.items));
            itemUI.OnOrderCompleted += HandleOrderCompleted;

            orderUI = itemUI;
        }
    }
    private void HandleOrderCompleted()
    {
        Debug.Log("Pedido completado ✅");
        gameEvents.OrderCompleted(); 
    }
    private void HandleItemDelivered(string itemName)
    {
        if (orderUI != null)
        {
            orderUI.MarkAsCollected(itemName);
        }
    }
}
