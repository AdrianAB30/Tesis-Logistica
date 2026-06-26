using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PrinterMinigame : MonoBehaviour
{
    [Header("Configuración del Timing")]
    [Tooltip("Distancia máxima en el eje X hacia los lados")]
    public float rangoMovimiento = 150f;
    [Tooltip("Qué tan cerca del centro (0) debe estar para ganar")]
    public float margenExito = 10f;

    private bool isPlaying = false;
    private Vector3 escalaOriginalCanvas;
    private Tween agujaTween;

    [Header("Referencias UI")]
    [SerializeField] private CanvasGroup uiCanvas;
    [SerializeField] private RectTransform aguja;
    [SerializeField] private RectTransform zonaVerde;

    [Header("Referencias Físicas (Máquina)")]
    [SerializeField] private Transform impresoraPivot;
    [SerializeField] private Transform botonRojo;
    [SerializeField] private Vector3 direccionHundirBoton = new Vector3(0, 0, -0.05f);

    [Header("Referencias Físicas (Etiqueta)")]
    [SerializeField] private Transform etiquetaFisica;
    [SerializeField] private Transform puntoRanura;
    [SerializeField] private Transform puntoAterrizaje;

    [Header("Conexión S.O.")]
    [SerializeField] private DOTweenConfig dtAnims;
    [SerializeField] private GameEvents gameEvents;

    private DeliverableItem cajaActual;

    private void Start()
    {
        if (uiCanvas != null)
        {
            escalaOriginalCanvas = uiCanvas.transform.localScale; 
            uiCanvas.alpha = 0f;
        }
        if (etiquetaFisica != null) etiquetaFisica.gameObject.SetActive(false);
    }

    public void IniciarMinijuego(DeliverableItem cajaDestino, float delayInteract = 0f)
    {
        cajaActual = cajaDestino;
        isPlaying = false; 
        uiCanvas.alpha = 1f;
        uiCanvas.transform.localScale = Vector3.zero;

        uiCanvas.transform.DOScale(escalaOriginalCanvas, dtAnims.printerUIPopTime).SetEase(dtAnims.printerUIPopEase);

        etiquetaFisica.gameObject.SetActive(false);

        aguja.localPosition = new Vector3(-rangoMovimiento, aguja.localPosition.y, 0);

        if (delayInteract > 0f)
        {
            DOVirtual.DelayedCall(delayInteract, () =>
            {
                isPlaying = true;
                IniciarAguja();
            });
        }
        else
        {
            isPlaying = true;
            IniciarAguja();
        }
    }
    private void IniciarAguja()
    {
        agujaTween = aguja.DOLocalMoveX(rangoMovimiento, dtAnims.printerNeedleSpeed)
            .SetEase(dtAnims.printerNeedleEase)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId("MovimientoAguja");
    }

    private void Update()
    {
        if (!isPlaying) return;

        if (Input.GetMouseButtonDown(0))
        {
            EvaluarTiming();
        }
    }

    private void EvaluarTiming()
    {
        agujaTween.Pause();
        float distanciaAlCentro = Mathf.Abs(aguja.localPosition.x);

        if (distanciaAlCentro <= margenExito)
        {
            isPlaying = false;
            DOTween.Kill("MovimientoAguja");
            AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxSuccess, AudioManager.Instance.audioDB.sfxSuccessVolume);

            EjecutarCinematicaImpresion();
        }
        else
        {
            AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxError, AudioManager.Instance.audioDB.sfxErrorVolume);

            if (gameEvents != null) gameEvents.MistakeMade();

            if (agujaTween != null) agujaTween.timeScale += 0.2f;

            uiCanvas.transform.DOShakePosition(dtAnims.printerErrorShakeDuration, dtAnims.printerErrorShakeStrength, dtAnims.printerErrorShakeVibrato)
                .OnComplete(() => agujaTween.Play());
        }
    }

    private void EjecutarCinematicaImpresion()
    {
        uiCanvas.transform.DOScale(Vector3.zero, dtAnims.printerUIPopTime).SetEase(Ease.InBack);

        Sequence seqImpresora = DOTween.Sequence();

        seqImpresora.Append(botonRojo.DOLocalMove(botonRojo.localPosition + direccionHundirBoton, dtAnims.printerButtonPunchTime).SetLoops(2, LoopType.Yoyo));
        seqImpresora.AppendCallback(() => AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, 1f));
        seqImpresora.Append(impresoraPivot.DOLocalJump(impresoraPivot.localPosition, dtAnims.printerJumpPower, 1, dtAnims.printerJumpTime));

        seqImpresora.AppendCallback(() =>
        {
            AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxWoosh, 1f);
            etiquetaFisica.gameObject.SetActive(true);
            etiquetaFisica.position = puntoRanura.position;
            etiquetaFisica.localScale = Vector3.zero;
        });

        seqImpresora.Append(etiquetaFisica.DOScale(Vector3.one, dtAnims.printerLabelScaleTime));
        seqImpresora.Join(etiquetaFisica.DOJump(puntoAterrizaje.position, dtAnims.printerLabelJumpPower, 1, dtAnims.printerLabelJumpTime));
        seqImpresora.Join(etiquetaFisica.DORotate(new Vector3(0, 360, 0), dtAnims.printerLabelRotateTime, RotateMode.FastBeyond360));

        seqImpresora.OnComplete(() =>
        {
            if (cajaActual != null && cajaActual.gameObject != null)
            {
                Transform puntoReal = cajaActual.transform.Find("Punto Aterrizaje");

                if (puntoReal != null)
                {
                    GameObject etiquetaClon = Instantiate(etiquetaFisica.gameObject, puntoReal.position, puntoReal.rotation);
                    etiquetaClon.transform.SetParent(cajaActual.transform, true);

                    etiquetaClon.transform.localScale = new Vector3(1 / cajaActual.transform.lossyScale.x,
                                                                   1 / cajaActual.transform.lossyScale.y,
                                                                   1 / cajaActual.transform.lossyScale.z);

                    etiquetaClon.transform.localPosition = new Vector3(-0.1f, 0.185f, -0.09f); 

                    etiquetaFisica.gameObject.SetActive(false);
                    cajaActual.isLabeled = true;

                    if (gameEvents != null) gameEvents.ItemLabeled(cajaActual.itemData.itemName);
                }
            }

            PrinterTableManager manager = FindFirstObjectByType<PrinterTableManager>();
            if (manager != null) manager.FinalizarMinijuego();

            CameraManager.Instance.ReturnToPlayerCamera();

            PlayerInteractor player = FindFirstObjectByType<PlayerInteractor>();
            if (player != null)
            {
                InputReader input = player.GetComponent<InputReader>();
                if (input != null) input.SetInputActive(true);
            }

            Debug.Log("[IMPRESORA] Control devuelto al jugador.");
        });
    }
}