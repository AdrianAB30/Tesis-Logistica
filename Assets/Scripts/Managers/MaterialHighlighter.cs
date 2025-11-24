// --- MaterialHighlighter.cs ---
using UnityEngine;

public class MaterialHighlighter : MonoBehaviour, IHighlightable
{
    private Renderer objectRenderer;
    private Material originalMaterial;
    private Material highlightedMaterial;

    private Color highlightColor = Color.yellow; 
    private float highlightEmission = 1f;        

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError($"Renderer no encontrado en {gameObject.name}. El resaltado no funcionará.", this);
            return;
        }

        // 1. Guardar el material original y crear una instancia del material para manipular.
        originalMaterial = objectRenderer.material;

        // 2. Crear una copia del material original para el resaltado (para no modificar el asset original)
        highlightedMaterial = new Material(originalMaterial);

        // 3. Configurar el material resaltado
        highlightedMaterial.EnableKeyword("_EMISSION");
        highlightedMaterial.SetColor("_EmissionColor", highlightColor * highlightEmission);
    }

    public void Highlight()
    {
        // Activa el material de resaltado
        if (objectRenderer != null && objectRenderer.material != highlightedMaterial)
        {
            objectRenderer.material = highlightedMaterial;
        }
    }

    public void Unhighlight()
    {
        // Vuelve al material original
        if (objectRenderer != null && objectRenderer.material != originalMaterial)
        {
            objectRenderer.material = originalMaterial;
        }
    }

    private void OnDisable()
    {
        // Asegura que el material original se restaure si el objeto se desactiva
        if (objectRenderer != null && originalMaterial != null)
        {
            objectRenderer.material = originalMaterial;
        }
    }
}