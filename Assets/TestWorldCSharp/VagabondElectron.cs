using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class VagabondElectron : MonoBehaviour
{
    public int distLimit;
    public int cellSize;
    public float elecForce;
    public Transform player;
    public Rigidbody rbody;
    public Renderer plane;
    public bool going;
    public Rigidbody[] all;
    public virtual void Awake()
    {
        this.rbody = this.GetComponent<Rigidbody>();
    }

    public virtual void Start()
    {
        if (Ngnome.nMGMT)
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
        this.rbody.useGravity = false;
        VagabondElectron[] allV = (VagabondElectron[]) UnityEngine.Object.FindObjectsOfType(typeof(VagabondElectron));
        this.all = new Rigidbody[allV.Length];
        int i = 0;
        while (i < allV.Length)
        {
            this.all[i] = allV[i].rbody;
            i++;
        }
    }

    public virtual void Update()
    {
        Vector3 screenSpace = Camera.main.WorldToViewportPoint(this.rbody.position);
        bool newGoing = screenSpace == new Vector3(Mathf.Clamp(screenSpace.x, -0.4f, 1.4f), Mathf.Clamp(screenSpace.y, -0.4f, 1.4f), screenSpace.z);
        if (newGoing != this.going)
        {
            if (newGoing)
            {
                this.rbody.velocity = new Vector3(Random.value, Random.value, 0).normalized * Random.Range(1, 2);
            }
            else
            {
                this.rbody.velocity = Vector3.zero;
            }
        }
        this.going = newGoing;
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
            if (!rejected)
            {
                this.transform.position = pos;
                this.rbody.velocity = new Vector3(Random.value, Random.value, 0);
            }
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody)
        {
            CollideReport other = (CollideReport) collision.rigidbody.gameObject.GetComponent(typeof(CollideReport));
            if (other && (other.atom.e < other.atom.p))
            {
                other.atom.e = other.atom.e + 1;
                if (other.atom.isPlayer)
                {
                    UserAtom.e = other.atom.e;
                }
                this.transform.position = new Vector3(1000, 1000, 0);
            }
        }
    }

    public virtual void FixedUpdate()
    {
        int i = 0;
        if (this.going)
        {
            this.rbody.MoveRotation(Quaternion.identity);
            this.rbody.MovePosition(Vector3.Scale(this.rbody.position, new Vector3(1, 1, 0)));
            this.rbody.angularVelocity = Vector3.zero;
            this.rbody.velocity = Vector3.Scale(this.rbody.velocity, new Vector3(1, 1, 0));
            i = 0;
            while (i < Atom.all.Length)
            {
                Vector3 diff = Atom.all[i].rbody.position - this.rbody.position;
                if (diff.sqrMagnitude < (13 * 13))
                {
                    float dist = diff.magnitude;
                    Vector3 dir = diff / dist;
                    if (diff.sqrMagnitude < Atom.all[i].electrons.transform.localScale.x)
                    {
                        if (Atom.all[i].e < Atom.all[i].p)
                        {
                            Atom.all[i].e++;
                            if (Atom.all[i].isPlayer)
                            {
                                UserAtom.e = Atom.all[i].e;
                            }
                            this.transform.position = new Vector3(1000, 1000, 0);
                        }
                    }
                    int charge = -1;
                    int chargeDiff = Atom.all[i].p == Atom.all[i].e ? 0 : (Atom.all[i].p < Atom.all[i].e ? charge : -2 * charge);
                    if (dist != 0)
                    {
                        this.rbody.AddForce(((dir * Mathf.Min(1f / (dist * dist), 3)) * this.elecForce) * chargeDiff);
                    }
                }
                i++;
            }
            i = 0;
            while (i < this.all.Length)
            {
                Vector3 diff2 = this.all[i].position - this.rbody.position;
                if (((this.all[i] != this.rbody) && (diff2.sqrMagnitude < (13 * 13))) && (this.all[i].velocity != Vector3.zero))
                {
                    float dist2 = diff2.magnitude;
                    Vector3 dir2 = diff2 / dist2;
                    if (dist2 != 0)
                    {
                        this.rbody.AddForce((dir2 * Mathf.Min(1f / (dist2 * dist2), 3)) * -this.elecForce);
                    }
                }
                i++;
            }
        }
    }

    public VagabondElectron()
    {
        this.distLimit = 13;
        this.cellSize = 3;
        this.elecForce = 1f;
    }

}