using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private DOTweenConfig animConfig;
    [SerializeField] private RectTransform slidesHolder;
    [SerializeField] private Image fader;

    [SerializeField] private int cantidadDeDiapositivas = 5;

    [Header("Skip Settings")]
    [SerializeField] private CanvasGroup skipTextGroup;
    [SerializeField] private float skipTextDuration = 2f;

    private bool isSkipping = false;
    private bool canSkip = true;
    private Coroutine introCoroutine;

    void Start()
    {
        skipTextGroup.alpha = 0;
        skipTextGroup.DOFade(1, animConfig.blinkTime).SetLoops(-1, LoopType.Yoyo).SetEase(animConfig.blinkEase);

        FadeManager.Instance.PlayFade(true);

        StartCoroutine(ManageSkipTextVisibility());
        introCoroutine = StartCoroutine(SequenceIntro());

        AudioManager.Instance.PlayMenuMusic();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canSkip && !isSkipping)
        {
            StartCoroutine(SkipIntro());
        }
    }

    IEnumerator ManageSkipTextVisibility()
    {
        yield return new WaitForSeconds(skipTextDuration);

        canSkip = false;
        skipTextGroup.DOKill();
        skipTextGroup.DOFade(0, 1f);
    }

    IEnumerator SequenceIntro()
    {
        for (int i = 0; i < cantidadDeDiapositivas; i++)
        {
            if (i > 0)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.audioDB.sfxWoosh, AudioManager.Instance.audioDB.sfxWooshVolume);
            }

            slidesHolder.DOAnchorPosX(-i * 1920f, animConfig.introPanelTime)
                        .SetEase(animConfig.introPanelEase);

            AudioManager.Instance.PlayIntroVoice(i);
            float duracionVoz = AudioManager.Instance.GetIntroVoiceLength(i);
            string textoSubtitulo = AudioManager.Instance.GetIntroSubtitle(i);

            if (SubtitleManager.Instance != null && !string.IsNullOrEmpty(textoSubtitulo))
            {
                SubtitleManager.Instance.MostrarSubtitulo(textoSubtitulo, duracionVoz);
            }

            yield return new WaitForSeconds(duracionVoz + 0.5f);
        }

        FadeManager.Instance.PlayFade(false, () => SceneManager.LoadScene("Menu"));
    }

    IEnumerator SkipIntro()
    {
        if (isSkipping) yield break;
        isSkipping = true;

        AudioManager.Instance.StopVoice();

        if (introCoroutine != null) StopCoroutine(introCoroutine);
        skipTextGroup.DOKill();
        skipTextGroup.alpha = 0;

        if (SubtitleManager.Instance != null)
        {
            SubtitleManager.Instance.MostrarSubtitulo("", 0f);
        }

        FadeManager.Instance.PlayFade(false, () => SceneManager.LoadScene("Menu"));
    }
}