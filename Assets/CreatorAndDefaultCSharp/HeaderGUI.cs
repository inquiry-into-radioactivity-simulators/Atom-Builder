using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class HeaderGUI : MonoBehaviour
{
    public Color color;
    public Font font;
    public Texture2D background;
    public AtomChart chart;
    private GUIStyle guiStyle;
    public static string header;
    public virtual void Start()
    {
        this.guiStyle = new GUIStyle();
        this.guiStyle.normal.background = this.background;
        this.guiStyle.normal.textColor = this.color;
        this.guiStyle.font = this.font;
        this.guiStyle.alignment = TextAnchor.UpperCenter;
        this.guiStyle.padding.top = 2;
    }

    public virtual void OnGUI()
    {
        if ((AtomChart.elements != null) && !TestGUI.cullBackAndCounter)
        {
            GUI.Box(new Rect((Screen.width - this.background.width) * 0.5f, 0, this.background.width, this.background.height), AtomChart.elements[UserAtom.p - 1].name, this.guiStyle);
        }
    }

    static HeaderGUI()
    {
        HeaderGUI.header = "";
    }

}