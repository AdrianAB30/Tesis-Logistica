using UnityEngine;

public class DeliverableItem : MonoBehaviour
{
    [SerializeField] private GameEvents gameEvents;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Despacho"))
        {
            gameEvents.ItemDelivered(gameObject.tag);
            Debug.Log("Caja entregada en despacho: " + gameObject.tag);

            this.gameObject.SetActive(false);
        }
    }
}
