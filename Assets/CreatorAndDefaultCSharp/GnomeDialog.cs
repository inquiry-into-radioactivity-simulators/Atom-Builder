using UnityEngine;
using System.Collections;

[System.Serializable]
public class GnomeDialog : Dialog
{
    public int mode;
    public string message;
    public Ngnome ngnome;
    public Texture2D button;
    public Texture2D buttonHover;
    public RectOffset buttonBorder;
    public Texture2D gnomeIcon;
    public Font font;
    public Color color;
    private GUIStyle labelStyle;
    private GUIStyle textStyle;
    private GUIStyle buttonStyle;
    private int lastControl;
    private float lastMove;
    private int gnomeClicks;
    public static GnomeDialog instance;
    public virtual void Start()
    {
        GnomeDialog.instance = this;
        this.labelStyle = new GUIStyle();
        this.labelStyle.normal.background = this.gnomeIcon;
        this.textStyle = new GUIStyle();
        this.textStyle.font = this.font;
        this.textStyle.normal.textColor = this.color;
        this.buttonStyle = new GUIStyle();
        this.buttonStyle.font = this.font;
        this.buttonStyle.normal.background = this.button;
        this.buttonStyle.normal.textColor = this.color;
        this.buttonStyle.hover.background = this.buttonHover;
        this.buttonStyle.hover.textColor = Color.white;
        this.buttonStyle.padding = this.buttonBorder;
        this.buttonStyle.border = this.buttonBorder;
        this.enabled = false;
    }

    public override void Content()
    {
        this.size.x = this.mode == 0 ? 247 : 172;
        int i = 0;
        GUILayout.Space(6);
        GUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.Space(6);
        if (GUILayout.Button("", this.labelStyle, new GUILayoutOption[] {GUILayout.Height(this.gnomeIcon.height), GUILayout.Width(this.gnomeIcon.width)}))
        {
            this.gnomeClicks++;
            if (this.gnomeClicks > 10)
            {
                Ngnome.nMGMT = false;
                this.enabled = false;
            }
        }
        GUILayout.Space(6);
        GUILayout.BeginVertical(new GUILayoutOption[] {});
        GUILayout.Space(50);
        GUILayout.Label(this.mode == 0 ? "You can't test this atom yet." : (((("Ok, ready to go!\np:" + UserAtom.p) + "  n: ") + UserAtom.n) + "  e: ") + UserAtom.e, this.textStyle, new GUILayoutOption[] {});
        GUILayout.Space(6);
        GUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.FlexibleSpace();
        if (this.mode == 0)
        {
            if (GUILayout.Button("Cancel", this.buttonStyle, new GUILayoutOption[] {GUILayout.Width(80), GUILayout.Height(30)}))
            {
                this.enabled = false;
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Fix it", this.buttonStyle, new GUILayoutOption[] {GUILayout.Width(80), GUILayout.Height(30)}))
            {
                if (this.mode == 0)
                {
                    this.StartCoroutine(this.ngnome.Go2());
                }
                else
                {
                    this.ngnome.Go();
                }
                this.enabled = false;
            }
        }
        else
        {
            if (GUILayout.Button("Ok", this.buttonStyle, new GUILayoutOption[] {GUILayout.Width(40), GUILayout.Height(30)}))
            {
                this.enabled = false;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    public GnomeDialog()
    {
        this.message = "";
        this.lastMove = -100f;
    }

}