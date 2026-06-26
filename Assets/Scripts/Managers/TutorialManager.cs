using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Referencias")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private GameObject puntoAparicionCajas;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private LevelManager levelManager;

    [Header("Flechas de Estaciones")]
    [Tooltip("Orden: 0=Escritorio, 1=Estante, 2=Empaque, 3=Etiquetas, 4=Despacho")]
    [SerializeField] private GameObject[] flechasEstaciones;

    [Header("DOTween (Animaciones)")]
    [SerializeField] private DOTweenConfig dtConfig;

    [Header("--- CONFIGURACIÓN DEL RETO ---")]
    public int cajasObjetivo = 3;
    private int cajasDespachadas = 0;

    private bool haSellado = false;
    private bool cajaAlmacenada = false;
    private bool engranajeRecogido = false;

    // -- BOOLEANOS DE ESTADO (Mesa preparada vs Minijuego iniciado) --
    private bool mesaEmpaqueLista = false;
    private bool mesaEtiquetasLista = false;

    private bool empaqueIniciado = false;
    private bool etiquetadoIniciado = false;

    private bool cajaEmpacada = false;
    private bool cajaEtiquetada = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        ApagarTodasLasFlechas();
    }

    private void OnEnable()
    {
        gameEvents.OnOrderReceived += (pedido) => haSellado = true;
        gameEvents.OnItemStored += (item) => cajaAlmacenada = true;
        gameEvents.OnItemPicked += (item) => engranajeRecogido = true;

        gameEvents.OnPackingStarted += () => empaqueIniciado = true;
        gameEvents.OnPrinterStarted += () => etiquetadoIniciado = true;

        gameEvents.OnItemPacked += (item) => cajaEmpacada = true;
        gameEvents.OnItemLabeled += (item) => cajaEtiquetada = true;
        gameEvents.OnItemDelivered += (item) => cajasDespachadas++;
    }

    // --- NUEVOS MÉTODOS PARA AVISAR DESDE LAS MESAS ---
    public void SetMesaEmpaqueLista() => mesaEmpaqueLista = true;
    public void SetMesaEtiquetasLista() => mesaEtiquetasLista = true;

    public void IniciarTutorial()
    {
        StartCoroutine(RutinaTutorial());
    }

    private void ReproducirVozYSubtitulo(int index)
    {
        float duracion = 3f; 

        if (AudioManager.Instance != null && AudioManager.Instance.audioDB != null)
        {
            try { duracion = AudioManager.Instance.GetTutorialVoiceLength(index); }
            catch { Debug.LogWarning($"[TUTORIAL] Cuidado: Falta el archivo de audio en el índice {index}."); }
        }

        string texto = AudioManager.Instance.GetTutorialSubtitle(index);
        AudioManager.Instance.PlayTutorialVoice(index);

        if (SubtitleManager.Instance != null)
        {
            SubtitleManager.Instance.MostrarSubtitulo(texto, duracion);
        }
    }

    private IEnumerator RutinaTutorial()
    {
        if (inputReader != null) inputReader.SetInputActive(false);
        ReproducirVozYSubtitulo(0);
        yield return new WaitForSeconds(AudioManager.Instance.GetTutorialVoiceLength(0) + 0.5f);

        ReproducirVozYSubtitulo(1);
        yield return new WaitForSeconds(AudioManager.Instance.GetTutorialVoiceLength(1));
        if (inputReader != null) inputReader.SetInputActive(true);

        Vector3 posInicial = playerTransform.position;
        yield return new WaitUntil(() => Vector3.Distance(posInicial, playerTransform.position) > 3f);

        // 2. SELLO
        ReproducirVozYSubtitulo(2);
        ActivarFlecha(0);
        yield return new WaitUntil(() => haSellado);
        ApagarFlecha(0);

        // 3. CAMIÓN
        ReproducirVozYSubtitulo(3);
        if (puntoAparicionCajas != null) yield return new WaitUntil(() => puntoAparicionCajas.activeInHierarchy);
        else yield return new WaitForSeconds(AudioManager.Instance.GetTutorialVoiceLength(3) + 2f);

        // 4. ALMACENAMIENTO
        ReproducirVozYSubtitulo(4);
        ActivarFlecha(1);
        yield return new WaitUntil(() => cajaAlmacenada);
        ApagarFlecha(1);

        // 5. PICKING
        ReproducirVozYSubtitulo(5);
        yield return new WaitUntil(() => engranajeRecogido);

        // 6. LLEVAR A EMPAQUE
        ReproducirVozYSubtitulo(6);
        ActivarFlecha(2);
        yield return new WaitUntil(() => mesaEmpaqueLista);
        ApagarFlecha(2);
        yield return new WaitUntil(() => empaqueIniciado);

        // 7 Y 8. MINIJUEGO CINTA
        ReproducirVozYSubtitulo(7);
        yield return new WaitForSeconds(AudioManager.Instance.GetTutorialVoiceLength(7) + 0.2f);
        ReproducirVozYSubtitulo(8);
        yield return new WaitUntil(() => cajaEmpacada);

        // 9. LLEVAR A ETIQUETAS
        ReproducirVozYSubtitulo(9);
        ActivarFlecha(3);
        yield return new WaitUntil(() => mesaEtiquetasLista);
        ApagarFlecha(3);
        yield return new WaitUntil(() => etiquetadoIniciado);

        // 10 Y 11. MINIJUEGO IMPRESORA
        ReproducirVozYSubtitulo(10);
        float espera10 = 3f;
        try { espera10 = AudioManager.Instance.GetTutorialVoiceLength(10) + 0.2f; } catch { }
        yield return new WaitForSeconds(espera10);

        ReproducirVozYSubtitulo(11);
        yield return new WaitUntil(() => cajaEtiquetada);

        ReproducirVozYSubtitulo(12);
        ActivarFlecha(4);

        yield return new WaitUntil(() => cajasDespachadas >= 1);

        // 13. DESPACHO 
        ReproducirVozYSubtitulo(13);
        inputReader.SetInputActive(false);

        float duracionVoz13 = AudioManager.Instance.GetTutorialVoiceLength(13);
        yield return new WaitForSeconds(duracionVoz13);

        levelManager.IniciarTemporizador();
        inputReader.SetInputActive(true);

        yield return new WaitUntil(() => cajasDespachadas >= cajasObjetivo);
        ApagarFlecha(4);

        // 14. FINAL (Victoria)
        ReproducirVozYSubtitulo(14);
    }

    private void ActivarFlecha(int index)
    {
        if (flechasEstaciones == null || index < 0 || index >= flechasEstaciones.Length || dtConfig == null) return;
        GameObject flecha = flechasEstaciones[index];
        if (flecha == null) return;

        flecha.SetActive(true);
        flecha.transform.DOKill();

        float startY = flecha.transform.localPosition.y;
        flecha.transform.DOLocalMoveY(startY + dtConfig.arrowMoveDistance, dtConfig.arrowMoveTime)
                  .SetLoops(-1, LoopType.Yoyo)
                  .SetEase(dtConfig.arrowMoveEase);
    }

    private void ApagarFlecha(int index)
    {
        if (flechasEstaciones == null || index < 0 || index >= flechasEstaciones.Length) return;
        GameObject flecha = flechasEstaciones[index];
        if (flecha != null)
        {
            flecha.transform.DOKill();
            flecha.SetActive(false);
        }
    }

    private void ApagarTodasLasFlechas()
    {
        if (flechasEstaciones == null) return;
        for (int i = 0; i < flechasEstaciones.Length; i++) ApagarFlecha(i);
    }
}