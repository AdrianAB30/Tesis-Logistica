using UnityEngine;

public interface IDropZone
{
    Transform DropTransform { get; }

    void ShowGhost();
    void HideGhost();

    bool CanDropItem(DeliverableItem item);

    void ReceiveItem(DeliverableItem item);
}