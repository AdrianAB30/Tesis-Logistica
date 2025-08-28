using MoreMountains.Feedbacks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MMF_Player blinkEffects;


    private void Start()
    {
        blinkEffects.PlayFeedbacks();
    }
}
