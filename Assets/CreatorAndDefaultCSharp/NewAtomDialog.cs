using UnityEngine;
using System.Collections;

[System.Serializable]
public class NewAtomDialog : Dialog
{
    public int[] vals;
    public string[] names;
    public Texture2D field;
    public RectOffset fieldBorder;
    public Texture2D button;
    public Texture2D buttonHover;
    public RectOffset buttonBorder;
    public Font font;
    public Color color;
    private GUIStyle labelStyle;
    private GUIStyle buttonStyle;
    private GUIStyle fieldStyle;
    private GUIStyle rowStyle;
    private int lastControl;
    private float lastMove;
    private bool check;
    public virtual void Start()
    {
        this.vals[0] = UserAtom.p;
        this.vals[1] = UserAtom.n;
        this.vals[2] = UserAtom.e;
        this.names[0] = "Proton";
        this.names[1] = "Neutron";
        this.names[2] = "Electron";
        this.labelStyle = new GUIStyle();
        this.labelStyle.font = this.font;
        this.labelStyle.normal.textColor = this.color;
        this.labelStyle.padding.left = 4;
        this.fieldStyle = new GUIStyle();
        this.fieldStyle.font = this.font;
        this.fieldStyle.normal.textColor = this.color;
        this.fieldStyle.normal.background = this.field;
        this.fieldStyle.padding.left = 4;
        this.fieldStyle.border = this.fieldBorder;
        this.buttonStyle = new GUIStyle();
        this.buttonStyle.font = this.font;
        this.buttonStyle.normal.background = this.button;
        this.buttonStyle.normal.textColor = this.color;
        this.buttonStyle.hover.background = this.buttonHover;
        this.buttonStyle.hover.textColor = Color.white;
        this.buttonStyle.padding = this.buttonBorder;
        this.buttonStyle.border = this.buttonBorder;
        this.rowStyle = new GUIStyle();
        this.rowStyle.margin.top = 8;
    }

    public override void Content()
    {
        int i = 0;
        GUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.Label("New Atom: ", this.labelStyle, new GUILayoutOption[] {GUILayout.Height(30)});
        string[] outs = new string[3];
        int[] nvals = new int[3];
        GUILayout.BeginVertical(new GUILayoutOption[] {});
        i = 0;
        while (i < 3)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[] {});
            GUILayout.Label(("     " + this.names[i]) + "s", this.labelStyle, new GUILayoutOption[] {});
            GUILayout.Space(7);
            //GUI.SetNextControlName("field" + i);
            outs[i] = GUILayout.TextField("" + (this.vals[i] == -1 ? "" : this.vals[i].ToString()), this.fieldStyle, new GUILayoutOption[] {GUILayout.Width(40), GUILayout.Height(19)});
            GUILayout.EndHorizontal();
            outs[i] = outs[i] == "" ? "-1" : outs[i];
            if (int.TryParse(outs[i], out nvals[i]))
            {
                if (this.vals[i] != nvals[i])
                {
                    this.lastMove = Time.time;
                }
                this.vals[i] = nvals[i];
            }
            i++;
        }
        GUILayout.EndVertical();
        nvals[0] = Mathf.Clamp(this.vals[0], 1, Ngnome.MaxProtons());
        nvals[1] = AtomChart.ClosestExistingNuclide(nvals[0], this.vals[1]);
        nvals[2] = Mathf.Clamp(this.vals[2], 0, this.vals[0] + 1);
        bool changed = false;
        i = 0;
        while (i < 3)
        {
            if (this.vals[i] != nvals[i])
            {
                changed = true;
                this.check = false;
            }
            i++;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(6);
        GUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.FlexibleSpace();
        if (this.check || !changed)
        {
            i = 0;
            while (i < 3)
            {
                this.vals[i] = nvals[i];
                i++;
            }
            this.lastControl = GUIUtility.keyboardControl;
            bool buttonDown = GUILayout.Button("Ok", this.buttonStyle, new GUILayoutOption[] {GUILayout.Width(40), GUILayout.Height(30)});
            if (buttonDown)
            {
                UserAtom.p = this.vals[0];
                UserAtom.n = this.vals[1];
                UserAtom.e = this.vals[2];
                Application.LoadLevel(0);
            }
        }
        else
        {
            if (GUILayout.Button("Check", this.buttonStyle, new GUILayoutOption[] {GUILayout.Width(60), GUILayout.Height(30)}))
            {
                this.check = true;
                i = 0;
                while (i < 3)
                {
                    this.vals[i] = nvals[i];
                    i++;
                }
            }
        }
        GUILayout.EndHorizontal();
    }

    public NewAtomDialog()
    {
        this.vals = new int[3];
        this.names = new string[3];
        this.lastMove = -100f;
    }

}