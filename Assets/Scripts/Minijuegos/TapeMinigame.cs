using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TapeMinigame : MonoBehaviour
{
    [System.Serializable]
    public class TapaConfig
    {
        public Transform bisagra;
        public Vector3 rotacionCerrada;
        [HideInInspector] public Vector3 rotacionAbierta;
    }

    [Header("Configuración del Minijuego")]
    public float tiempoParaSellar = 2f;
    [Tooltip("Porcentaje mínimo para soltar y ganar (ej. 0.80 = 80%)")]
    public float porcentajeExitoMin = 0.80f;
    private float progresoActual = 0f;
    private bool isPlaying = false;

    [Header("Referencias UI")]
    [SerializeField] private CanvasGroup minigameCanvas;
    [SerializeField] private RectTransform uiContenedor;
    [SerializeField] private Image barraCircular;
    [SerializeField] private Color colorInicio = Color.yellow;
    [SerializeField] private Color colorFin = Color.green;

    [Header("Referencias Cinemáticas (Caja Activa)")]
    private Transform boxPivotActivo;
    private TapaConfig[] tapasActivas;
    [SerializeField] private Vector3 rotacionCajaDePie = new Vector3(360, 0, 0);

    private Vector2 posicionInicialUI;
    private Sequence secuenciaEmpaque;

    [Header("El Prop Swap (Caja Real)")]
    [SerializeField] private GameObject prefabCajaEmpacadaReal;
    [SerializeField] private Vector3 offsetAparicionCaja = Vector3.zero;

    [Header("Referencias Core")]
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private DOTweenConfig dtAnims;

    private DeliverableItem productoFisicoActual;

    private void Start()
    {
        minigameCanvas.alpha = 0f;
        barraCircular.fillAmount = 0f;

        if (uiContenedor != null)
            posicionInicialUI = uiContenedor.anchoredPosition;
    }

    public void IniciarMinijuegoDinamico(DeliverableItem producto, Transform clonCaja, TapaConfig[] configsTapas, float delayInteract = 0f)
    {
        productoFisicoActual = producto;
        boxPivotActivo = clonCaja;
        tapasActivas = configsTapas;

        progresoActual = 0f;
        barraCircular.fillAmount = 0f;
        barraCircular.color = colorInicio;
        isPlaying = false;

        if (secuenciaEmpaque != null) secuenciaEmpaque.Kill();

        foreach (var config in tapasActivas)
        {
            if (config.bisagra != null)
                config.rotacionAbierta = config.bisagra.localEulerAngles;
        }

        minigameCanvas.DOFade(1f, dtAnims.minigameUIAnimTime);
        uiContenedor.DOKill();
        uiContenedor.anchoredPosition = posicionInicialUI;
        uiContenedor.localScale = Vector3.zero;
        uiContenedor.DOScale(Vector3.one, dtAnims.minigameUIAnimTime).SetEase(Ease.OutBack);

        if (delayInteract > 0f)
            DOVirtual.DelayedCall(delayInteract, () => isPlaying = true);
        else
            isPlaying = true;
    }

    private void Update()
    {
        if (!isPlaying) return;

        if (Input.GetMouseButtonDown(0))
        {
            uiContenedor.DOPunchScale(dtAnims.tapeClickPunchScale, dtAnims.tapeClickPunchDuration);
            AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, 1f);
        }

        if (Input.GetMouseButton(0))
        {
            progresoActual += Time.deltaTime;
            float porcentaje = progresoActual / tiempoParaSellar;
            barraCircular.fillAmount = porcentaje;

            if (porcentaje >= porcentajeExitoMin) barraCircular.color = colorFin;
            else barraCircular.color = colorInicio;

            if (!DOTween.IsTweening("ShakeCinta"))
            {
                uiContenedor.DOShakeAnchorPos(dtAnims.tapeTensionShakeDuration, dtAnims.tapeTensionShakeStrength, dtAnims.tapeTensionShakeVibrato, 90f, false, true).SetId("ShakeCinta");
            }

            if (porcentaje > 1f) RomperCinta();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            float porcentaje = progresoActual / tiempoParaSellar;

            if (porcentaje >= porcentajeExitoMin && porcentaje <= 1f) CompletarMinijuego();
            else if (porcentaje > 0f) RomperCinta();
        }
    }

    private void RomperCinta()
    {
        progresoActual = 0f;
        barraCircular.fillAmount = 0f;
        barraCircular.color = colorInicio;
        DOTween.Kill("ShakeCinta");
        uiContenedor.DOShakeAnchorPos(dtAnims.tapeErrorShakeDuration, dtAnims.tapeErrorShakeStrength, dtAnims.tapeErrorShakeVibrato);
        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxError, 1f);
        if (gameEvents != null) gameEvents.MistakeMade();
    }

    private void CompletarMinijuego()
    {
        isPlaying = false;
        DOTween.Kill("ShakeCinta");
        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxSuccess, 1f);

        minigameCanvas.DOFade(0f, dtAnims.minigameUIAnimTime);
        uiContenedor.DOScale(Vector3.zero, dtAnims.minigameUIAnimTime).SetEase(Ease.InBack);

        secuenciaEmpaque = DOTween.Sequence();

        if (boxPivotActivo != null)
        {
            float mitadTiempo = dtAnims.boxFlipDuration / 2f;
            float posZOriginal = boxPivotActivo.localPosition.z;

            secuenciaEmpaque.Append(boxPivotActivo.DOLocalMoveZ(posZOriginal + dtAnims.boxFlipJumpPower, mitadTiempo).SetEase(Ease.OutQuad));
            secuenciaEmpaque.Append(boxPivotActivo.DOLocalMoveZ(posZOriginal, mitadTiempo).SetEase(Ease.InQuad));
            secuenciaEmpaque.Insert(0, boxPivotActivo.DOLocalRotate(rotacionCajaDePie, dtAnims.boxFlipDuration, RotateMode.LocalAxisAdd).SetEase(dtAnims.boxFlipEase));
            secuenciaEmpaque.AppendCallback(() => AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, 1f));
        }

        secuenciaEmpaque.AppendInterval(0.3f);

        secuenciaEmpaque.AppendCallback(() =>
        {
            AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxWoosh, 1f);
            if (tapasActivas != null)
            {
                foreach (var config in tapasActivas)
                {
                    if (config.bisagra != null)
                        config.bisagra.DOLocalRotate(config.rotacionCerrada, dtAnims.boxCloseTapasDuration).SetEase(dtAnims.boxCloseTapasEase);
                }
            }
        });

        secuenciaEmpaque.AppendInterval(dtAnims.boxCloseTapasDuration);

        secuenciaEmpaque.OnComplete(() =>
        {
            GameObject nuevaCaja = EjecutarPropSwap();
            if (nuevaCaja != null) nuevaCaja.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.3f, 1);

            PackingTableManager manager = FindFirstObjectByType<PackingTableManager>();
            if (manager != null) manager.FinalizarMinijuego();

            DOVirtual.DelayedCall(0.5f, () => CameraManager.Instance.ReturnToPlayerCamera());
        });
    }

    private GameObject EjecutarPropSwap()
    {
        if (productoFisicoActual != null && boxPivotActivo != null)
        {
            Vector3 posicionFinal = boxPivotActivo.position;
            Quaternion rotacionPropiaDelPrefab = prefabCajaEmpacadaReal.transform.rotation;

            Destroy(boxPivotActivo.gameObject);

            DropZoneHandler zonaPadre = productoFisicoActual.GetComponentInParent<DropZoneHandler>();
            if (zonaPadre != null) zonaPadre.FreeSlot();

            if (prefabCajaEmpacadaReal != null)
            {
                GameObject cajaReal = Instantiate(prefabCajaEmpacadaReal, posicionFinal + offsetAparicionCaja, rotacionPropiaDelPrefab);
                cajaReal.layer = LayerMask.NameToLayer("Interactable");

                Collider[] colliders = cajaReal.GetComponentsInChildren<Collider>();
                foreach (var col in colliders)
                {
                    col.enabled = true;
                    col.isTrigger = false;
                }

                Rigidbody rb = cajaReal.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;

                if (cajaReal.TryGetComponent<DeliverableItem>(out var deliverable))
                {
                    deliverable.itemData = productoFisicoActual.itemData;
                    deliverable.isPacked = true;
                    deliverable.isStored = productoFisicoActual.isStored;
                    deliverable.ChangeBoxState(DeliverableItem.BoxState.OnDropTable);
                }

                gameEvents.ItemPacked(productoFisicoActual.itemData.itemName);
                Destroy(productoFisicoActual.gameObject);

                return cajaReal;
            }
        }
        return null;
    }
}