using UnityEngine;
using System.Collections;

public enum DragEventType
{
    Over = 0,
    Down = 1,
    Up = 2,
    Exit = 3
}

[System.Serializable]
public partial class DragNDrop : MonoBehaviour
{
    public static DraggableCollider[] colliders;
    public static Draggable current;
    public static bool myActive;
    private DraggableCollider lastInsideCollider;
    private DraggableCollider downCollider;
    public virtual void Update()
    {
        RaycastHit hit = default(RaycastHit);
        Vector2 newSize = new Vector2(Screen.width, Screen.height);
        if (!DragNDrop.myActive)
        {
            return;
        }
        int i = 0;
        float oldY = Screen.height;
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y) * (newSize.y / oldY);
        DraggableCollider curCollider = null;
        i = 0;
        while (i < DragNDrop.colliders.Length)
        {
            if (((DragNDrop.colliders[i].guiRect.width != 0) && (!curCollider || (curCollider.guiZ <= DragNDrop.colliders[i].guiZ))) && DragNDrop.colliders[i].guiRect.Contains(mousePosition))
            {
                curCollider = DragNDrop.colliders[i];
            }
            i++;
        }
        if (!curCollider)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                curCollider = (DraggableCollider) hit.collider.gameObject.GetComponent(typeof(DraggableCollider));
            }
        }
        if (this.lastInsideCollider && (curCollider != this.lastInsideCollider))
        {
            this.lastInsideCollider.inside = false;
            DragNDrop.current = this.lastInsideCollider.parent.OnDragEvent(DragEventType.Exit, this.lastInsideCollider.id, DragNDrop.current, curCollider ? curCollider.parent : null);
        }
        if (curCollider)
        {
            if (!curCollider.inside)
            {
                curCollider.inside = true;
                DragNDrop.current = curCollider.parent.OnDragEvent(DragEventType.Over, curCollider.id, DragNDrop.current, curCollider.parent);
            }
            if (!curCollider.clicked && Input.GetMouseButtonDown(0))
            {
                curCollider.clicked = true;
                DragNDrop.current = curCollider.parent.OnDragEvent(DragEventType.Down, curCollider.id, DragNDrop.current, curCollider.parent);
                this.downCollider = curCollider;
            }
        }
        if (this.downCollider && Input.GetMouseButtonUp(0))
        {
            this.downCollider.clicked = false;
            DragNDrop.current = this.downCollider.parent.OnDragEvent(DragEventType.Up, this.downCollider.id, DragNDrop.current, curCollider ? curCollider.parent : null);
            this.downCollider = null;
        }
        this.lastInsideCollider = curCollider;
        if (DragNDrop.current)
        {
            DragNDrop.current.position = mousePosition;
        }
    }

    public static Vector3 ScreenToWorld(Vector2 screenP)
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.ScreenPointToRay(screenP).direction);
        return (ray.origin + (ray.direction * (ray.origin.z < 0 ? -ray.origin.z : ray.origin.z))) / ray.direction.z;
    }

    public static Vector2 WorldToScreen(Vector3 worldP)
    {
        Vector3 screenP = Camera.main.WorldToScreenPoint(worldP);
        return new Vector2(screenP.x, screenP.y);
    }

    public static void Add(DraggableCollider d)
    {
        DraggableCollider[] newDraggables = new DraggableCollider[DragNDrop.colliders.Length + 1];
        int i = 0;
        while (i < DragNDrop.colliders.Length)
        {
            newDraggables[i] = DragNDrop.colliders[i];
            i++;
        }
        newDraggables[DragNDrop.colliders.Length] = d;
        DragNDrop.colliders = newDraggables;
    }

    public static void Remove(DraggableCollider d)
    {
        DraggableCollider[] newDraggables = new DraggableCollider[DragNDrop.colliders.Length - 1];
        int added = 0;
        int i = 0;
        while (i < DragNDrop.colliders.Length)
        {
            if (DragNDrop.colliders[i] != d)
            {
                newDraggables[added] = DragNDrop.colliders[i];
                added++;
            }
            i++;
        }
        DragNDrop.colliders = newDraggables;
    }

    static DragNDrop()
    {
        DragNDrop.colliders = new DraggableCollider[0];
        DragNDrop.myActive = true;
    }

}