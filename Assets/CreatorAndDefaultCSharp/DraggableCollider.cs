using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class DraggableCollider : MonoBehaviour
{
    public Rect guiRect;
    public float guiZ;
    public DraggableType parent;
    public int id;
    public bool inside;
    public bool clicked;
    public virtual void OnEnable()
    {
        DragNDrop.Add(this);
    }

    public virtual void OnDisable()
    {
        DragNDrop.Remove(this);
    }

}