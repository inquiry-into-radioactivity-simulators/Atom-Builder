using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class GUISprite : MonoBehaviour
{
    // GUISprite.js
    public Color color;
    public int depth;
    public Texture2D texture;
    public Vector2 size;
    public Vector2 position;
    public float scale;
    public float cycleTime;
    public bool oneShot;
    private float startTime;
    private GUIStyle style;
    public virtual void Start()
    {
        this.OverridePreStart();
        this.startTime = Time.time;
        this.style = new GUIStyle();
        this.style.normal.background = this.texture;
    }

    public virtual void OverridePreStart()
    {
    }

    public virtual void OnGUI()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        if (this.texture)
        {
            float frameCount = this.size.x * this.size.y;
            float progress = (Time.time - this.startTime) / this.cycleTime;
            if ((progress > 1f) && this.oneShot)
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
            else
            {
                int floatSizeX = Screen.width;
                int frame = UnityScript.Lang.UnityBuiltins.parseInt(Mathf.Repeat(Mathf.Floor(progress * frameCount), frameCount));
                Vector2 s = new Vector2(this.texture.width / this.size.x, this.texture.height / this.size.y) * this.scale;
                Vector2 offset = Vector2.Scale(new Vector2(frame % UnityScript.Lang.UnityBuiltins.parseInt(this.size.x), Mathf.Floor(UnityScript.Lang.UnityBuiltins.parseFloat(frame) / this.size.x)), s);
                Rect rect = new Rect(this.position.x - (s.x * 0.5f), this.position.y - (s.y * 0.5f), s.x, s.y);
                GUI.depth = this.depth;
                GUI.BeginGroup(rect);
                Color oldColor = GUI.color;
                GUI.color = this.color;
                this.style.normal.background = this.texture;
                GUI.Label(new Rect(-offset.x, -offset.y, this.texture.width * this.scale, this.texture.height * this.scale), "", this.style);
                GUI.color = oldColor;
                GUI.EndGroup();
            }
        }
    }

    public GUISprite()
    {
        this.color = Color.white;
        this.size = new Vector2(1, 1);
        this.position = Vector2.zero;
        this.scale = 1f;
        this.cycleTime = 1f;
    }

}