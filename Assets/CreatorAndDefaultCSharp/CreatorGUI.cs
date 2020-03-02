using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CreatorGUI : MonoBehaviour
{
    public Color color;
    public Font font;
    public Ngnome ngnome;
    public Vector2 displaySize;
    public float trayHeight;
    public float itemSize;
    public RectOffset mainDisplayBorder;
    public RectOffset mainDisplayEdge;
    public Texture2D mainDisplayTexture;
    public Texture2D boxTexture;
    public Texture2D boxTextureHover;
    public Texture2D buttonTexture;
    public Texture2D buttonTextureHover;
    public RectOffset boxBorder;
    public RectOffset boxEdge;
    public NewAtomDialog newAtomBox;
    public TrayGUI[] trays;
    private Vector2 traySize;
    private GUIStyle mainDisplayStyle;
    private GUIStyle boxStyle;
    private GUIStyle labelStyle;
    private GUIStyle buttonStyle;
    private bool initialized;
    private Vector2 lastResolution;
    public static float guiScale;
    public virtual void Awake()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        CreatorGUI.guiScale = screenSize.y / 480f;
    }

    public virtual IEnumerator Start()
    {
        PType type = default(PType);
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        int i = 0;
        int ii = 0;
        this.mainDisplayStyle = new GUIStyle();
        this.mainDisplayStyle.normal.background = this.mainDisplayTexture;
        this.mainDisplayStyle.border = this.mainDisplayBorder;
        this.labelStyle = new GUIStyle();
        this.labelStyle.normal.textColor = this.color;
        this.labelStyle.font = this.font;
        this.labelStyle.padding = this.boxEdge;
        this.labelStyle.padding.left = this.labelStyle.padding.left + 5;
        this.buttonStyle = new GUIStyle();
        this.buttonStyle.normal.background = this.buttonTexture;
        this.buttonStyle.hover.background = this.buttonTextureHover;
        this.boxStyle = new GUIStyle();
        this.boxStyle.font = this.font;
        this.boxStyle.normal.background = this.boxTexture;
        this.boxStyle.normal.textColor = this.color;
        this.boxStyle.hover.background = this.boxTextureHover;
        this.boxStyle.hover.textColor = Color.white;
        this.boxStyle.padding.left = 6;
        this.boxStyle.padding.top = 4;
        this.boxStyle.border = this.boxBorder;
        this.trays = new TrayGUI[3];
        i = 0;
        while (i < 3)
        {
            type = (PType) (i + 1);
            GameObject obj = new GameObject(("" + type) + " Tray");
            obj.transform.parent = this.transform;
            TrayGUI tray = (TrayGUI) obj.AddComponent(typeof(TrayGUI));
            tray.texture = this.boxTexture;
            tray.border = this.boxBorder.right;
            tray.itemSize = (new Vector2(1, 1) * this.itemSize) * CreatorGUI.guiScale;
            this.traySize = new Vector2(tray.itemSize.x + (tray.border * 1.4f), (tray.itemSize.y * (this.trayHeight - 1)) + (tray.border * 0.5f));
            Vector2 traySpacing = new Vector2(tray.itemSize.x + (tray.border * 1.35f), tray.itemSize.y * (this.trayHeight - 1));
            tray.position = new Vector2((traySpacing.x * (UnityScript.Lang.UnityBuiltins.parseFloat(i) + 0.5f)) + (tray.border * 0.3f), screenSize.y - ((traySpacing.y * 0.5f) - (tray.itemSize.y * 0.5f)));
            tray.dimensions = new Vector2(1, this.trayHeight);
            tray.acceptTypes = new int[3];
            ii = 0;
            while (ii < tray.acceptTypes.Length)
            {
                tray.acceptTypes[i] = 0;
                ii++;
            }
            tray.acceptTypes[i] = 1;
            this.trays[i] = tray;
            i++;
        }
        yield return null;
        i = 0;
        while (i < 3)
        {
            type = (PType) (i + 1);
            this.trays[i].contents[0].type = PType.Empty;
            ii = 1;
            while (ii < this.trays[i].contents.Length)
            {
                this.trays[i].contents[ii].type = type;
                ii++;
            }
            i++;
        }
        this.initialized = true;
    }

    public virtual void Update()
    {
        Vector2 res = new Vector2(Screen.width, Screen.height);
        if (this.lastResolution != res)
        {
            if (this.lastResolution != new Vector2(0, 0))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
            this.lastResolution = res;
        }
        if (this.initialized)
        {
            int i = 0;
            int ii = 0;
            i = 0;
            while (i < this.trays.Length)
            {
                int iType = i + 1;
                ii = this.trays[i].contents.Length - 2;
                while (ii >= 1)
                {
                    if (this.trays[i].contents[ii + 1].type == PType.Empty)
                    {
                        this.trays[i].contents[ii + 1].type = this.trays[i].contents[ii].type;
                        this.trays[i].contents[ii].type = PType.Empty;
                    }
                    ii--;
                }
                if (this.trays[i].contents[0].type != PType.Empty)
                {
                    this.trays[i].contents[0].type = PType.Empty;
                    this.trays[i].childOffset = new Vector2(0, -this.itemSize * CreatorGUI.guiScale);
                }
                if (this.trays[i].contents[1].type == PType.Empty)
                {
                    this.trays[i].contents[1].type = (PType) iType;
                    this.trays[i].childOffset = new Vector2(0, this.itemSize * CreatorGUI.guiScale);
                }
                this.trays[i].childOffset.y = Mathf.Lerp(this.trays[i].childOffset.y, 0, Time.deltaTime * 5);
                i++;
            }
        }
    }

    public virtual void OnGUI()
    {
        if (this.initialized)
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            GUI.depth = 1;
            float traysWidth = (this.traySize.x * 3) + this.mainDisplayEdge.left;
            Rect traysHolderRect = new Rect(-50, screenSize.y - this.traySize.y, (this.traySize.x * 3) + 50, this.traySize.y);
            Rect displayRect = new Rect(traysWidth, screenSize.y - this.displaySize.y, this.displaySize.x, this.displaySize.y);
            int textWidth = 55;
            Rect textRect = this.boxEdge.Add(displayRect);
            textRect.width = textWidth;
            textRect.height = textRect.height + 50;
            Rect buttonRect = this.boxEdge.Add(displayRect);
            buttonRect.x = buttonRect.x + (textWidth + 10);
            buttonRect.width = 89;
            buttonRect.y = buttonRect.y + 16;
            buttonRect.height = 29;
            Rect button2Rect = this.boxEdge.Add(displayRect);
            button2Rect.x = button2Rect.x + ((textWidth + 20) + 89);
            button2Rect.width = this.buttonTexture.width;
            button2Rect.y = button2Rect.y - 1;
            button2Rect.height = this.buttonTexture.height;
            GUI.Box(this.mainDisplayBorder.Add(displayRect), "", this.mainDisplayStyle);
            this.boxStyle.hover.background = this.boxTexture;
            GUI.Box(textRect, "", this.boxStyle);
            GUI.Box(textRect, (((("p:" + UserAtom.p) + "\nn:") + UserAtom.n) + "\ne:") + UserAtom.e, this.labelStyle);
            this.boxStyle.hover.background = this.newAtomBox.enabled ? this.boxTexture : this.boxTextureHover;
            this.boxStyle.hover.textColor = this.newAtomBox.enabled ? this.color : Color.white;
            if (GUI.Button(buttonRect, "New Atom", this.boxStyle))
            {
                this.newAtomBox.enabled = true;
            }
            if (GUI.Button(button2Rect, "", this.buttonStyle))
            {
                this.ngnome.Go();
            }
            GUI.Box(this.mainDisplayBorder.Add(traysHolderRect), "", this.mainDisplayStyle);
        }
    }

    public CreatorGUI()
    {
        this.displaySize = new Vector2(500, 65);
        this.trayHeight = 5f;
        this.itemSize = 28f;
        this.traySize = Vector2.zero;
        this.lastResolution = new Vector2(0, 0);
    }

    static CreatorGUI()
    {
        CreatorGUI.guiScale = 1f;
    }

}