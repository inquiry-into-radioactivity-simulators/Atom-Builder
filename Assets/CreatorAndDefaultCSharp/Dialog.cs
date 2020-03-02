using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Dialog : MonoBehaviour
{
    public Vector2 size;
    public RectOffset border;
    private GUIStyle bgStyle1;
    private GUIStyle bgStyle2;
    public virtual void Awake()
    {
        this.bgStyle1 = new GUIStyle();
        this.bgStyle1.normal.background = (Texture2D)Resources.Load("Tray_TIF");
        this.bgStyle1.border = this.border;
        this.bgStyle2 = new GUIStyle();
        this.bgStyle2.normal.background =  (Texture2D)Resources.Load("BlackShadow_TIF");
        this.bgStyle2.border = this.border;
    }

    public virtual void OnEnable()
    {
        DragNDrop.myActive = false;
    }

    public virtual void OnDisable()
    {
        DragNDrop.myActive = true;
    }

    public virtual void OnGUI()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Rect rect = new Rect((screenSize.x * 0.5f) - (this.size.x * 0.5f), (screenSize.y * 0.5f) - (this.size.y * 0.5f), this.size.x, this.size.y);
        GUI.depth = -10;
        GUI.Label(new Rect(-10, -10, screenSize.x + 20, screenSize.y + 20), "", this.bgStyle2);
        GUI.Label(this.border.Add(rect), "", this.bgStyle1);
        GUI.BeginGroup(rect);
        this.Content();
        GUI.EndGroup();
    }

    public virtual void Content()
    {
    }

    public Dialog()
    {
        this.size = Vector2.zero;
    }

}