// --- DropZoneHandler.cs ---
using UnityEngine;

public class DropZoneHandler : MonoBehaviour, IDropZone
{
    public Transform DropTransform => transform;

    [Header("Configuración de Zona")]
    [Tooltip("Tag del item que esta zona acepta. (Ej: 'CajaA')")]
    public string requiredItemTag = "DefaultItem";

    [Header("Referencias")]
    [Tooltip("El GameObject que actúa como el 'fantasma' (Ghost) de la caja.")]
    [SerializeField] private GameObject ghostObject;


    private void Start()
    {
        if (ghostObject == null)
        {
            Debug.LogError($"El objeto fantasma (Ghost) no está asignado en {gameObject.name}.", this);
        }
        HideGhost();
    }

    public void ShowGhost()
    {
        if (ghostObject != null && !ghostObject.activeSelf)
        {
            // Opcional: Cambiar el material del fantasma a transparente/azul
            ghostObject.SetActive(true);
        }
    }

    public void HideGhost()
    {
        if (ghostObject != null && ghostObject.activeSelf)
        {
            ghostObject.SetActive(false);
        }
    }

    // Lógica para el Nivel 1: solo checkea el tag.
    public bool CanDropItem(string itemTag)
    {
        return itemTag == requiredItemTag;
    }
}