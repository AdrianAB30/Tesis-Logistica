using UnityEngine;
using DG.Tweening;

public class TruckDispatchManager : MonoBehaviour, IInteractable
{
    [Header("Componentes del Camión")]
    [SerializeField] private Transform cuerpoCamion;
    [SerializeField] private Transform puertaIzquierda;
    [SerializeField] private Transform puertaDerecha;
    [SerializeField] private Transform[] ruedas;

    [Header("Puntos de Destino (Movimiento)")]
    [Tooltip("Punto fuera del almacén donde el camión espera oculto")]
    [SerializeField] private Transform puntoInicioCamion;
    [Tooltip("Punto en el muelle donde el camión se estaciona")]
    [SerializeField] private Transform puntoLlegadaCamion;
    [SerializeField] private Transform puntoAterrizajeCaja;
    [SerializeField] private Transform puntoFugaCamion;

    [Header("Cinemática y UI")]
    [SerializeField] private GameObject uiInteractuarF;

    [Header("Conexión S.O.")]
    [SerializeField] private DOTweenConfig dtAnims;
    [SerializeField] private GameEvents gameEvents;

    [Header("Configuración del Pedido")]
    public int cajasNecesarias = 3;
    private int cajasCargadas = 0;

    private bool camionEnMuelle = false;
    private bool listoParaDespachar = false;
    private DeliverableItem cajaADespachar = null;

    private void Start()
    {
        if (uiInteractuarF != null) uiInteractuarF.SetActive(false);

        if (puntoInicioCamion != null) transform.position = puntoInicioCamion.position;
    }

    // --- 1. LLEGADA DEL CAMIÓN ---
    public void HacerEntrarCamion()
    {
        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxTruckReverse, AudioManager.Instance.audioDB.sfxTruckReverseVolume);

        Sequence seqLlegada = DOTween.Sequence();

        foreach (Transform rueda in ruedas)
        {
            if (rueda != null)
                rueda.DOLocalRotate(new Vector3(360, 0, 0), 1f / (dtAnims.wheelRotationSpeed / 360f), RotateMode.FastBeyond360)
                     .SetEase(dtAnims.wheelEase).SetRelative(true).SetLoops(-1, LoopType.Restart);
        }

        cuerpoCamion.DOShakePosition(dtAnims.truckShakeTime, dtAnims.truckShakeStrength, dtAnims.truckShakeVibrato).SetEase(dtAnims.truckMoveEase);

        seqLlegada.Append(transform.DOMove(puntoLlegadaCamion.position, dtAnims.truckShakeTime).SetEase(dtAnims.truckMoveEase));

