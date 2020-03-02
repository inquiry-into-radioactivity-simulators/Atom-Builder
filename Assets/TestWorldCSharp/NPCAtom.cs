using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class NPCAtom : MonoBehaviour
{
    public int distLimit;
    public int cellSize;
    public Transform player;
    public Atom atom;
    public int p;
    public int n;
    public int e;
    public virtual void Start()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
        //transform.position =  Vector3.Scale(Random.onUnitSphere, Vector3(1,1,0)).normalized * Random.Range(startDist, distLimit);
        this.atom.p = this.p;
        this.atom.e = this.e;
        this.atom.n = this.n;
    }

    public virtual void Update()
    {
        float dist = (this.player.position - this.transform.position).sqrMagnitude;
        if (dist > (this.distLimit * this.distLimit))
        {
            Vector3 pos = this.player.transform.position + (Vector3.Scale((this.player.GetComponent<Rigidbody>().velocity * 0.02f) + Random.onUnitSphere, new Vector3(1, 1, 0)).normalized * this.distLimit);
            bool rejected = false;
            int ii = 0;
            while (ii < Atom.all.Length)
            {
                if ((pos - Atom.all[ii].rbody.position).sqrMagnitude < (this.cellSize * this.cellSize))
                {
                    rejected = true;
                }
                ii++;
            }
            if (!rejected && !TestGUI.cullBackAndCounter)
            {
                this.transform.position = pos;
                this.atom.p = this.atom.origP;
                this.atom.n = this.atom.origN;
                this.atom.e = this.atom.origE;
            }
        }
    }

    public NPCAtom()
    {
        this.distLimit = 9;
        this.cellSize = 8;
    }

}