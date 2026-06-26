using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    [SerializeField] private DOTweenConfig config;
    private Image fadeImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else { Destroy(gameObject); }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject faderObj = GameObject.FindGameObjectWithTag("Fader");
        if (faderObj != null)
        {
            fadeImage = faderObj.GetComponent<Image>();
            fadeImage.color = Color.black;
            fadeImage.gameObject.SetActive(true);
        }
    }
    public void PlayFade(bool fadeIn, Action onComplete = null)
    {
        if (fadeImage == null) return;

        fadeImage.gameObject.SetActive(true);
        float duration = config.simpleFadeTime;
        float startAlpha = fadeIn ? 1f : 0f;
        float endAlpha = fadeIn ? 0f : 1f;

        fadeImage.color = new Color(0, 0, 0, startAlpha);

        fadeImage.DOFade(endAlpha, duration)
            .SetEase(config.sceneTransitionEase)
            .OnComplete(() => {
                if (!fadeIn) onComplete?.Invoke();
                else fadeImage.gameObject.SetActive(false);
            });
    }
}