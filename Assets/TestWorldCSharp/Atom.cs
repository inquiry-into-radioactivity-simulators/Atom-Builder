using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Atom : MonoBehaviour
{
    public int p;
    public int n;
    public int e;
    public bool isPlayer;
    public static Atom[] all;
    public Rigidbody rbody;
    public GameObject field;
    private Color fieldColorBlue;
    private Color fieldColorRed;
    public GameObject nucleus;
    public ParticleSystem electrons;
    private ParticleSystem electrons2;
    public GameObject pPositron;
    public GameObject pElectron;
    public GameObject pNeutron;
    public GameObject pNucleus;
    public GameObject pGamma;
    public GameObject insetCamera;
    public float moveForce;
    public float minDrag;
    public float maxDrag;
    public float elecForce;
    public float minMass;
    public float maxMass;
    
    public Material[] electronMaterials;
    public float minFieldSize;
    public float maxFieldSize;
    private float minFieldAmt;
    private float maxFieldAmt;
    public float minNucSize;
    public float maxNucSize;
    public float minPScale;
    public float maxPScale;
    public float minEScale;
    public float maxEScale;
    private float originalETrans1;
    private float originalETrans2;
    private string last;
    public int origP;
    public int origN;
    public int origE;
    private SphereCollider coll;
    public Transform trans;
    public static Atom playerInstance;
    public virtual void Start()
    {
        this.fieldColorRed = new Color(1f, 15f/255, 0f);
        this.fieldColorBlue = new Color(0f, 20f/255, 1f);

        if (this.isPlayer)
        {
            Atom.playerInstance = this;
            this.p = UserAtom.p;
            this.n = UserAtom.n;
            this.e = UserAtom.e;
        }
        this.origP = this.p;
        this.origN = this.n;
        this.origE = this.e;
        this.trans = this.transform;

        var shapeModule = this.electrons.shape;
        shapeModule.radius = 0.5f;

        var mainModule = this.electrons.main;
        mainModule.prewarm = true;

        this.electrons2 = UnityEngine.Object.Instantiate(this.electrons.gameObject).GetComponent<ParticleSystem>();
        this.electrons2.transform.parent = transform;
        this.electrons2.transform.localScale = Vector3.one;
        this.electrons2.transform.localPosition = Vector3.zero;

        this.electrons.GetComponent<Renderer>().material = electronMaterials[0];
        this.electrons2.GetComponent<Renderer>().material = electronMaterials[1];

        //shapeModule.position = new Vector3(0f,0f,0.5f);

        this.originalETrans1 = this.electrons.GetComponent<Renderer>().material.GetColor("_TintColor").a;
        this.originalETrans2 = this.electrons2.GetComponent<Renderer>().material.GetColor("_TintColor").a;
        //this.electrons.GetComponent<Renderer>().material.

        this.coll = (SphereCollider) this.electrons.gameObject.AddComponent(typeof(SphereCollider));
        this.coll.material = (PhysicMaterial)Resources.Load("ColliderMaterial");
        this.rbody.useGravity = false;
        CollideReport r = (CollideReport) this.rbody.gameObject.AddComponent(typeof(CollideReport));
        r.atom = this;
    }

    public virtual void OnEnable()
    {
        Atom.Add(this);
    }

    public virtual void OnDisable()
    {
        Atom.Remove(this);
    }

    public virtual void Decay()
    {
        DecayMode mode = AtomChart.elements[this.p - 1].nuclides[this.n].decayMode;
        GameObject p0 = null;
        GameObject insO = null;
        InsetCamera ins = null;
        float i = 0f;
        int eOut = 0;
        int toal = 0;
        GameObject objOut = null;
        Vector3[] dirs = new Vector3[8];
        i = 0;
        while (i < 4)
        {
            Vector3 derp = Quaternion.AngleAxis(Mathf.Lerp(130f, 215f, i / 4f), Vector3.forward) * Vector3.right;
            dirs[Mathf.RoundToInt(i)] = derp;
            dirs[Mathf.RoundToInt(i) + 4] = -derp;
            i++;
        }
        float ri = Mathf.Round(Random.Range(0, 4));
        Vector3 rDir = dirs[Mathf.RoundToInt(ri)] * (Random.value > 0.5f ? 1 : -1);
        float gi = ri;
        while ((gi == ri) || (gi == (ri + 4)))
        {
            gi = Mathf.Round(Random.Range(0, 8));
        }
        bool onlyGamma = AtomChart.elements[this.p - 1].nuclides[this.n].onlyGamma;
        if (AtomChart.elements[this.p - 1].nuclides[this.n].gamma)
        {
            p0 = UnityEngine.Object.Instantiate(this.pGamma, this.transform.position, this.transform.rotation);
            ((Debris) p0.GetComponent(typeof(Debris))).velocity = dirs[Mathf.RoundToInt(gi)] * 12;
            insO = UnityEngine.Object.Instantiate(this.insetCamera);
            ins = (InsetCamera) insO.GetComponent(typeof(InsetCamera));
            ins.targetObject = p0.transform;
            ins.pTexture = (Texture2D)p0.GetComponent<Renderer>().material.mainTexture;
            ins.pCharge = 0;
            ins.pSpeed = 7;
            ins.p = 0;
            ins.n = 0;
        }
        //DecayMode.ECPE, DecayMode.Beta, DecayMode.Alpha, DecayMode.Proton, DecayMode.Neutron, DecayMode.Fission
        if (mode == DecayMode.ECPE)
        {
            this.p--;
            this.n++;
            p0 = UnityEngine.Object.Instantiate(this.pPositron, this.transform.position, this.transform.rotation);
            ((Debris) p0.GetComponent(typeof(Debris))).velocity = rDir * 7;
            insO = UnityEngine.Object.Instantiate(this.insetCamera);
            ins = (InsetCamera) insO.GetComponent(typeof(InsetCamera));
            ins.targetObject = p0.transform;
            ins.pTexture = (Texture2D)p0.GetComponent<Renderer>().material.mainTexture;
            ins.pCharge = 1;
            ins.pSpeed = 1.5f;
            ins.p = 0;
            ins.n = 0;
            ins.e = -1;
            if (this.e > 0)
            {
                this.RipElectron(rDir);
            }
            if ((this.e > 0) && (Random.value > 0.5f))
            {
                this.RipElectron(rDir);
            }
        }
        else
        {
            if (mode == DecayMode.Beta)
            {
                this.p++;
                this.n--;
                p0 = UnityEngine.Object.Instantiate(this.pElectron, this.transform.position, this.transform.rotation);
                ((Debris) p0.GetComponent(typeof(Debris))).velocity = rDir * (onlyGamma ? 3.5f : 7f);
                if (!onlyGamma)
                {
                    insO = UnityEngine.Object.Instantiate(this.insetCamera);
                    ins = (InsetCamera) insO.GetComponent(typeof(InsetCamera));
                    ins.targetObject = p0.transform;
                    ins.pTexture = (Texture2D)p0.GetComponent<Renderer>().material.mainTexture;
                    ins.pCharge = -1;
                    ins.pSpeed = 1.5f;
                    ins.p = 0;
                    ins.n = 0;
                    ins.e = 1;
                }
                if (this.e > 0)
                {
                    this.RipElectron(rDir);
                }
                if ((this.e > 0) && (Random.value > 0.5f))
                {
                    this.RipElectron(rDir);
                }
            }
            else
            {
                if (mode == DecayMode.Alpha)
                {
                    this.p = this.p - 2;
                    this.n = this.n - 2;
                    p0 = UnityEngine.Object.Instantiate(this.pNucleus, this.transform.position, this.transform.rotation);
                    ((Debris) p0.GetComponent(typeof(Debris))).velocity = rDir * 4;
                    insO = UnityEngine.Object.Instantiate(this.insetCamera);
                    ins = (InsetCamera) insO.GetComponent(typeof(InsetCamera));
                    ins.targetObject = p0.transform;
                    ins.pTexture = (Texture2D)p0.GetComponent<Renderer>().material.mainTexture;
                    ins.pCharge = 2;
                    ins.pSpeed = 0.5f;
                    ins.p = 2;
                    ins.n = 2;
                    toal = (int) (eOut = (int) Mathf.Min(Mathf.Max(1, Mathf.Ceil(this.e * 0.07f)), this.e));
                    if ((this.p == 4) && (this.n == 4))
                    {
                        toal = eOut = 2;
                    }
                    while (eOut > 0)
                    {
                        objOut = this.RipElectron(rDir);
                        foreach (Transform t in objOut.transform)
                        {
                            t.GetComponent<Renderer>().material.color = Color.Lerp(Color.clear, t.GetComponent<Renderer>().material.color, 8f / toal);
                        }
                        eOut--;
                    }
                }
                else
                {
                    if (mode == DecayMode.Proton)
                    {
                        this.p = this.p - 1;
                        p0 = UnityEngine.Object.Instantiate(this.pNucleus, this.transform.position, this.transform.rotation);
                        ((Debris) p0.GetComponent(typeof(Debris))).velocity = rDir * 6;
                        insO = UnityEngine.Object.Instantiate(this.insetCamera);
                        ins = (InsetCamera) insO.GetComponent(typeof(InsetCamera));
                        ins.targetObject = p0.transform;
                        ins.pTexture = (Texture2D)p0.GetComponent<Renderer>().material.mainTexture;
                        ins.pCharge = 1;
                        ins.pSpeed = 0.8f;
                        ins.p = 1;
                        ins.n = 0;
                        toal = (int) (eOut = (int) Mathf.Min(Mathf.Max(1, Mathf.Round(this.e * 0.1f)), this.e));
                        while (eOut > 0)
                        {
                            objOut = this.RipElectron(rDir);
                            foreach (Transform t in objOut.transform)
                            {
                                t.GetComponent<Renderer>().material.color = Color.Lerp(Color.clear, t.GetComponent<Renderer>().material.color, 4f / toal);
                            }
                            eOut--;
                        }
                    }
                    else
                    {
                        if (mode == DecayMode.Neutron)
                        {
                            this.n = this.n - 1;
                            p0 = UnityEngine.Object.Instantiate(this.pNeutron, this.transform.position, this.transform.rotation);
                            ((Debris) p0.GetComponent(typeof(Debris))).velocity = rDir * 6;
                            insO = UnityEngine.Object.Instantiate(this.insetCamera);
                            ins = (InsetCamera) insO.GetComponent(typeof(InsetCamera));
                            ins.targetObject = p0.transform;
                            ins.pTexture = (Texture2D)p0.GetComponent<Renderer>().material.mainTexture;
                            ins.pCharge = 0;
                            ins.pSpeed = 0.8f;
                            ins.p = 0;
                            ins.n = 1;
                        }
                        else
                        {
                            if (mode == DecayMode.Fission) //  || mode == DecayMode.Unknown
                            {
                                float eAmt = Mathf.Round(this.e * 0.1f) * 2;
                                float prod0 = 0.5f + (Random.Range(0.1f, 0.2f) * (Random.value > 0.5f ? 1 : -1));
                                int pool = 3;
                                this.n = this.n - pool;
                                float p0p = this.p * prod0;
                                this.p = (int) (this.p - p0p);
                                float p0n = this.n * prod0;
                                this.n = (int) (this.n - p0n);
                                DecayMode p0dm = AtomChart.elements[Mathf.RoundToInt(p0p) - 1].nuclides[Mathf.RoundToInt(p0n)].decayMode;
                                DecayMode p1dm = AtomChart.elements[this.p - 1].nuclides[this.n].decayMode;
                                int itr = 0;
                                while ((itr < 100) && ((p0dm == DecayMode.ECPE) || (p1dm == DecayMode.ECPE)))
                                {
                                    itr++;
                                    if (p0dm == DecayMode.ECPE)
                                    {
                                        if (AtomChart.elements[this.p - 1].nuclides[this.n - 1].decayMode != DecayMode.ECPE)
                                        {
                                            this.n--;
                                            p0n++;
                                        }
                                        else
                                        {
                                            pool--;
                                            p0n++;
                                        }
                                    }
                                    if (p0dm == DecayMode.ECPE)
                                    {
                                        if (AtomChart.elements[Mathf.RoundToInt(p0p) - 1].nuclides[Mathf.RoundToInt(p0n) - 1].decayMode != DecayMode.ECPE)
                                        {
                                            p0n--;
                                            this.n++;
                                        }
                                        else
                                        {
                                            pool--;
                                            this.n++;
                                        }
                                    }
                                    p0dm = AtomChart.elements[Mathf.RoundToInt(p0p) - 1].nuclides[Mathf.RoundToInt(p0p)].decayMode;
                                    p1dm = AtomChart.elements[this.p - 1].nuclides[this.n].decayMode;
                                }
                                Debug.Log((((("pool:" + pool) + " p0dm: ") + p0dm) + "  p1dm: ") + p1dm);
                                if (pool < 1)
                                {
                                    pool = 1;
                                    Debug.Log("Something wrong with fission neutron exchange loop: negative pool result!!!!");
                                }
                                if (itr > 50)
                                {
                                    Debug.Log("Something wrong with fission neutron exchange loop: insane amount of iterations!!");
                                }
                                p0 = UnityEngine.Object.Instantiate(this.pNucleus, this.transform.position, this.transform.rotation);
                                ((Debris) p0.GetComponent(typeof(Debris))).velocity = dirs[Mathf.RoundToInt(ri)] * 4;
                                ((Debris) p0.GetComponent(typeof(Debris))).eRenderer.enabled = true;
                                insO = UnityEngine.Object.Instantiate(this.insetCamera);
                                ins = (InsetCamera) insO.GetComponent(typeof(InsetCamera));
                                ins.targetObject = p0.transform;
                                ins.pTexture = (Texture2D)p0.GetComponent<Renderer>().material.mainTexture;
                                ins.pCharge = 1;
                                ins.pSpeed = 0.3f;
                                ins.p = (int) p0p;
                                ins.n = (int) p0n;
                                ins.e = (int) (eAmt * prod0);
                                p0 = UnityEngine.Object.Instantiate(this.pNucleus, this.transform.position, this.transform.rotation);
                                ((Debris) p0.GetComponent(typeof(Debris))).velocity = dirs[Mathf.RoundToInt(ri) + 4] * 4;
                                ((Debris) p0.GetComponent(typeof(Debris))).eRenderer.enabled = true;
                                insO = UnityEngine.Object.Instantiate(this.insetCamera);
                                ins = (InsetCamera) insO.GetComponent(typeof(InsetCamera));
                                ins.targetObject = p0.transform;
                                ins.pTexture = (Texture2D)p0.GetComponent<Renderer>().material.mainTexture;
                                ins.pCharge = 1;
                                ins.pSpeed = 0.3f;
                                ins.p = this.p;
                                ins.n = this.n;
                                ins.e = (int) (eAmt * (1 - prod0));
                                int[] nis = new int[pool];
                                int added = 0;
                                i = 0;
                                while (i < 8)
                                {
                                    if (((i != ri) && (i != (ri + 4))) && (i != gi))
                                    {
                                        if (added < pool)
                                        {
                                            nis[added] = (int) i;
                                            added++;
                                        }
                                        else
                                        {
                                            if (Random.value > 0.5f)
                                            {
                                                nis[Mathf.RoundToInt(Random.Range(0, pool - 1))] = (int) i;
                                            }
                                        }
                                    }
                                    i++;
                                }
                                i = 0;
                                while (i < 3)
                                {
                                    p0 = UnityEngine.Object.Instantiate(this.pNeutron, this.transform.position, this.transform.rotation);
                                    ((Debris) p0.GetComponent(typeof(Debris))).velocity = dirs[nis[Mathf.RoundToInt(i)]] * 9;
                                    insO = UnityEngine.Object.Instantiate(this.insetCamera);
                                    ins = (InsetCamera) insO.GetComponent(typeof(InsetCamera));
                                    ins.targetObject = p0.transform;
                                    ins.pTexture = (Texture2D)p0.GetComponent<Renderer>().material.mainTexture;
                                    ins.pCharge = 0;
                                    ins.pSpeed = 0.8f;
                                    ins.p = 0;
                                    ins.n = 1;
                                    i++;
                                }
                                toal = (int) (eOut = (int) (this.e - (eAmt * 2)));
                                while (eOut > 0)
                                {
                                    objOut = this.RipElectron();
                                    foreach (Transform t in objOut.transform)
                                    {
                                        t.GetComponent<Renderer>().material.color = Color.Lerp(Color.clear, t.GetComponent<Renderer>().material.color, 10f / toal);
                                    }
                                    eOut--;
                                }
                                TestGUI.cullBackAndCounter = true;
                                UnityEngine.Object.Destroy(this.gameObject);
                            }
                            else
                            {
                                Debug.Log(" oops: " + mode);
                            }
                        }
                    }
                }
            }
        }
        UserAtom.p = this.p;
        UserAtom.n = this.n;
        UserAtom.e = this.e;
    }

    public virtual GameObject RipElectron(Vector3 smack)
    {
        GameObject p = UnityEngine.Object.Instantiate(this.pElectron, this.transform.position, this.transform.rotation);
        Debris deb = (Debris) p.GetComponent(typeof(Debris));
        deb.velocity = ((Quaternion.AngleAxis(Random.Range(0, 60f) * (Random.value > 0.5f ? -1 : 1), Vector3.forward) * smack) * 3) * Random.Range(0.8f, 1.2f);
        deb.timeout = 4;
        this.e--;
        return p;
    }

    public virtual GameObject RipElectron()
    {
        GameObject p = UnityEngine.Object.Instantiate(this.pElectron, this.transform.position, this.transform.rotation);
        Debris deb = (Debris) p.GetComponent(typeof(Debris));
        deb.SetSpeed(3);
        deb.timeout = 4;
        this.e--;
        return p;
    }

    public virtual void Update()
    {
        float fieldAmt = Mathf.Log(Mathf.Abs(this.p - this.e)) * 0.22f;
        float colorAmt = Mathf.Lerp(this.minFieldAmt, this.maxFieldAmt, fieldAmt);
        float fieldSize = Mathf.Lerp(this.minFieldSize, this.maxFieldSize, fieldAmt);
        this.field.GetComponent<Renderer>().enabled = this.p != this.e;
        float alpha1 = (190f / 256) * colorAmt;
        Color theColor = this.p > this.e ? fieldColorRed : fieldColorBlue;
        this.field.GetComponent<Renderer>().material.color = new Color(theColor.r, theColor.g, theColor.b, alpha1);
        this.field.transform.localScale = Vector3.one * fieldSize;
        float nucAmt = UnityScript.Lang.UnityBuiltins.parseFloat(this.p) / Ngnome.MaxProtons();
        this.nucleus.transform.localScale = (Vector3.one * Mathf.Lerp(this.minNucSize, this.maxNucSize, nucAmt)) * 0.5f;
        this.rbody.mass = Mathf.Lerp(this.minMass, this.maxMass, nucAmt);
        this.rbody.drag = Mathf.Lerp(this.minDrag, this.maxDrag, nucAmt) * Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical")));
        this.coll.isTrigger = this.e == 0;
        float minSize = 31f;
        float maxSize = 260f;
        float ionAmt = UnityScript.Lang.UnityBuiltins.parseFloat(this.e) / this.p;
        if ((ionAmt > 0) && (ionAmt < 0.2f))
        {
            ionAmt = 0.2f;
        }
        float sizeAmt = (UnityScript.Lang.UnityBuiltins.parseFloat(AtomChart.elements[this.p - 1].size) - minSize) / maxSize;

        var electronsScale = Vector3.one * Mathf.Lerp(this.minEScale, this.maxEScale, sizeAmt * ionAmt);
        this.electrons.transform.localScale = electronsScale;
        this.electrons2.transform.localScale = electronsScale;

        float electronsStartSize = Mathf.Lerp(this.minEScale, this.maxEScale, sizeAmt * ionAmt);
        var mainModule = this.electrons.main;
        var mainModule2 = this.electrons2.main;
        mainModule.startSize = new ParticleSystem.MinMaxCurve(electronsStartSize, electronsStartSize);
        mainModule2.startSize = new ParticleSystem.MinMaxCurve(electronsStartSize, electronsStartSize);

        Color c1 = this.electrons.GetComponent<Renderer>().material.GetColor("_TintColor");
        Color c2 = this.electrons2.GetComponent<Renderer>().material.GetColor("_TintColor");
        this.electrons.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(c1.r, c1.g, c1.b, (this.originalETrans1 * Mathf.Clamp01((1f - sizeAmt) + 0.5f)) * ionAmt));
        this.electrons2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(c2.r, c2.g, c2.b, (this.originalETrans2 * Mathf.Clamp01((1f - sizeAmt) + 0.5f)) * ionAmt));
        
        if(!this.electrons.isPlaying)  {
            this.electrons.Play();
            this.electrons2.Play();
        }

        string hash = (((("" + this.p) + "") + this.n) + "") + this.e;
        if (this.last != hash)
        {
            // TODO need to replace this logic??
            // ParticleSystem.Particle[] noneParticles = new ParticleSystem.Particle[0];
            // this.electrons.SetParticles(noneParticles, 0);
            // UnityEngine.Object.Destroy((ParticlesRightAway) this.electrons.GetComponent(typeof(ParticlesRightAway)));
            // this.electrons.gameObject.AddComponent(typeof(ParticlesRightAway));
            this.last = hash;
        }
    }

    public virtual void Hit(Collision collision)
    {
        if (collision.rigidbody)
        {
            CollideReport other = (CollideReport) collision.rigidbody.gameObject.GetComponent(typeof(CollideReport));
            if (other)
            {
                this.ElectronCheck(other.atom);
            }
        }
    }

    public virtual bool ElectronCheck(Atom other)
    {
        if (this.isPlayer)
        {
            bool hit = false;
            if ((other.p > other.e) && (this.p < this.e))
            {
                other.e = other.e + 1;
                this.e = this.e - 1;
                hit = true;
            }
            else
            {
                if ((other.p < other.e) && (this.p > this.e))
                {
                    other.e = other.e - 1;
                    this.e = this.e + 1;
                    hit = true;
                }
            }
            if (hit)
            {
                UserAtom.p = this.p;
                UserAtom.n = this.n;
                UserAtom.e = this.e;
            }
            return hit;
        }
        else
        {
            return false;
        }
    }

    public virtual void LateUpdate()
    {
        this.rbody.transform.rotation = Quaternion.identity;
    }

    public virtual void FixedUpdate()
    {
        if (this.isPlayer)
        {
            this.rbody.MoveRotation(Quaternion.identity);
            this.rbody.MovePosition(Vector3.Scale(this.rbody.position, new Vector3(1, 1, 0)));
            this.rbody.angularVelocity = Vector3.zero;
            this.rbody.velocity = Vector3.Scale(this.rbody.velocity, new Vector3(1, 1, 0));
            Vector3 direction = Vector3.zero;
            if ((Input.acceleration.x != 0) || (Input.acceleration.y != 0))
            {
                direction = new Vector3(Input.acceleration.x, Input.acceleration.y, 0);
            }
            else
            {
                direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized;
            }
            //var moveForceClamp = rbody.velocity.magnitude * Vector3.Dot(rbody.velocity.normalized, direction);
            this.rbody.AddForce(direction * this.moveForce);
            int i = 0;
            while (i < Atom.all.Length)
            {
                Vector3 diff = Atom.all[i].rbody.position - this.rbody.position;
                if ((Atom.all[i] != this) && (diff.sqrMagnitude < (13 * 13)))
                {
                    float dist = diff.magnitude;
                    Vector3 dir = diff / dist;
                    if (diff.sqrMagnitude < Atom.all[i].electrons.transform.localScale.x)
                    {
                        if (this.ElectronCheck(Atom.all[i]))
                        {
                            this.transform.position = Atom.all[i].transform.position + ((dir * Atom.all[i].electrons.transform.localScale.x) * -2);
                        }
                    }
                    int charge = Mathf.Min(Mathf.Abs(this.p - this.e) + Mathf.Abs(Atom.all[i].p - Atom.all[i].e), 5);
                    int chargeDiff = (this.p == this.e) || (Atom.all[i].p == Atom.all[i].e) ? 0 : (((this.p > this.e) && (Atom.all[i].p < Atom.all[i].e)) || ((this.p < this.e) && (Atom.all[i].p > Atom.all[i].e)) ? charge : -2 * charge);
                    this.rbody.AddForce(((dir * Mathf.Min(1f / (dist * dist), 3)) * this.elecForce) * chargeDiff);
                }
                i++;
            }
        }
    }

    public static void Add(Atom a)
    {
        Atom[] newAll = new Atom[Atom.all.Length + 1];
        int i = 0;
        while (i < Atom.all.Length)
        {
            newAll[i] = Atom.all[i];
            i++;
        }
        newAll[Atom.all.Length] = a;
        Atom.all = newAll;
    }

    public static void Remove(Atom a)
    {
        Atom[] newAll = new Atom[Atom.all.Length - 1];
        int added = 0;
        int i = 0;
        while (i < Atom.all.Length)
        {
            if (Atom.all[i] != a)
            {
                newAll[added] = Atom.all[i];
                added++;
            }
            i++;
        }
        Atom.all = newAll;
    }

    public Atom()
    {
        this.moveForce = 10f;
        this.minDrag = 6f;
        this.maxDrag = 1f;
        this.elecForce = 10f;
        this.minMass = 0.7f;
        this.maxMass = 3f;
        this.minFieldSize = 3f;
        this.maxFieldSize = 10f;
        this.minFieldAmt = 0.3f;
        this.maxFieldAmt = 1f;
        this.minNucSize = 0.07f;
        this.maxNucSize = 0.14f;
        this.minPScale = 0.3f;
        this.maxPScale = 1f;
        this.minEScale = 0.121154f;
        this.maxEScale = 0.9f;
        this.last = "";
        
    }

    static Atom()
    {
        Atom.all = new Atom[0];
    }

}