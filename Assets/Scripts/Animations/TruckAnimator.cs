using UnityEngine;
using DG.Tweening;

public class TruckAnimator : MonoBehaviour
{
    [SerializeField] private DOTweenConfig dtAnims;
    [SerializeField] private Transform[] ruedas;
    [SerializeField] private Transform cuerpoCamion;

    public void LlegadaDeCamion(Vector3 puntoDestino)
    {
        cuerpoCamion.DOShakePosition(dtAnims.truckShakeTime, dtAnims.truckShakeStrength, dtAnims.truckShakeVibrato)
                    .SetEase(Ease.Linear);

        foreach (Transform rueda in ruedas)
        {
            rueda.DOLocalRotate(new Vector3(360, 0, 0), 1f / (dtAnims.wheelRotationSpeed / 360f), RotateMode.FastBeyond360)
                 .SetEase(dtAnims.wheelEase)
                 .SetRelative(true)
                 .SetLoops(-1, LoopType.Restart);
        }

        transform.DOMove(puntoDestino, dtAnims.truckShakeTime)
              .SetEase(dtAnims.truckMoveEase)
              .OnComplete(() => {
                  foreach (Transform rueda in ruedas) rueda.DOKill();

                  cuerpoCamion.DOKill();

                  cuerpoCamion.DOPunchRotation(dtAnims.truckBrakePunch, dtAnims.truckBrakeTime, dtAnims.truckBrakeVibrato, 0.5f);

                  AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxAirBrakes, AudioManager.Instance.audioDB.sfxAirBrakesVolume);
              });
    }
}