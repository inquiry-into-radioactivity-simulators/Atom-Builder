using UnityEngine;
using System.Collections;

[System.Serializable]
public class Draggable : GUISprite
{
    public Vector2 origin;
    public float failedTime;
    public float lerpSpeed;
    public float lerpTime;
    public virtual void Update()
    {
        this.FailDragUpdate();
    }

    public virtual void FailDragUpdate()
    {
        if ((this.position != Vector2.zero) && (this.origin == Vector2.zero))
        {
            this.origin = this.position;
        }
        if (this.failedTime != 0)
        {
            float dT = Time.deltaTime * this.lerpSpeed;
            float oneMinusDT = 1 - dT;
            this.position = (this.position * oneMinusDT) + (this.origin * dT);
            this.color.a = this.color.a * oneMinusDT;
            if ((Time.time - this.failedTime) > this.lerpTime)
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }
    }

    public virtual void FailDrag()
    {
        this.failedTime = Time.time;
    }

    public virtual void Die()
    {
        UnityEngine.Object.Destroy(this.gameObject);
    }

    public Draggable()
    {
        this.origin = Vector2.zero;
        this.lerpSpeed = 2f;
        this.lerpTime = 1.6f;
    }

}