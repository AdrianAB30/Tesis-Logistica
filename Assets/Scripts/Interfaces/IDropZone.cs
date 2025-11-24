using UnityEngine;

public interface IDropZone
{
    Transform DropTransform { get; }

    void ShowGhost();
    void HideGhost();

    bool CanDropItem(string itemTag);
}