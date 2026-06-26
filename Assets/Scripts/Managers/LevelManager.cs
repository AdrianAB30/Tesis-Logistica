using UnityEngine;
using DG.Tweening;
using TMPro; 
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Conexión de Eventos")]
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private UiManager uiManager;

    [Header("Animación de Llegada")]
    [SerializeField] private GarageDoorController controladorPuertaGaraje;
    [SerializeField] private GarageDoorController puertaDespacho;
    [SerializeField] private Transform camion;
    [SerializeField] private Transform puntoLlegadaCamion;
    [SerializeField] private TruckAnimator truckAnim;

    [Header("Mercancía")]
    [SerializeField] private Transform cajasParaAparecer;

    [Header("Animación (DOTween)")]
    [SerializeField] private DOTweenConfig dtConfig;

    [Header("--- SISTEMA DE EVALUACIÓN ---")]
    [SerializeField] private CanvasGroup panelResultados;
    [SerializeField] private TMP_Text textoTiempo;
    [SerializeField] private TMP_Text textoErrores;
    [SerializeField] private Image[] estrellas;
    [SerializeField] private Color colorEstrellaEncendida = Color.yellow;
    [SerializeField] private TMP_Text textoContador;

    [Header("--- BALANCE DE DIFICULTAD ---")]
    [Tooltip("Tiempo máximo en segundos para no perder estrellas (Ej: 300 = 5 minutos)")]
    public float limiteTiempoTresEstrellas = 300f;
    [Tooltip("Tiempo máximo antes de perder la segunda estrella (Ej: 480 = 8 minutos)")]
    public float limiteTiempoDosEstrellas = 480f;

    private bool camionEsperandoAfuera = false;

    private float tiempoInicio;
    private int cantidadErrores = 0;
    private bool nivelTerminado = false;

    private bool cronometroActivo = false;
    private int estrellasActuales = 3;

    private void OnEnable()
    {
        if (gameEvents == null) return;
        gameEvents.OnOrderReceived += PrepararLlegadaCamion;
        gameEvents.OnItemLabeled += DesbloquearSalida;
        gameEvents.OnMistakeMade += RegistrarError;
    }
    private void OnDisable()
    {
        if (gameEvents == null) return;
        gameEvents.OnOrderReceived -= PrepararLlegadaCamion;
        gameEvents.OnItemLabeled -= DesbloquearSalida;
        gameEvents.OnMistakeMade -= RegistrarError;
    }

    private void Start()
    {
        AudioManager.Instance.PlayGameMusic();
        if (cajasParaAparecer != null) cajasParaAparecer.gameObject.SetActive(false);

        if (panelResultados != null)
        {
            panelResultados.alpha = 0f;
            panelResultados.gameObject.SetActive(true);
        }
    }
    private void Update()
    {
        if (!cronometroActivo) return;

        float tiempoTranscurrido = Time.time - tiempoInicio;

        int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60F);
        int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60F);
        textoContador.text = string.Format("Tiempo: {0:00}:{1:00}", minutos, segundos);

        if (estrellasActuales == 3 && tiempoTranscurrido > limiteTiempoTresEstrellas)
        {
            estrellasActuales = 2;
            gameEvents.StarLost(estrellasActuales);
            textoContador.color = Color.red; 
        }
        else if (estrellasActuales == 2 && tiempoTranscurrido > limiteTiempoDosEstrellas)
        {
            estrellasActuales = 1;
            gameEvents.StarLost(estrellasActuales);
        }
    }

    private void RegistrarError()
    {
        if (!nivelTerminado) cantidadErrores++;
    }

    private void PrepararLlegadaCamion(PedidosData pedido)
    {
        camionEsperandoAfuera = true;
        if (controladorPuertaGaraje != null) controladorPuertaGaraje.DesbloquearPuerta();
    }

    private void DesbloquearSalida(string itemName)
    {
        if (puertaDespacho != null) puertaDespacho.DesbloquearPuerta();
    }

    public void HacerEntrarCamion()
    {
        if (!camionEsperandoAfuera) return;

        camionEsperandoAfuera = false;
        Sequence secuenciaLlegada = DOTween.Sequence();

        secuenciaLlegada.AppendCallback(() =>
        {
            camion.gameObject.SetActive(true);
            AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxTruckReverse, AudioManager.Instance.audioDB.sfxTruckReverseVolume);
            truckAnim.LlegadaDeCamion(puntoLlegadaCamion.position);
        });

        secuenciaLlegada.AppendInterval(dtConfig.truckShakeTime);
        secuenciaLlegada.OnComplete(() =>
        {
            cajasParaAparecer.gameObject.SetActive(true);

            foreach (Transform caja in cajasParaAparecer)
            {
                Vector3 escalaOriginal = caja.localScale;

                caja.localScale = Vector3.one * 0.01f;

                caja.DOScale(escalaOriginal, dtConfig.boxPopTime).SetEase(dtConfig.boxPopEase);
            }
        });
    }
    public void IniciarTemporizador()
    {
        tiempoInicio = Time.time;
        cronometroActivo = true;
        uiManager.AparecerContador();
        uiManager.MostrarAvisoObjetivo();
    }
    public void PausarTemporizador()
    {
        cronometroActivo = false;
    }

    public void CompleteLevel()
    {
        if (nivelTerminado) return;
        PausarTemporizador();

        nivelTerminado = true;
        panelResultados.gameObject.SetActive(true);
        if (inputReader != null) inputReader.SetInputActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        float tiempoTotal = Time.time - tiempoInicio;
        int minutos = Mathf.FloorToInt(tiempoTotal / 60F);
        int segundos = Mathf.FloorToInt(tiempoTotal - minutos * 60);
        string tiempoFormateado = string.Format("{0:00}:{1:00}", minutos, segundos);

        int estrellasGanadas = 3;
        if (tiempoTotal > limiteTiempoDosEstrellas) estrellasGanadas -= 2;
        else if (tiempoTotal > limiteTiempoTresEstrellas) estrellasGanadas -= 1;

        if (cantidadErrores == 1 || cantidadErrores == 2) estrellasGanadas--;
        if (cantidadErrores >= 3) estrellasGanadas = 0; 

        estrellasGanadas = Mathf.Clamp(estrellasGanadas, 0, 3);

        if (panelResultados != null)
        {
            panelResultados.gameObject.SetActive(true);
            if (textoTiempo != null) textoTiempo.text = tiempoFormateado;
            if (textoErrores != null) textoErrores.text = cantidadErrores.ToString();

            Sequence seqFinal = DOTween.Sequence();
            seqFinal.Append(panelResultados.DOFade(1f, 0.5f));

            for (int i = 0; i < estrellasGanadas; i++)
            {
                if (i < estrellas.Length && estrellas[i] != null)
                {
                    Image estrella = estrellas[i];
                    seqFinal.Append(estrella.DOColor(colorEstrellaEncendida, dtConfig.starFadeDuration));
                    seqFinal.Join(estrella.transform.DOPunchScale(dtConfig.starPunchScale, dtConfig.starPunchDuration, dtConfig.starPunchVibrato));
                    seqFinal.AppendCallback(() => AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxSuccess, 1f));
                    seqFinal.AppendInterval(dtConfig.starInterval);
                }
            }
        }
    }
}