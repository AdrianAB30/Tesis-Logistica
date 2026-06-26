using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class GarageDoorController : MonoBehaviour, IInteractable
{
    public enum EjeRotacion { X, Y, Z }

    [Header("Referencias de la Puerta")]
    [SerializeField] private Transform peldanosParent;
    [SerializeField] private Transform rodillo;
    [SerializeField] private Transform[] peldanosIndividuales;

    [Header("Configuración de Animación")]
    public float tiempoApertura = 2.5f;
    [SerializeField] private float distanciaApertura = 3.5f;
    [SerializeField] private int vueltasRodillo = 3;
    [Tooltip("Elige en qué eje debe girar el rodillo de ESTA puerta específica.")]
    [SerializeField] private EjeRotacion ejeDeGiroRodillo = EjeRotacion.X;

    [Header("Estado y Control")]
    [Tooltip("Si está bloqueada, el botón no hará nada hasta que el LevelManager la desbloquee.")]
    public bool bloqueada = false;

    [Header("Bloqueo de Paso")]
    [SerializeField] private GameObject muroInvisible;

    [Header("Feedback")]
    [SerializeField] private GameObject uiInteractuarF;

    [Header("Eventos")]
    public UnityEvent OnDoorOpened;

    private bool isOpen = false;
    private bool isAnimating = false;
    private float posInicialY;

    private void Start()
    {
        if (peldanosParent != null)
        {
            posInicialY = peldanosParent.position.y;
        }
        ActualizarUI();
    }

    public void DesbloquearPuerta()
    {
        bloqueada = false;
        ActualizarUI();
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (bloqueada || isAnimating) return;

        AbrirOCerrarPuerta();
    }

    public void AbrirOCerrarPuerta()
    {
        if (isAnimating) return;

        isAnimating = true;
        isOpen = !isOpen;

        //if (muroInvisible != null)
        //{
        //    muroInvisible.SetActive(isOpen);
        //}

        ActualizarUI();

        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxOpenGarage, AudioManager.Instance.audioDB.sfxOpenGarageVolume);

        float targetY = isOpen ? posInicialY + distanciaApertura : posInicialY;
        float giroAmount = isOpen ? 360f * vueltasRodillo : -360f * vueltasRodillo;

        Vector3 vectorGiro = Vector3.zero;
        if (ejeDeGiroRodillo == EjeRotacion.X) vectorGiro = new Vector3(giroAmount, 0, 0);
        else if (ejeDeGiroRodillo == EjeRotacion.Y) vectorGiro = new Vector3(0, giroAmount, 0);
        else if (ejeDeGiroRodillo == EjeRotacion.Z) vectorGiro = new Vector3(0, 0, giroAmount);

        Sequence seqPuerta = DOTween.Sequence();

        seqPuerta.Append(peldanosParent.DOMoveY(targetY, tiempoApertura).SetEase(Ease.InOutQuad));

        if (rodillo != null)
        {
            seqPuerta.Join(rodillo.DOLocalRotate(vectorGiro, tiempoApertura, RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuad));
        }

        seqPuerta.OnComplete(() =>
        {
            isAnimating = false;
            ActualizarVisibilidadPeldanos();
            ActualizarUI();

            if (isOpen) OnDoorOpened?.Invoke();
        });
    }

    private void Update()
    {
        if (isAnimating) ActualizarVisibilidadPeldanos();
    }

    private void ActualizarVisibilidadPeldanos()
    {
        if (rodillo == null || peldanosIndividuales.Length == 0) return;

        float alturaLimite = rodillo.position.y;

        foreach (Transform peldano in peldanosIndividuales)
        {
            if (peldano == null) continue;

            bool deberiaEstarVisible = peldano.position.y < alturaLimite;

            if (peldano.gameObject.activeSelf != deberiaEstarVisible)
            {
                peldano.gameObject.SetActive(deberiaEstarVisible);
            }
        }
    }

    private void ActualizarUI()
    {
        if (uiInteractuarF != null)
        {
            uiInteractuarF.SetActive(!bloqueada && !isAnimating && !isOpen);
        }
    }
}