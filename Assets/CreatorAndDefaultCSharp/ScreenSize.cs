using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class ScreenSize : MonoBehaviour
{
    public float resX;
    public float resY;
    public float oFOV;
    public virtual void Start()
    {
        this.oFOV = Camera.main.fieldOfView;
    }

    public virtual void Update()
    {
        float desiredAspect = this.resX / this.resY;
        float curAspect = UnityScript.Lang.UnityBuiltins.parseFloat(Screen.width) / UnityScript.Lang.UnityBuiltins.parseFloat(Screen.height);
        if (curAspect != desiredAspect)
        {
            float desiredHeight = UnityScript.Lang.UnityBuiltins.parseFloat(Screen.width) / desiredAspect;
            float margin = (UnityScript.Lang.UnityBuiltins.parseFloat(Screen.height) - desiredHeight) / UnityScript.Lang.UnityBuiltins.parseFloat(Screen.height);
            Camera.main.fieldOfView = this.oFOV + ((this.oFOV * margin) * 1.1f);
        }
        else
        {
            Camera.main.fieldOfView = this.oFOV;
        }
    }

    public ScreenSize()
    {
        this.resX = 800f;
        this.resY = 450f;
    }

}