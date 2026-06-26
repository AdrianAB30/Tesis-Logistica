using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Cinemática de Cámara")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform posMenuPrincipal;
    [SerializeField] private Transform posMonitor;
    [SerializeField] private float tiempoViaje = 1.5f;

    [Header("Paneles UI (Screen Space vs World Space)")]
    [SerializeField] private GameObject panelBotonesPrincipales;

    private void Start()
    {
        if (mainCamera != null && posMenuPrincipal != null)
        {
            mainCamera.position = posMenuPrincipal.position;
            mainCamera.rotation = posMenuPrincipal.rotation;
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }
    }

    public void IrAOpciones()
    {
        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, AudioManager.Instance.audioDB.sfxClickVolume);

        panelBotonesPrincipales.SetActive(false);

        mainCamera.DOMove(posMonitor.position, tiempoViaje).SetEase(Ease.InOutCubic);
        mainCamera.DORotate(posMonitor.eulerAngles, tiempoViaje).SetEase(Ease.InOutCubic);
    }

    public void VolverAlMenuPrincipal()
    {
        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, AudioManager.Instance.audioDB.sfxClickVolume);

        mainCamera.DOMove(posMenuPrincipal.position, tiempoViaje).SetEase(Ease.InOutCubic);
        mainCamera.DORotate(posMenuPrincipal.eulerAngles, tiempoViaje).SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                panelBotonesPrincipales.SetActive(true);
            });
    }
    public void CambiarCalidadGrafica(int indiceCalidad)
    {
        QualitySettings.SetQualityLevel(indiceCalidad);
        Debug.Log("Calidad gráfica cambiada al nivel: " + indiceCalidad);
    }
    public void ExitGame()
    {
        AudioManager.Instance.PlayFromDB(AudioManager.Instance.audioDB.sfxClick, AudioManager.Instance.audioDB.sfxClickVolume);

        Application.Quit();
        Debug.Log("Salir del juego");
    }

}
