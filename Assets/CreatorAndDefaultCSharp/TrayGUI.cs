using UnityEngine;
using System.Collections;

[System.Serializable]
public class TraySlot : object
{
    public DraggableCollider collider;
    public PType type;
}
[System.Serializable]
public class TrayGUI : DraggableType
{
    public Texture2D texture;
    public Vector2 dimensions;
    public Vector2 itemSize;
    public Vector2 position;
    public TraySlot[] contents;
    public Texture2D[] particleTextures;
    public Vector2 childOffset;
    public int border;
    public int[] acceptTypes;
    private GUIStyle style;
    private Color color;
    private int mouseDown;
    private int mouseOver;
    public virtual void Start()
    {
        this.particleTextures = new Texture2D[3];
        this.particleTextures[0] = (Texture2D)Resources.Load("Proton_TIF");
        this.particleTextures[1] = (Texture2D)Resources.Load("Neutron_TIF");
        this.particleTextures[2] = (Texture2D)Resources.Load("Electron_TIF");
        if (!this.texture)
        {
            this.texture = (Texture2D)Resources.Load("Tray_TIF");
        }
        this.style = new GUIStyle();
        this.style.normal.background = this.texture;
        this.style.border.left = this.style.border.right = this.style.border.top = this.style.border.bottom = this.border;
        this.contents = new TraySlot[Mathf.RoundToInt(this.dimensions.x * this.dimensions.y)];
        int i = 0;
        while (i < this.contents.Length)
        {
            TraySlot s = new TraySlot();
            s.type = PType.Empty;
            s.collider = (DraggableCollider) this.gameObject.AddComponent(typeof(DraggableCollider));
            s.collider.id = i;
            s.collider.parent = this;
            this.contents[i] = s;
            i++;
        }
    }

    public override Draggable OnDragEvent(DragEventType type, int id, Draggable current, DraggableType target)
    {
        if (type == DragEventType.Over)
        {
            this.mouseOver = id;
        }
        else
        {
            if (type == DragEventType.Down)
            {
                if (this.contents[id].type != PType.Empty)
                {
                    GameObject obj = new GameObject("Draggable");
                    Draggable_Nucleon dr = (Draggable_Nucleon) obj.AddComponent(typeof(Draggable_Nucleon));
                    dr.type = this.contents[id].type;
                    this.contents[id].type = PType.Empty;
                    this.mouseDown = id;
                    return dr;
                }
            }
            else
            {
                if (type == DragEventType.Up)
                {
                    if (target && target.CanDrop(current))
                    {
                        target.Drop(current);
                        current.Die();
                    }
                    else
                    {
                        if (current)
                        {
                            current.FailDrag();
                            this.Drop(current);
                        }
                    }
                    this.mouseDown = -1;
                    return null;
                }
                else
                {
                    if (type == DragEventType.Exit)
                    {
                        this.mouseOver = -1;
                    }
                }
            }
        }
        return current;
    }

    public override bool CanDrop(Draggable d)
    {
        bool space = false;
        Draggable_Nucleon nuc = d as Draggable_Nucleon;
        if (nuc && (this.acceptTypes[((int) (nuc.type - 1))] != 0))
        {
            int i = 0;
            while (i < this.contents.Length)
            {
                if (this.contents[i].type == PType.Empty)
                {
                    space = true;
                    i = this.contents.Length;
                }
                i++;
            }
        }
        return space;
    }

    public override void Drop(Draggable d)
    {
        Draggable_Nucleon nuc = d as Draggable_Nucleon;
        int i = this.contents.Length - 1;
        while (i >= 0)
        {
            if (this.contents[i].type == PType.Empty)
            {
                this.contents[i].type = nuc.type;
                i = -1;
            }
            i--;
        }
    }

    public virtual void OnGUI()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 s = new Vector2(this.itemSize.x * this.dimensions.x, this.itemSize.y * this.dimensions.y);
        Rect rect = new Rect((this.position.x - (s.x * 0.5f)) - this.border, (this.position.y - (s.y * 0.5f)) - this.border, s.x + (this.border * 2), s.y + (this.border * 2));
        Color oldColor = GUI.color;
        GUI.color = this.color;
        GUI.depth = this.mouseOver == -1 ? 0 : -1;
        GUI.Label(rect, "", this.style);
        int i = 0;
        while (i < this.contents.Length)
        {
            Vector2 pos = Vector2.Scale(new Vector2(i % UnityScript.Lang.UnityBuiltins.parseInt(this.dimensions.x), Mathf.Floor(UnityScript.Lang.UnityBuiltins.parseFloat(i) / this.dimensions.x)), this.itemSize) + new Vector2(rect.x + this.border, rect.y + this.border);
            Rect recti = new Rect(pos.x + this.childOffset.x, pos.y + this.childOffset.y, this.itemSize.x, this.itemSize.y);
            this.contents[i].collider.guiRect = recti;
            if (this.contents[i].type != PType.Empty)
            {
                GUI.DrawTexture(recti, this.particleTextures[((int) (this.contents[i].type - 1))]);
            }
            i++;
        }
        GUI.color = oldColor;
    }

    public TrayGUI()
    {
        this.dimensions = new Vector2(1, 1);
        this.itemSize = new Vector2(32, 32);
        this.position = Vector2.zero;
        this.childOffset = Vector2.zero;
        this.border = 11;
        this.acceptTypes = new int[3];
        this.color = Color.white;
        this.mouseDown = -1;
        this.mouseOver = -1;
    }

}