using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Settings")]
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private AudioMixer myAudioMixer;
    [SerializeField] private float fadeMusicTime;

    [Header("Audio Sources")]   
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource voiceSource;

    [Header("Base de Datos")]
    public AudioDatabase audioDB;

    private Slider masterSlider, musicSlider, sfxSlider, voiceSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject m = GameObject.Find("Master Slider");
        GameObject mu = GameObject.Find("Music Slider");
        GameObject s = GameObject.Find("Sfx Slider ");
        GameObject v = GameObject.Find("Voices Slider ");

        if (m)
        {
            masterSlider = m.GetComponent<Slider>();
            masterSlider.onValueChanged.RemoveAllListeners(); 
            masterSlider.onValueChanged.AddListener(delegate { SetMasterVolume(); });
        }

        if (mu)
        {
            musicSlider = mu.GetComponent<Slider>();
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        }

        if (s)
        {
            sfxSlider = s.GetComponent<Slider>();
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(delegate { SetSfxVolume(); });
        }

        if (v)
        {
            voiceSlider = v.GetComponent<Slider>();
            voiceSlider.onValueChanged.RemoveAllListeners();
            voiceSlider.onValueChanged.AddListener(delegate { SetVoiceVolume(); });
        }

        LoadVolume();
    }

    public void LoadVolume()
    {
        if (masterSlider) masterSlider.SetValueWithoutNotify(audioSettings.masterVolume);
        if (musicSlider) musicSlider.SetValueWithoutNotify(audioSettings.musicVolume);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(audioSettings.sfxVolume);
        if (voiceSlider) voiceSlider.SetValueWithoutNotify(audioSettings.voiceVolume);

        myAudioMixer.SetFloat("MasterVolume", Mathf.Log10(audioSettings.masterVolume) * 20);
        myAudioMixer.SetFloat("MusicVolume", Mathf.Log10(audioSettings.musicVolume) * 20);
        myAudioMixer.SetFloat("SfxVolume", Mathf.Log10(audioSettings.sfxVolume) * 20);
        myAudioMixer.SetFloat("VoiceVolume", Mathf.Log10(audioSettings.voiceVolume) * 20);
    }

    public void SetMasterVolume()
    {
        audioSettings.masterVolume = masterSlider.value;
        myAudioMixer.SetFloat("MasterVolume", Mathf.Log10(audioSettings.masterVolume) * 20);
    }

    public void SetMusicVolume()
    {
        audioSettings.musicVolume = musicSlider.value;
        myAudioMixer.SetFloat("MusicVolume", Mathf.Log10(audioSettings.musicVolume) * 20);
    }

    public void SetSfxVolume()
    {
        audioSettings.sfxVolume = sfxSlider.value;
        myAudioMixer.SetFloat("SfxVolume", Mathf.Log10(audioSettings.sfxVolume) * 20);
    }

    public void SetVoiceVolume()
    {
        audioSettings.voiceVolume = voiceSlider.value;
        myAudioMixer.SetFloat("VoiceVolume", Mathf.Log10(audioSettings.voiceVolume) * 20);
    }

    // MÉTODOS BASE DE REPRODUCCIÓN

    public void PlayVoice(AudioClip clip)
    {
        if (clip != null)
        {
            voiceSource.clip = clip;
            voiceSource.Play();
        }
    }

    public void StopVoice()
    {
        if (voiceSource.isPlaying)
        {
            voiceSource.Stop();
        }
    }

    public void PlayMusicWithFade(AudioClip newMusic, float targetVolume = 1f)
    {
        if (newMusic == null) return;

        if (musicSource.clip != null && musicSource.clip.name == newMusic.name)
        {
            if (musicSource.isPlaying) return;
        }

        StartCoroutine(FadeMusicCoroutine(newMusic, fadeMusicTime, targetVolume));
    }

    private IEnumerator FadeMusicCoroutine(AudioClip newMusic, float fadeTime, float finalVolume)
    {
        if (musicSource.isPlaying && musicSource.clip != null)
        {
            while (musicSource.volume > 0)
            {
                musicSource.volume -= Time.deltaTime / fadeTime;
                yield return null;
            }
        }
        else
        {
            musicSource.volume = 0f;
        }

        musicSource.clip = newMusic;
        musicSource.Play();

        while (musicSource.volume < finalVolume)
        {
            musicSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
        musicSource.volume = finalVolume;
    }
    public void PlayFromDB(AudioClip clip, float volume = 1f)
    {
        if (clip != null) PlaySFX(clip, volume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }
    public void PlayMenuMusic()
    {
        if (audioDB != null) PlayMusicWithFade(audioDB.menuMusic, audioDB.menuMusicVolume);
    }

    public void PlayGameMusic()
    {
        if (audioDB != null) PlayMusicWithFade(audioDB.gameMusic, audioDB.gameMusicVolume);
    }
  
    public void PlayIntroVoice(int index)
    {
        if (audioDB != null && index >= 0 && index < audioDB.introVoices.Length)
        {
            PlayVoice(audioDB.introVoices[index]);
        }
    }
    public void PlayTutorialVoice(int index)
    {
        if (audioDB != null && index >= 0 && index < audioDB.tutorialVoices.Length)
        {
            PlayVoice(audioDB.tutorialVoices[index]);
        }
    }

    public float GetIntroVoiceLength(int index)
    {
        if (audioDB != null && index >= 0 && index < audioDB.introVoices.Length && audioDB.introVoices[index] != null)
        {
            return audioDB.introVoices[index].length;
        }
        return 0f;
    }
    public float GetTutorialVoiceLength(int index)
    {
        if (audioDB != null && index >= 0 && index < audioDB.tutorialVoices.Length && audioDB.tutorialVoices[index] != null)
        {
            return audioDB.tutorialVoices[index].length;
        }
        return 0f;
    }
    public string GetIntroSubtitle(int index)
    {
        if (audioDB != null && index >= 0 && index < audioDB.introSubtitles.Length)
            return audioDB.introSubtitles[index];
        return "";
    }

    public string GetTutorialSubtitle(int index)
    {
        if (audioDB != null && index >= 0 && index < audioDB.tutorialSubtitles.Length)
            return audioDB.tutorialSubtitles[index];
        return "";
    }
}