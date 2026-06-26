using UnityEngine;
using TMPro;
using DG.Tweening;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance;

    [Header("UI Referencias")]
    [SerializeField] private CanvasGroup panelSubtitulosGroup;
    [SerializeField] private TMP_Text textoSubtitulos;
    [SerializeField] private DOTweenConfig dtConfig; 

    private Tween typingTween;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        panelSubtitulosGroup.alpha = 0f;
    }

    public void MostrarSubtitulo(string texto, float duracionAudio)
    {
        typingTween?.Kill();
        panelSubtitulosGroup.DOKill();
        panelSubtitulosGroup.transform.DOKill();

        panelSubtitulosGroup.alpha = 1f;
        panelSubtitulosGroup.transform.localScale = Vector3.one * 0.8f;
        panelSubtitulosGroup.transform.DOScale(Vector3.one, dtConfig.subtitleScaleTime).SetEase(dtConfig.subtitleScaleEase);

        textoSubtitulos.text = texto;
        textoSubtitulos.maxVisibleCharacters = 0;

        typingTween = DOTween.To(() => textoSubtitulos.maxVisibleCharacters,
                                 x => textoSubtitulos.maxVisibleCharacters = x,
                                 texto.Length,
                                 dtConfig.subtitleTypingDuration)
                             .SetEase(Ease.Linear);

        panelSubtitulosGroup.DOFade(0f, dtConfig.subtitleFadeOutTime).SetDelay(duracionAudio);
    }
}