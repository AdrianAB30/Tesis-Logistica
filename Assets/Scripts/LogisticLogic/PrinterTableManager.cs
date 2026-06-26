using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class PrinterTableManager : MonoBehaviour, IInteractable
{
    [Header("Transición Cinematográfica")]
    [SerializeField] private CinemachineCamera camaraMinijuegos;

    [Header("Referencias UI")]
    [SerializeField] private GameObject uiInteractuarF;

    [Header("Conexiones")]
    [SerializeField] private PrinterMinigame minijuegoImpresora;
    [SerializeField] private GameEvents gameEvents;

    private bool listoParaEtiquetar = false;
    private bool minijuegoEnCurso = false;
    private bool primeraVez = true;
    private DeliverableItem cajaEnMesa = null;

    private void Start()
    {
        if (uiInteractuarF != null) uiInteractuarF.SetActive(false);
    }

    public void PrepararMesaParaEtiquetado(DeliverableItem caja)
    {
        if (listoParaEtiquetar || minijuegoEnCurso) return;

        cajaEnMesa = caja;
        listoParaEtiquetar = true;

        if (uiInteractuarF != null) uiInteractuarF.SetActive(true);
        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, 1f);

        if (TutorialManager.Instance != null) TutorialManager.Instance.SetMesaEtiquetasLista();
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!listoParaEtiquetar) return;

        listoParaEtiquetar = false;
        minijuegoEnCurso = true;
        if (uiInteractuarF != null) uiInteractuarF.SetActive(false);

        InputReader input = interactor.GetComponent<InputReader>();
        if (input != null) input.SetInputActive(false);

        if (gameEvents != null) gameEvents.PrinterStarted();

        CameraManager.Instance.SwitchToMinigameCamera(camaraMinijuegos);

        float delayAudio = 0f;
        if (primeraVez && AudioManager.Instance != null)
        {
            try { delayAudio = AudioManager.Instance.GetTutorialVoiceLength(10); } catch { }
            primeraVez = false;
        }

        DOVirtual.DelayedCall(0.5f, () =>
        {
            if (minijuegoImpresora != null)
            {
                minijuegoImpresora.IniciarMinijuego(cajaEnMesa, delayAudio);
            }
        });
    }
    public void FinalizarMinijuego()
    {
        listoParaEtiquetar = false;
        minijuegoEnCurso = false;
        cajaEnMesa = null;

        InputReader input = FindFirstObjectByType<InputReader>();
        if (input != null) input.SetInputActive(true);

        if (uiInteractuarF != null) uiInteractuarF.SetActive(false);

        Debug.Log("[LOGÍSTICA] Mesa de etiquetas reiniciada para siguiente caja.");
    }
}