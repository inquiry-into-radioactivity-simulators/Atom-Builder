using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class LoadingGUI : MonoBehaviour
{
    public float width;
    public float height;
    public int levelToLoad;
    public virtual void OnGUI()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        GUI.Label(new Rect((screenSize.x * 0.5f) - (this.width * 0.5f), screenSize.y * 0.5f, this.width, this.height), "Loading");
    }

    public virtual IEnumerator Start()
    {
        yield return null;
        yield return null;
        yield return null;
        Application.LoadLevel(this.levelToLoad);
    }

    public LoadingGUI()
    {
        this.width = 60f;
        this.height = 20f;
        this.levelToLoad = 1;
    }

}