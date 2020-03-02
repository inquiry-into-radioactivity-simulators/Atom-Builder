using System.Collections;

[System.Serializable]
public class Draggable_Nucleon : Draggable
{
    public PType type;
    public override void OverridePreStart()
    {
        string str = this.type == PType.P ? "Proton_TIF" : (this.type == PType.N ? "Neutron_TIF" : "Electron_TIF");
        this.texture =  (UnityEngine.Texture2D)UnityEngine.Resources.Load(str);
        this.depth = -2;
        this.scale = 0.34f * CreatorGUI.guiScale;
    }

    public override void Update()
    {
        if (this.failedTime != 0)
        {
            float dT = UnityEngine.Time.deltaTime * this.lerpSpeed;
            float oneMinusDT = 1 - dT;
            this.position = (this.position * oneMinusDT) + ((this.position + new UnityEngine.Vector2(-50, 50)) * dT);
            this.color.a = this.color.a * oneMinusDT;
            if ((UnityEngine.Time.time - this.failedTime) > this.lerpTime)
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }
    }

}