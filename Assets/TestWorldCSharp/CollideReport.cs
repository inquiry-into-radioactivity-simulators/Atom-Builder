using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CollideReport : MonoBehaviour
{
    public Atom atom;
    public InsetCamera insetCam;
    public virtual void OnCollisionEnter(Collision collision)
    {
        if (this.atom)
        {
            this.atom.Hit(collision);
        }
    }

    public virtual void OnMouseUp()
    {
        if (this.insetCam)
        {
            this.insetCam.OnMouseUp();
        }
    }

    public virtual void OnMouseOver()
    {
        if (this.insetCam)
        {
            this.insetCam.OnMouseOver();
        }
    }

    public virtual void OnMouseExit()
    {
        if (this.insetCam)
        {
            this.insetCam.OnMouseExit();
        }
    }

}