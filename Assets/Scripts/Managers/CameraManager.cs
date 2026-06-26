using UnityEngine;
using Unity.Cinemachine; 
using System.Collections;
using System;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Referencias Core")]
    [Tooltip("La cámara en primera persona que usa el jugador al caminar")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private InputReader inputReader;

    private CinemachineCamera currentActiveCamera;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentActiveCamera = playerCamera;
        SetCameraPriority(playerCamera, 20);
    }

    /// Llamar a este método pasándole la cámara del minijuego para iniciar la transición.
    public void SwitchToMinigameCamera(CinemachineCamera minigameCamera, Action onComplete = null)
    {
        inputReader.SetInputActive(false);

        if (currentActiveCamera != null)
        {
            SetCameraPriority(currentActiveCamera, 10);
        }

        currentActiveCamera = minigameCamera;
        SetCameraPriority(currentActiveCamera, 20);

        Debug.Log($"[CÁMARAS] Transición a la cámara: {minigameCamera.gameObject.name}");

        if (onComplete != null)
        {
            StartCoroutine(WaitForCameraBlend(onComplete));
        }
    }

    /// Llamar a este método al terminar el minijuego para devolverle el control al jugador.
    public void ReturnToPlayerCamera()
    {
        if (currentActiveCamera != playerCamera)
        {
            SetCameraPriority(currentActiveCamera, 10);
            currentActiveCamera = playerCamera;
            SetCameraPriority(currentActiveCamera, 20);
        }

        StartCoroutine(ReactivatePlayerInput());
    }

    private void SetCameraPriority(CinemachineCamera cam, int priority)
    {
        if (cam != null) cam.Priority = priority;
    }

    private IEnumerator ReactivatePlayerInput()
    {
        yield return new WaitForSeconds(1.5f);
        inputReader.SetInputActive(true);
        Debug.Log("[CÁMARAS] Control devuelto al jugador.");
    }
    private IEnumerator WaitForCameraBlend(Action onComplete)
    {
        yield return null;

        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();

        if (brain != null)
        {
            while (brain.IsBlending)
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        onComplete.Invoke();
    }
}