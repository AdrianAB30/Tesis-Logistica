using UnityEngine;
using Unity.Cinemachine;
using DG.Tweening;

public class PackingTableManager : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class ConfiguracionTapaMesa
    {
        [Tooltip("Escribe el nombre EXACTO de la bisagra (ej. Bisagra_1)")]
        public string nombreBisagra;
        public Vector3 rotacionCerrada;
    }

    [Header("Matemática de Volumen")]
    [SerializeField] private float capacidadCajaActualCm3 = 5000f;

    [Header("Transición Cinematográfica")]
    [SerializeField] private CinemachineCamera camaraMinijuegos;

    [Header("Referencias UI")]
    [SerializeField] private GameObject uiInteractuarF;

    [Header("FÁBRICA DE INSTANCIAS")]
    [SerializeField] private GameObject prefabBoxPivotCinematico;
    [SerializeField] private Transform puntoAnclajeMesa;

    [Header("Configuración de Cierre de Tapas (POR NOMBRE)")]
    [SerializeField] private ConfiguracionTapaMesa[] configuracionTapasBase;

    [Header("Conexiones")]
    [SerializeField] private TapeMinigame minijuegoCinta;
    [SerializeField] private GameEvents gameEvents;

    private bool listoParaEmpacar = false;
    private bool minijuegoEnCurso = false;
    private bool primeraVez = true;
    private DeliverableItem productoEnMesa = null;

    private void Start()
    {
        if (uiInteractuarF != null) uiInteractuarF.SetActive(false);
    }

    public void PrepararMesaParaEmpaque(DeliverableItem producto)
    {
        if (listoParaEmpacar || minijuegoEnCurso) return;

        productoEnMesa = producto;

        if (producto.itemData.volumenCm3 <= capacidadCajaActualCm3)
        {
            listoParaEmpacar = true;
            if (uiInteractuarF != null) uiInteractuarF.SetActive(true);
            AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, 1f);

            if (gameEvents != null) gameEvents.ItemStored("MesaLista");
            if (TutorialManager.Instance != null) TutorialManager.Instance.SetMesaEmpaqueLista();
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!listoParaEmpacar || prefabBoxPivotCinematico == null || puntoAnclajeMesa == null) return;

        listoParaEmpacar = false;
        minijuegoEnCurso = true;
        if (uiInteractuarF != null) uiInteractuarF.SetActive(false);

        InputReader input = interactor.GetComponent<InputReader>();
        if (input != null) input.SetInputActive(false);

        if (gameEvents != null) gameEvents.PackingStarted();

        CameraManager.Instance.SwitchToMinigameCamera(camaraMinijuegos);

        float delayAudio = 0f;
        if (primeraVez && AudioManager.Instance != null)
        {
            try { delayAudio = AudioManager.Instance.GetTutorialVoiceLength(7); } catch { }
            primeraVez = false;
        }

        GameObject nuevoClonCaja = Instantiate(prefabBoxPivotCinematico, puntoAnclajeMesa.position + (Vector3.up * 3f), puntoAnclajeMesa.rotation, null);

        TapeMinigame.TapaConfig[] tapasClonadas = VincularTapasDinamicas(nuevoClonCaja.transform);

        nuevoClonCaja.transform.DOMove(puntoAnclajeMesa.position, 0.6f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxWoosh, 1f);
            if (minijuegoCinta != null)
                minijuegoCinta.IniciarMinijuegoDinamico(productoEnMesa, nuevoClonCaja.transform, tapasClonadas, delayAudio);
        });
    }

    private TapeMinigame.TapaConfig[] VincularTapasDinamicas(Transform clonPadre)
    {
        TapeMinigame.TapaConfig[] configs = new TapeMinigame.TapaConfig[configuracionTapasBase.Length];

        for (int i = 0; i < configuracionTapasBase.Length; i++)
        {
            configs[i] = new TapeMinigame.TapaConfig();
            configs[i].rotacionCerrada = configuracionTapasBase[i].rotacionCerrada;

            if (!string.IsNullOrEmpty(configuracionTapasBase[i].nombreBisagra))
            {
                configs[i].bisagra = BuscarHijoPorNombre(clonPadre, configuracionTapasBase[i].nombreBisagra);
            }
        }
        return configs;
    }

    private Transform BuscarHijoPorNombre(Transform padre, string nombre)
    {
        if (padre.name == nombre) return padre;
        foreach (Transform hijo in padre)
        {
            Transform encontrado = BuscarHijoPorNombre(hijo, nombre);
            if (encontrado != null) return encontrado;
        }
        return null;
    }

    public void FinalizarMinijuego()
    {
        listoParaEmpacar = false;
        minijuegoEnCurso = false;
        if (uiInteractuarF != null) uiInteractuarF.SetActive(false);
    }
}