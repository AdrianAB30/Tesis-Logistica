using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Application.targetFrameRate = 60;

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

    public void ChangeScene(string sceneName)
    {
        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, AudioManager.Instance.audioDB.sfxClickVolume);
        SceneManager.LoadScene(sceneName);
    }
    public void RestartScene()
    {
        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, AudioManager.Instance.audioDB.sfxClickVolume);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }
}
