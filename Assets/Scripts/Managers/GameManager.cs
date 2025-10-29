using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject panelOptions;

    private void Start()
    {
        panelOptions.SetActive(false);
    }
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ChangeSettings()
    {
        panelOptions.SetActive(!panelOptions.activeSelf);
    }
    public void ExitApplication()
    {
        Application.Quit();
        Debug.Log("Saliendo del Simulador");
    }
}
