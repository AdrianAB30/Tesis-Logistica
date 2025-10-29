using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private AudioMixer myAudioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        LoadVolume();
    }
    public void LoadVolume()
    {
        masterSlider.value = audioSettings.masterVolume;
        musicSlider.value = audioSettings.musicVolume;
        sfxSlider.value = audioSettings.sfxVolume;
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
}