using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

[System.Serializable]

public class UiManager : MonoBehaviour
{
    [Header("Fade inicial")]
    [SerializeField] private MMF_Player blinkEffects;

    [Header("Pop-ups")]
    [SerializeField] private RectTransform[] popUp;
    private bool activePopUps;

    [Header("Player Control")]
    [SerializeField] private PlayerController playerController;

    [Header("DOTween")]
    [SerializeField] private DOTweenConfig dgConfig;
    [SerializeField] private Vector3[] targetPositions;
    [SerializeField] private Vector3[] originalPositions;


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
        yield return new WaitForSeconds(2.5f);
        ActivatePopUps(true);
        yield return new WaitForSeconds(1);
        playerController.SetCanMove(true);
        DOTween.Sequence()
        .Append(popUp[0].DOAnchorPos(targetPositions[0], dgConfig.popUpTime).SetEase(dgConfig.popUpEase))
        .AppendInterval(3f)
        .Append(popUp[0].DOAnchorPos(originalPositions[0], dgConfig.popUpTime).SetEase(dgConfig.popUpEase));

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
}
