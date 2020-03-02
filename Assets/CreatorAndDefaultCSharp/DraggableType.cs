using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class DraggableType : MonoBehaviour
{
    public virtual Draggable OnDragEvent(DragEventType type, int id, Draggable current, DraggableType target)
    {
        return current;
    }

    public virtual bool CanDrop(Draggable d)
    {
        return true;
    }

    public virtual void Drop(Draggable d)
    {
    }

}