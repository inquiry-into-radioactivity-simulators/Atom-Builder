using UnityEngine;
using System.Collections;

[System.Serializable]
public class TimeInterval : object
{
    public string name;
    public float time;
    public TimeInterval()
    {
        this.name = "";
    }

}
[System.Serializable]
public partial class TestGUI : MonoBehaviour
{
    public Color color;
    public Color radColor;
    public Font font;
    public Vector2 displaySize;
    public int timeRectWidth;
    public float intervalInRealTime;
    public static bool cullBackAndCounter;
    public RectOffset mainDisplayBorder;
    public Texture2D mainDisplayTexture;
    public Texture2D boxTexture;
    public Texture2D boxTexture2;
    public Texture2D boxTexture3;
    public Texture2D timeTexture;
    public Texture2D radTexture;
    public Texture2D buttonTexture;
    public Texture2D buttonTextureHover;
    public Texture2D barBG;
    public Texture2D bar;
    public Texture2D barBG2;
    public Texture2D bar2;
    public Texture2D pause1;
    public Texture2D pause2;
    public Texture2D play1;
    public Texture2D play2;
    public RectOffset barBorder;
    public RectOffset boxBorder;
    private GUIStyle boxStyle;
    private GUIStyle okStyle;
    private GUIStyle mainDisplayStyle;
    private GUIStyle labelStyle;
    private GUIStyle backStyle;
    private GUIStyle barStyle;
    private GUIStyle iconStyle;
    private GUIStyle icon2Style;
    private float realTime;
    public static float simTime;
    public TimeInterval[] times;
    public bool exploded;
    public bool cullTimer;
    public bool paused;
    public static bool hasBar;
    public virtual void OnDisable()
    {
        TestGUI.cullBackAndCounter = false;
        Time.timeScale = 1;
        TestGUI.simTime = 0;
    }

    public virtual void Start()
    {
        TestGUI.simTime = 0;
        TestGUI.cullBackAndCounter = false;
        this.mainDisplayStyle = new GUIStyle();
        this.mainDisplayStyle.normal.background = this.mainDisplayTexture;
        this.mainDisplayStyle.border = this.mainDisplayBorder;
        this.backStyle = new GUIStyle();
        this.backStyle.normal.background = this.buttonTexture;
        this.backStyle.hover.background = this.buttonTextureHover;
        this.boxStyle = new GUIStyle();
        this.boxStyle.font = this.font;
        this.boxStyle.normal.background = this.boxStyle.hover.background = this.boxTexture;
        this.boxStyle.normal.textColor = this.boxStyle.hover.textColor = this.color;
        this.boxStyle.padding.left = 6;
        this.boxStyle.padding.top = 4;
        this.boxStyle.border = this.boxBorder;
        this.okStyle = new GUIStyle();
        this.okStyle.font = this.font;
        this.okStyle.normal.background = this.boxTexture2;
        this.okStyle.hover.background = this.boxTexture3;
        this.okStyle.normal.textColor = this.radColor;
        this.okStyle.hover.textColor = Color.white;
        this.okStyle.padding.left = 9;
        this.okStyle.padding.top = 4;
        this.okStyle.border = this.boxBorder;
        this.barStyle = new GUIStyle();
        this.barStyle.normal.background = this.barBG;
        this.barStyle.border = this.barBorder;
        this.iconStyle = new GUIStyle();
        this.iconStyle.normal.background = this.timeTexture;
        this.icon2Style = new GUIStyle();
        this.icon2Style.normal.background = this.pause1;
        this.icon2Style.hover.background = this.pause2;
        this.labelStyle = new GUIStyle();
        this.labelStyle.font = this.font;
        this.labelStyle.alignment = TextAnchor.MiddleCenter;
        this.labelStyle.normal.textColor = this.labelStyle.hover.textColor = this.color;
    }

