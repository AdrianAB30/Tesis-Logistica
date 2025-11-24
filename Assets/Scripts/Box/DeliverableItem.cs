using UnityEngine;

public class DeliverableItem : MonoBehaviour
{
    [SerializeField] private GameEvents gameEvents;

    [HideInInspector] public bool isPicked = false;
    [HideInInspector] public bool isPacked = false;

    public void ItemPicked()
    {
        if (!isPicked)
        {
            isPicked = true;
            gameEvents.ItemPicked(gameObject.tag);
            Debug.Log("Picking completado para: " + gameObject.tag);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. LÓGICA DE PACKING (Acondicionamiento)
        // El jugador debe llevar el ítem (isPicked = true) a la estación de empaque.
        if (other.CompareTag("EstacionEmpaque") && isPicked && !isPacked)
        {
            // NOTA: Asumiendo que el jugador DEBE soltar el ítem para que entre
            // en el Trigger de la estación de empaque.

            isPacked = true;
            gameEvents.ItemPacked(gameObject.tag);
            Debug.Log("Caja acondicionada/empacada: " + gameObject.tag);
        }

        // 2. LÓGICA DE DESPACHO (Entrega)
        // Solo se puede entregar si está empacado.
        else if (other.CompareTag("Despacho") && isPacked)
        {
            gameEvents.ItemDelivered(gameObject.tag);
            Debug.Log("Caja entregada en despacho: " + gameObject.tag);

            // Desactivamos el objeto solo si la entrega fue exitosa
            this.gameObject.SetActive(false);
        }

        // 3. (OPCIONAL) Feedback de error si intenta entregar sin empacar
        else if (other.CompareTag("Despacho") && isPicked && !isPacked)
        {
            // Podrías lanzar un evento de error para que la UI muestre un pop-up:
            // gameEvents.ShowError("¡Error! Debes empacar el ítem en la Estación antes de despachar.");
            Debug.LogWarning("Error logístico: ¡El ítem " + gameObject.tag + " no ha sido empacado!");
        }
    }
}