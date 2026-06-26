using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioDatabase", menuName = "Scriptable Objects/Audio/Audio Database", order = 4)]
public class AudioDatabase : ScriptableObject
{
    [Header("Música (Music)")]
    public AudioClip menuMusic;
    [Range(0f, 1f)] public float menuMusicVolume;

    public AudioClip gameMusic;
    [Range(0f, 1f)] public float gameMusicVolume;

    [Header("Efectos de Sonido (SFX)")]
    public AudioClip sfxWoosh;
    [Range(0f, 1f)] public float sfxWooshVolume;

    public AudioClip sfxClick;
    [Range(0f, 1f)] public float sfxClickVolume = 1f;

    public AudioClip sfxSuccess;
    [Range(0f, 1f)] public float sfxSuccessVolume = 1f;

    public AudioClip sfxError;
    [Range(0f, 1f)] public float sfxErrorVolume = 1f;

    public AudioClip sfxDoor;
    [Range(0f, 1f)] public float sfxDoorVolume = 1f;

    public AudioClip sfxOpenGarage;
    [Range(0f, 1f)] public float sfxOpenGarageVolume = 1f;

    public AudioClip sfxContador;
    [Range(0f, 1f)] public float sfxContadorVolume = 1f;

    [Header("Efectos de Sonido Camion (SFX)")]
    public AudioClip sfxTruckReverse;
    [Range(0f, 1f)] public float sfxTruckReverseVolume = 1f;

    public AudioClip sfxAirBrakes;
    [Range(0f, 1f)] public float sfxAirBrakesVolume = 1f;

    public AudioClip sfxApertura;
    [Range(0f, 1f)] public float sfxAperturaVolume = 1f;


    [Header("Voces (Voices)")]
    public AudioClip[] introVoices;
    public AudioClip[] tutorialVoices;

    [Header("Subtítulos (Subtitles)")]
    [TextArea(2, 4)] public string[] introSubtitles;
    [TextArea(2, 4)] public string[] tutorialSubtitles;
}
