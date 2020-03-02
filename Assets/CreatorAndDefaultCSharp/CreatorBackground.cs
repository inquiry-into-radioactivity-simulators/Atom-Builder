using UnityEngine;
using System.Collections;

[System.Serializable]
public class CreatorBackground : DraggableType
{
    public AtomChart chart;
    public GameObject particlePrefab;
    public Orbitals orbitals;
    public Nucleus nucleus;
    public Transform strongForceRegion;
    public int mouseOver;
    public virtual void Start()
    {
        DraggableCollider coll = (DraggableCollider) this.gameObject.AddComponent(typeof(DraggableCollider));
        coll.id = 0;
        coll.parent = this;
        foreach (Transform t in this.transform)
        {
            this.strongForceRegion = t;
        }
        coll = (DraggableCollider) this.strongForceRegion.gameObject.AddComponent(typeof(DraggableCollider));
        coll.id = 1;
        coll.parent = this;
    }

    public virtual void Update()
    {
        float size = (((this.nucleus.points[(this.nucleus.p + this.nucleus.n) - 1].pos.magnitude * 1.47f) + this.nucleus.particleRadius) * this.nucleus.overallScale) * 2;
        this.strongForceRegion.localScale = Vector3.one * size;
        this.strongForceRegion.position = (this.transform.position - Camera.main.transform.position).normalized * ((size * 0.5f) + 0.3f);

        {
            float _75 = this.strongForceRegion.position.x * Mathf.Lerp(1.5f, 1f, UnityScript.Lang.UnityBuiltins.parseFloat(this.nucleus.p + this.nucleus.n) / 80);
            Vector3 _76 = this.strongForceRegion.position;
            _76.x = _75;
            this.strongForceRegion.position = _76;
        }
    }

    public override Draggable OnDragEvent(DragEventType type, int id, Draggable current, DraggableType target)
    {
        if (type == DragEventType.Over)
        {
            this.mouseOver = id;
        }
        else
        {
            if (type == DragEventType.Exit)
            {
                this.mouseOver = -1;
            }
        }
        return current;
    }

    public override bool CanDrop(Draggable d)
    {
        if (this.mouseOver == 1)
        {
            return this.nucleus.CanDrop(d);
        }
        return d as Draggable_Nucleon ? true : false;
    }

    public override void Drop(Draggable d)
    {
        float time = 0f;
        Draggable_Nucleon nuc = d as Draggable_Nucleon;
        Vector3 pos = DragNDrop.ScreenToWorld(Input.mousePosition) + Vector3.forward;
        Vector3 outDir = new Vector3(pos.x, pos.y, 0).normalized;
        GameObject particleObj = UnityEngine.Object.Instantiate(this.particlePrefab, pos, Quaternion.identity);
        CreatorParticle mover = (CreatorParticle) particleObj.GetComponent(typeof(CreatorParticle));
        string resourceName = nuc.type == PType.E ? "Electron_TIF" : (nuc.type == PType.P ? "Proton_TIF" : "Neutron_TIF");
        particleObj.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load(resourceName);
        if (this.mouseOver == 0)
        {
            if (nuc.type == PType.E)
            {
                if ((UserAtom.p >= UserAtom.e) && (AtomChart.elements[UserAtom.p - 1].grabElectron || (UserAtom.e < UserAtom.p)))
                {
                    float speed = 3f;
                    Vector3 targetPos = Vector3.right * this.orbitals.shells[this.orbitals.outerShell].radius;
                    speed = Mathf.Clamp(speed * (targetPos - pos).magnitude, 3, 10);
                    time = (pos - targetPos).magnitude / speed;
                    mover.velocity = (targetPos - pos).normalized * speed;
                    mover.lifetime = time;
                    this.StartCoroutine(this.AddToOrbitalsRoutine(time));
                }
                else
                {
                    mover.velocity = outDir * (UserAtom.p < UserAtom.e ? 4 : 1);
                    mover.lifetime = 10;
                }
            }
            else
            {
                if (nuc.type == PType.N)
                {
                    mover.velocity = outDir;
                    mover.lifetime = 10;
                }
                else
                {
                    float ratio = Mathf.Min(pos.magnitude / this.orbitals.shells[this.orbitals.outerShell].radius, 1);
                    mover.velocity = outDir * Mathf.Lerp(8, 1, ratio);
                    mover.lifetime = 10;
                }
            }
        }
        else
        {
             // mouseOver == 1
            if (nuc.type == PType.E)
            {
                mover.velocity = outDir * 8;
                mover.lifetime = 3;
            }
            else
            {
                mover.velocity = outDir * -4;
                time = 0.2f;
                mover.lifetime = time;
                this.StartCoroutine(this.AddToNucleusRoutine(time, nuc.type));
            }
        }
    }

    public virtual IEnumerator AddToOrbitalsRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        this.orbitals.AddElectron();
    }

    public virtual IEnumerator AddToNucleusRoutine(float time, PType t)
    {
        yield return new WaitForSeconds(time);
        if (t == PType.N)
        {
            this.nucleus.n++;
            this.nucleus.Rebuild();
        }
        else
        {
            this.nucleus.p++;
            this.nucleus.Rebuild();
        }
    }

    public CreatorBackground()
    {
        this.mouseOver = -1;
    }

}