    public virtual void Update()
    {
        Time.timeScale = this.paused ? 0 : (this.exploded ? 0.333f : 1);
        Time.fixedDeltaTime = this.exploded ? 0.006667f : 0.02f;
        if (!this.exploded && !Ngnome.nMGMT)
        {
            float noBarTime = 1.5f;
            this.realTime = this.realTime + (Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 5 : 1));
            if ((TestGUI.hasBar && (TestGUI.simTime > AtomChart.elements[UserAtom.p - 1].nuclides[UserAtom.n].halfLife)) || ((!TestGUI.hasBar && (this.realTime > noBarTime)) && (AtomChart.elements[UserAtom.p - 1].nuclides[UserAtom.n].halfLife < AtomChart.halfLives[0])))
            {
                this.exploded = true;
                Atom.playerInstance.Decay();
            }
            if ((AtomChart.elements[UserAtom.p - 1].nuclides[UserAtom.n].halfLife == AtomChart.halfLives[0]) && ((TestGUI.hasBar && (TestGUI.simTime >= AtomChart.halfLives[0])) || (!TestGUI.hasBar && (this.realTime > (noBarTime * 2)))))
            {
                this.cullTimer = true;
            }
        }
    }

    public virtual void OnGUI()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        GUI.depth = 1;
        bool hasClockPart = !Ngnome.nMGMT && !this.cullTimer;
        Rect displayRect = new Rect(0, screenSize.y - this.displaySize.y, this.displaySize.x, this.displaySize.y);
        Rect buttonRect = new Rect(3, displayRect.y - 5, this.buttonTexture.width, this.buttonTexture.height);
        Rect textRect = displayRect;
        textRect.width = 55;
        textRect.height = textRect.height + 50;
        textRect.y = textRect.y - 3;
        textRect.x = textRect.x + (10 + this.buttonTexture.width);
        int okBonus = this.exploded && !TestGUI.cullBackAndCounter ? 50 : 0;
        int barBonus = TestGUI.hasBar ? 200 : 0;
        int timeRectTotalWidth = ((this.timeRectWidth + 3) + okBonus) + barBonus;
        Rect timeRect = new Rect((textRect.x + textRect.width) + 5, displayRect.y - 3, timeRectTotalWidth - 3, 150);
        displayRect.width = displayRect.width + (!hasClockPart ? 0 : timeRectTotalWidth);
        GUI.Box(this.mainDisplayBorder.Add(displayRect), "", this.mainDisplayStyle);
        if (!TestGUI.cullBackAndCounter)
        {
            GUI.Box(textRect, (((("p:" + UserAtom.p) + "\nn:") + UserAtom.n) + "\ne:") + UserAtom.e, this.boxStyle);
            if (GUI.Button(buttonRect, "", this.backStyle))
            {
                Application.LoadLevel(0);
            }
        }
        if (hasClockPart)
        {
            float adjustedRealTime = this.realTime / this.intervalInRealTime;
            float curInterval = Mathf.Min(Mathf.Floor(adjustedRealTime), this.times.Length - 1);
            float intProgress = Mathf.Min(adjustedRealTime - curInterval, 1);
            TestGUI.simTime = Mathf.Lerp(this.times[Mathf.RoundToInt(curInterval)].time, this.times[Mathf.RoundToInt(Mathf.Min(curInterval + 1, this.times.Length - 1))].time, intProgress);
            GUI.Box(timeRect, "", this.boxStyle);
            GUI.BeginGroup(timeRect);
            Rect pauseRect = new Rect(7, 7, this.timeTexture.width, this.timeTexture.height);
            Rect timerRect = new Rect(10 + this.timeTexture.width, 7, this.timeTexture.width, this.timeTexture.height);
            float alreadyWidth = (timerRect.width * 2) + 15;
            Rect text2Rect = new Rect(alreadyWidth - 4, 12, (timeRectTotalWidth - (okBonus + 3)) - alreadyWidth, (this.timeTexture.height - 12) * 0.5f);
            Rect barRect = new Rect(alreadyWidth + 14, 34, ((timeRectTotalWidth - (okBonus + 3)) - alreadyWidth) - 40, 15);
            Rect bar2Rect = this.barBorder.Remove(barRect);
            bar2Rect.width = bar2Rect.width * intProgress;
            this.iconStyle.normal.background = this.exploded ? this.radTexture : this.timeTexture;
            this.icon2Style.normal.background = !this.paused ? this.pause1 : this.play1;
            this.icon2Style.hover.background = !this.paused ? this.pause2 : this.play2;
            if (GUI.Button(pauseRect, "", this.icon2Style))
            {
                this.paused = !this.paused;
            }
            if (GUI.Button(timerRect, "", this.iconStyle))
            {
                TestGUI.hasBar = !TestGUI.hasBar;
            }
            this.labelStyle.normal.textColor = this.labelStyle.hover.textColor = this.exploded ? this.radColor : this.color;
            this.barStyle.normal.background = this.exploded ? this.barBG2 : this.barBG;
            if (TestGUI.hasBar)
            {
                GUI.Label(text2Rect, this.times[Mathf.RoundToInt(curInterval)].name, this.labelStyle);
                GUI.Box(barRect, "", this.barStyle);
                GUI.DrawTexture(bar2Rect, this.exploded ? this.bar2 : this.bar);
            }
            if (this.exploded && !TestGUI.cullBackAndCounter)
            {
                Rect okButtonRect = new Rect((text2Rect.x + text2Rect.width) + 4, 17, 38, 30);
                if (GUI.Button(okButtonRect, "ok", this.okStyle))
                {
                    this.StartCoroutine(this.ResetRoutine());
                }
            }
            GUI.EndGroup();
        }
    }

    public virtual IEnumerator ResetRoutine()
    {
        yield return null;
        this.exploded = false;
        this.realTime = 0;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Debris");
        foreach (GameObject o in objs)
        {
            UnityEngine.Object.Destroy(o);
        }
    }

    public TestGUI()
    {
        this.displaySize = new Vector2(100, 65);
        this.timeRectWidth = 180;
        this.intervalInRealTime = 5f;
    }

}