        seqLlegada.OnComplete(() =>
        {
            foreach (Transform rueda in ruedas) if (rueda != null) rueda.DOKill();
            cuerpoCamion.DOKill();

            cuerpoCamion.DOPunchRotation(dtAnims.truckBrakePunch, dtAnims.truckBrakeTime, dtAnims.truckBrakeVibrato, 0.5f);
            AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxAirBrakes, AudioManager.Instance.audioDB.sfxAirBrakesVolume);

            puertaIzquierda.DOLocalRotate(dtAnims.rotacionIzqAbierta, dtAnims.dispatchDoorCloseTime).SetEase(dtAnims.doorsOpened);
            puertaDerecha.DOLocalRotate(dtAnims.rotacionDerAbierta, dtAnims.dispatchDoorCloseTime).SetEase(dtAnims.doorsOpened);

            camionEnMuelle = true;

            if (cajaADespachar != null && !listoParaDespachar)
            {
                PrepararDespacho(cajaADespachar);
            }
        });
    }

    // --- 2. PREPARAR CAJA ---
    public void PrepararDespacho(DeliverableItem caja)
    {
        cajaADespachar = caja;

        if (camionEnMuelle && !listoParaDespachar)
        {
            listoParaDespachar = true;
            if (uiInteractuarF != null) uiInteractuarF.SetActive(true);
            AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, 1f);
        }
    }

    // --- 3. INTERACCIÓN (F) ---
    public void Interact(PlayerInteractor interactor)
    {
        if (cajaADespachar == null || !camionEnMuelle) return;

        listoParaDespachar = false;
        if (uiInteractuarF != null) uiInteractuarF.SetActive(false);

        InputReader input = interactor.GetComponent<InputReader>();
        if (input != null) input.SetInputActive(false);

        if (interactor.objectHeld != null && interactor.objectHeld == cajaADespachar.gameObject)
        {
            interactor.ClearHand();
        }

        if (gameEvents != null) gameEvents.ItemDelivered(cajaADespachar.itemData.itemName);

        EjecutarCinematicaCargaYSalida(input);
    }

    private void EjecutarCinematicaCargaYSalida(InputReader input)
    {
        cajasCargadas++;
        bool esUltimaCaja = (cajasCargadas >= cajasNecesarias);

        Sequence seqDespacho = DOTween.Sequence();

        if (cajaADespachar != null)
        {
            cajaADespachar.transform.SetParent(cuerpoCamion, true);
            Rigidbody rb = cajaADespachar.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            
            Vector3 offset = Vector3.zero;
            if (cajasCargadas == 1) offset = puntoAterrizajeCaja.right * -0.4f;      // Primera caja: Izquierda
            else if (cajasCargadas == 2) offset = puntoAterrizajeCaja.right * 0.4f; // Segunda caja: Derecha
            else if (cajasCargadas == 3) offset = puntoAterrizajeCaja.forward * 0.5f; // Tercera caja: Al centro, pegada a la puerta

            seqDespacho.Append(cajaADespachar.transform.DOJump(puntoAterrizajeCaja.position + offset, dtAnims.dispatchBoxJumpPower, 1, dtAnims.dispatchBoxJumpTime).SetEase(dtAnims.dispatchBoxJumpEase));
            seqDespacho.Join(cajaADespachar.transform.DORotate(puntoAterrizajeCaja.eulerAngles, dtAnims.dispatchBoxJumpTime));
        }

        seqDespacho.AppendCallback(() => AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxSuccess, 1f));

        if (esUltimaCaja)
        {
            seqDespacho.Append(puertaIzquierda.DOLocalRotate(Vector3.zero, dtAnims.dispatchDoorCloseTime).SetEase(dtAnims.dispatchDoorCloseEase));
            seqDespacho.Join(puertaDerecha.DOLocalRotate(Vector3.zero, dtAnims.dispatchDoorCloseTime).SetEase(dtAnims.dispatchDoorCloseEase));
            seqDespacho.AppendCallback(() => AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, 1f));

            Vector3 direccionAtras = (puntoInicioCamion.position - transform.position).normalized;
            seqDespacho.Append(transform.DOMove(transform.position + (direccionAtras * dtAnims.dispatchAnticipationDist), dtAnims.dispatchAnticipationTime).SetEase(dtAnims.dispatchAnticipationEase));
            seqDespacho.Join(cuerpoCamion.DOLocalRotate(dtAnims.dispatchAnticipationRot, dtAnims.dispatchAnticipationTime));

            seqDespacho.AppendCallback(() => {
                foreach (Transform rueda in ruedas)
                    if (rueda != null) rueda.DOLocalRotate(new Vector3(360, 0, 0), dtAnims.dispatchFastWheelTime, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetRelative(true).SetLoops(-1, LoopType.Restart);
            });

            seqDespacho.Append(transform.DOMove(puntoFugaCamion.position, dtAnims.dispatchFugaTime).SetEase(dtAnims.dispatchFugaEase));
            seqDespacho.Join(cuerpoCamion.DOLocalRotate(Vector3.zero, dtAnims.dispatchTrompaBajaTime));
            seqDespacho.Join(cuerpoCamion.DOShakePosition(dtAnims.dispatchFugaTime, dtAnims.dispatchFugaShakeStrength, dtAnims.dispatchFugaShakeVibrato));

            if (puntoInicioCamion != null)
            {
                seqDespacho.AppendCallback(() => {
                    foreach (Transform rueda in ruedas) if (rueda != null) rueda.DOKill();
                });
                seqDespacho.Append(transform.DOMove(puntoInicioCamion.position, dtAnims.dispatchReturnTime).SetEase(dtAnims.dispatchReturnEase));
            }
        }

        seqDespacho.OnComplete(() =>
        {
            if (esUltimaCaja)
            {
                foreach (Transform hijo in cuerpoCamion)
                {
                    if (hijo.GetComponent<DeliverableItem>() != null)
                    {
                        hijo.gameObject.SetActive(false);
                    }
                }

                if (cuerpoCamion != null)
                {
                    cuerpoCamion.DOKill();
                    cuerpoCamion.localRotation = Quaternion.identity;
                }
                camionEnMuelle = false;
                LevelManager levelManager = FindFirstObjectByType<LevelManager>();
                if (levelManager != null) levelManager.CompleteLevel();
            }
            else
            {
                if (input != null) input.SetInputActive(true);
            }
        });
    }

    public void MostrarFParaMano(DeliverableItem caja)
    {
        if (camionEnMuelle && !listoParaDespachar)
        {
            cajaADespachar = caja;
            if (uiInteractuarF != null) uiInteractuarF.SetActive(true);
        }
    }

    public void OcultarFParaMano()
    {
        if (camionEnMuelle && !listoParaDespachar)
        {
            cajaADespachar = null;
            if (uiInteractuarF != null) uiInteractuarF.SetActive(false);
        }
    }
}