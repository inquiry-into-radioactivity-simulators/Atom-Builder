using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CreatorParticle : MonoBehaviour
{
    public float lifetime;
    public Vector3 velocity;
    public Vector3 fromLerp;
    public float lerpEndTime;
    private float startTime;
    private float lerpStartTime;
    public virtual void Awake()
    {
        this.startTime = Time.time;
        Nucleus.recentParticle = this.transform;
    }

    public virtual void LateUpdate()
    {
        this.transform.position = this.transform.position + (this.velocity * Time.deltaTime);
        if ((Time.time < this.lerpEndTime) && (this.fromLerp != Vector3.zero))
        {
            if (this.lerpStartTime == 0f)
            {
                this.lerpStartTime = Time.time;
            }
            // transform.position set in update
            this.transform.position = Vector3.Lerp(this.fromLerp, this.transform.position, (Time.time - this.lerpStartTime) / (this.lerpEndTime - Time.time));
        }
        else
        {
            this.lerpStartTime = 0;
        }
        if ((this.lifetime != 0f) && (Time.time > (this.startTime + this.lifetime)))
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

    public CreatorParticle()
    {
        this.velocity = Vector3.zero;
        this.fromLerp = Vector3.zero;
    }

}