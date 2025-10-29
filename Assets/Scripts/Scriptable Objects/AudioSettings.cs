using UnityEngine;

[CreateAssetMenu(fileName = "Audio Settings", menuName = "Scriptable Objects/Audio Settings", order = 4)]
public class AudioSettings : ScriptableObject
{
    public float masterVolume = 1;
    public float musicVolume = 1;
    public float sfxVolume = 1;
}