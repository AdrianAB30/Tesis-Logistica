using UnityEngine;
using DG.Tweening;

public class DoorRotator : MonoBehaviour, IInteractable
{
    [Header("Configuración de Rotación")]
    [SerializeField] private Vector3 anguloAbierto = new Vector3(0, 90, 0); 
    [SerializeField] private float duracion = 0.8f;
    [SerializeField] private Ease curvaFacilidad = Ease.InOutCubic;

    private bool estaAbierta = false;
    private bool enMovimiento = false;
    private Vector3 rotacionCerrada;

    private void Start()
    {
        rotacionCerrada = transform.localEulerAngles;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (enMovimiento) return;

        enMovimiento = true;
        Vector3 target = estaAbierta ? rotacionCerrada : anguloAbierto;
        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxDoor, AudioManager.Instance.audioDB.sfxDoorVolume);
        transform.DOLocalRotate(target, duracion)
            .SetEase(curvaFacilidad)
            .OnComplete(() =>
            {
                estaAbierta = !estaAbierta;
                enMovimiento = false;
            });
    }
}