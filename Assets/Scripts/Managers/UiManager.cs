using UnityEngine;
using DG.Tweening;
using TMPro;

public class UiManager : MonoBehaviour
{
    [Header("Secuencia Cinematográfica (Intro)")]
    [SerializeField] private TMP_Text textoBienvenida;
    [SerializeField] private RectTransform parpadoSuperior;
    [SerializeField] private RectTransform parpadoInferior;

    [Header("Elementos In-Game")]
    [SerializeField] private CanvasGroup grupoContador;

    [Header("Player Control")]
    [SerializeField] private InputReader inputReader;

    [Header("DOTween")]
    [SerializeField] private DOTweenConfig dgConfig;

    [Header("UI Feedback")]
    [SerializeField] private TMP_Text textoAlerta;
    [SerializeField] private TMP_Text textoObjetivo;

    [Header("Referencias de Eventos")]
    [SerializeField] private GameEvents gameEvents;

    private void OnEnable()
    {
        if (gameEvents != null) gameEvents.OnStarLost += PenalizarVisualmente;
    }

    private void OnDisable()
    {
        if (gameEvents != null) gameEvents.OnStarLost -= PenalizarVisualmente;
    }

    private void Start()
    {
        inputReader.SetInputActive(false); 

        textoBienvenida.rectTransform.localScale = Vector3.one * 5f;
        textoBienvenida.color = new Color(textoBienvenida.color.r, textoBienvenida.color.g, textoBienvenida.color.b, 0f);

        StartCinematicIntro();
    }

    private void StartCinematicIntro()
    {
        Sequence introSeq = DOTween.Sequence();

        introSeq.Append(textoBienvenida.DOFade(dgConfig.endValueFade, dgConfig.durationFade));
        introSeq.Join(textoBienvenida.rectTransform.DOScale(Vector3.one, dgConfig.textScaleTime).SetEase(dgConfig.textScaleEase))
                .OnStart(() =>
                {
                    AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxApertura, AudioManager.Instance.audioDB.sfxAperturaVolume );
                });

        introSeq.AppendInterval(dgConfig.textReadTime);
        introSeq.Append(textoBienvenida.DOFade(0f, dgConfig.textFadeOutTime));

        introSeq.Append(parpadoSuperior.DOAnchorPosY(dgConfig.eyelidMoveDistance, dgConfig.eyelidMoveTime).SetEase(dgConfig.eyelidMoveEase));
        introSeq.Join(parpadoInferior.DOAnchorPosY(-dgConfig.eyelidMoveDistance, dgConfig.eyelidMoveTime).SetEase(dgConfig.eyelidMoveEase));

        introSeq.OnComplete(() =>
        {
            inputReader.SetInputActive(true);

            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.IniciarTutorial();
            }
        });
    }
    public void AparecerContador()
    {
        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxContador, AudioManager.Instance.audioDB.sfxContadorVolume);
        grupoContador.alpha = 0;
        grupoContador.gameObject.SetActive(true);
        grupoContador.DOFade(1f, 1f);
    }
    private void PenalizarVisualmente(int estrellasRestantes)
    {
        grupoContador.transform.DOKill(); 
        grupoContador.transform.DOShakePosition(0.5f, 20f, 10, 90);

        textoAlerta.text = "¡EFICIENCIA BAJA!";
        textoAlerta.color = Color.red;
        textoAlerta.transform.localScale = Vector3.zero;
        textoAlerta.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        textoAlerta.DOFade(0f, 1f).SetDelay(1.5f);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxError, AudioManager.Instance.audioDB.sfxErrorVolume);
    }
    public void MostrarAvisoObjetivo()
    {
        textoObjetivo.alpha = 0f;

        textoObjetivo.DOFade(1f, 1f);

        textoObjetivo.transform.DOMoveY(textoObjetivo.transform.position.y + 20f, 2f);

        textoObjetivo.DOFade(0f, 1f).SetDelay(4f);
    }
}