using UnityEngine;
using System.Collections;

[System.Serializable]
public class NucleusRank : object
{
    public NucleusPoint[] points;
}
[System.Serializable]
public class NucleusPoint : object
{
    public int index;
    public int[] neighbors;
    public Vector3 pos;
    public float dist;
    public int distRank;
    public PType state; // 0 = empty, 1 = proton, 2 = neutron
    public int tempIndex;
    public float plane;
    public NucleusPoint()
    {
        this.index = -1;
        this.state = PType.Empty;
    }

}
[System.Serializable]
public class Nucleus : DraggableType
{
    public int p;
    public int n;
    public float colliderRadius;
    public float particleRadius;
    public float overallScale;
    public bool flipPositions;
    public float changeInterval;
    public float movementSpeed;
    public float movementAmount;
    public NucleusPoint[] points;
    public NucleusRank[] ranks;
    public float zSpacingInMesh;
    public GameObject particlePrefab;
    public CreatorBackground background;
    public Orbitals orbitals;
    public AtomChart chart;
    public Color protonColor;
    public Color neutronColor;
    public static Transform recentParticle;
    private NucleusDisplay display;
    private Transform colliderParent;
    private GameObject[] colliders;
    private int mouseOver;
    private int mouseDown;
    private float randomizeOrder;
    private float nextChangeTime;
    private int lastP;
    private int lastN;
    public virtual void Start()
    {
        this.p = UserAtom.p;
        this.n = UserAtom.n;
        this.display = (NucleusDisplay) this.GetComponent(typeof(NucleusDisplay));
        GameObject chObj = new GameObject("Colliders");
        this.colliderParent = chObj.transform;
        this.colliderParent.position = this.transform.position;
        this.colliderParent.parent = this.transform;
        this.CopyPoints();
        this.colliders = new GameObject[this.points.Length];
        int i = 0;
        while (i < this.points.Length)
        {
            GameObject cObj = new GameObject("Collider");
            cObj.transform.parent = this.colliderParent;
            cObj.transform.localPosition = this.points[i].pos;
            SphereCollider coll = (SphereCollider) cObj.AddComponent(typeof(SphereCollider));
            coll.radius = this.colliderRadius;
            DraggableCollider script = (DraggableCollider) cObj.AddComponent(typeof(DraggableCollider));
            script.id = i;
            script.parent = this;
            this.colliders[i] = cObj;
            i++;
        }
        this.Rebuild();
    }

    public virtual void Update()
    {
        this.transform.rotation = Quaternion.LookRotation(new Vector3(1, 0.5f, 0.5f) + ((SmoothRandom.GetVector3(this.movementSpeed) - (Vector3.one * 0.3f)) * this.movementAmount));
        if ((this.mouseOver == -1) && (Time.time > this.nextChangeTime))
        {
            int curP = this.p;
            int curN = this.n;
            
            var displayNucleons = this.display.GetNucleons();
            if(displayNucleons != null && displayNucleons.Length > 0) {
                int i = 0;
                while (i < displayNucleons.Length)
                {
                    bool isProton = ((Random.value < (UnityScript.Lang.UnityBuiltins.parseFloat(this.p) / (this.p + this.n))) && (curP > 0)) || (curN == 0);
                    displayNucleons[i].color = isProton ? this.protonColor : this.neutronColor;
                    if (isProton)
                    {
                        curP--;
                    }
                    else
                    {
                        curN--;
                    }
                    i++;
                }
                this.display.SetNucleons(displayNucleons);
            }
            
            this.nextChangeTime = Time.time + this.changeInterval;
        }
        if ((this.lastP != this.p) || (this.lastN != this.n))
        {
            this.Rebuild();
            this.lastP = this.p;
            this.lastN = this.n;
        }
    }

    public override Draggable OnDragEvent(DragEventType type, int id, Draggable current, DraggableType target)
    {
        if (type == DragEventType.Over)
        {
            this.mouseOver = id;
            this.Rebuild();
        }
        else
        {
            if (type == DragEventType.Down)
            {
                int newp = this.p;
                int newn = this.n;
                if (this.points[id].state == PType.P) {
                    newp--;
                } else {
                    newn--;
                }

                if (newp > 0 && AtomChart.elements[newp - 1].nuclides[newn] != null) {
                    this.p = newp;
                    this.n = newn;
                    GameObject obj = new GameObject("Draggable");
                    Draggable_Nucleon dr = (Draggable_Nucleon) obj.AddComponent(typeof(Draggable_Nucleon));
                    dr.type = this.points[id].state;
                    this.randomizeOrder = Random.value;
                    this.mouseDown = id;
                    this.Rebuild();
                    return dr;
                }
            }
            else
            {
                if (type == DragEventType.Up)
                {
                    if (target && target.CanDrop(current))
                    {
                        target.Drop(current);
                        if ((target != this) && ((UserAtom.p + 1) <= UserAtom.e))
                        {
                            Draggable_Nucleon nuc = current as Draggable_Nucleon;
                            if (nuc.type == PType.P)
                            {
                                this.orbitals.RemoveElectron(false);
                                if ((target as Orbitals) || ((target as CreatorBackground) && (this.background.mouseOver == 0)))
                                {
                                    this.StartCoroutine(this.ElectronToProton());
                                }
                                else
                                {
                                    if (target as TrayGUI)
                                    {
                                        this.StartCoroutine(this.ElectronToTray());
                                    }
                                }
                            }
                        }
                        current.Die();
                    }
                    else
                    {
                        if (current)
                        {
                            current.FailDrag();
                            this.Drop(current);
                        }
                    }
                    this.randomizeOrder = Random.value;
                    this.mouseDown = -1;
                    this.Rebuild();
                    return null;
                }
                else
                {
                    if (type == DragEventType.Exit)
                    {
                        this.mouseOver = -1;
                        this.Rebuild();
                    }
                }
            }
        }
        return current;
    }

    public virtual IEnumerator ElectronToTray()
    {
        TrayGUI[] allTrays = (TrayGUI[]) UnityEngine.Object.FindObjectsOfType(typeof(TrayGUI));
        TrayGUI theTray = null;
        int i = 0;
        while (i < allTrays.Length)
        {
            if (allTrays[i].acceptTypes[2] == 1)
            {
                theTray = allTrays[i];
            }
            i++;
        }
        Vector2 origin = DragNDrop.WorldToScreen(Vector3.right * this.orbitals.shells[this.orbitals.outerShell].radius);
        GameObject obj = new GameObject("Draggable");
        Draggable_Nucleon dr = (Draggable_Nucleon) obj.AddComponent(typeof(Draggable_Nucleon));
        dr.type = PType.E;
        float time = 0.8f;
        float startTime = Time.time;
        while (Time.time < (startTime + time))
        {
            float amt = (Time.time - startTime) / time;
            dr.position = (origin * (1f - amt)) + ((theTray.position - new Vector2(0, ((UnityScript.Lang.UnityBuiltins.parseFloat(theTray.dimensions.y) - 0.5f) * theTray.itemSize.y) * 0.5f)) * amt);
            yield return null;
        }
        theTray.Drop(dr);
        dr.Die();
    }

    public virtual IEnumerator ElectronToProton()
    {
        Transform target = Nucleus.recentParticle;
        Vector3 pos = Vector3.right * this.orbitals.shells[this.orbitals.outerShell].radius;
        GameObject particleObj = UnityEngine.Object.Instantiate(this.particlePrefab, pos, Quaternion.identity);
        particleObj.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Electron_TIF");
        float time = 0.8f;
        CreatorParticle mover = (CreatorParticle) particleObj.GetComponent(typeof(CreatorParticle));
        mover.lifetime = time;
        float startTime = Time.time;
        while (Time.time < (startTime + time))
        {
            particleObj.transform.position = Vector3.Lerp(pos, target.position, (Time.time - startTime) / time);
            yield return null;
        }
        target.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Hydrogen_TIF");
        target.localScale = target.localScale * 2.2f;
    }

    public override bool CanDrop(Draggable d)
    {
        Draggable_Nucleon nuc = d as Draggable_Nucleon;
        if (nuc)
        {
            PType type = nuc.type;
            int newp = this.p;
            int newn = this.n;
            if (type == PType.P)
            {
                newp++;
            }
            else
            {
                if (type == PType.N)
                {
                    newn++;
                }
            }
            return (newp <= Ngnome.MaxProtons()) && (AtomChart.elements[newp - 1].nuclides[newn] != null) ? true : false;
        }
        return false;
    }

    public override void Drop(Draggable d)
    {
        Draggable_Nucleon nuc = d as Draggable_Nucleon;
        if (nuc.type == PType.P)
        {
            this.p++;
            this.Rebuild();
        }
        else
        {
            if (nuc.type == PType.N)
            {
                this.n++;
                this.Rebuild();
            }
            else
            {
                if (nuc.type == PType.E)
                {
                    this.background.Drop(d);
                }
            }
        }
    }

    public virtual void Rebuild()
    {
        //Debug.Log(Time.time);
        UserAtom.p = this.p;
        UserAtom.n = this.n;
        int i = 0;
        int ii = 0;
        i = 0;
        while (i < this.points.Length)
        {
            this.colliders[i].SetActive(false);
            this.points[i].state = PType.Empty;
            i++;
        }
        int curRank = 0;
        int curP = this.p;
        int curN = this.n;
        Vector3 centerPoint = Vector3.zero;
        int added = 0;
        NucleusDisplay.Nucleon[] displayNucleons = new NucleusDisplay.Nucleon[this.p + this.n];
        while ((curP > 0) || (curN > 0))
        {
            bool nextIsProton = (curP > 0) && ((curN == 0) || ((((curP + 0.01f) / (curN + 0.01f)) + ((this.randomizeOrder - 0.5f) * 0.2f)) > ((this.p + 0.01f) / (this.n + 0.01f))));
            int maxNeighbors = -1;
            float maxRatio = -1f;
            int bestFit = -1;
            int availableInRank = 0;
            i = 0;
            while (i < this.ranks[curRank].points.Length)
            {
                NucleusPoint point = this.ranks[curRank].points[i];
                if (point.state == PType.Empty)
                {
                    availableInRank++;
                    float prs = 0f;
                    float nus = 0f;
                    ii = 0;
                    while (ii < point.neighbors.Length)
                    {
                        if ((point.neighbors[ii] != -1) && (this.points[point.neighbors[ii]].state == PType.P))
                        {
                            prs++;
                        }
                        if ((point.neighbors[ii] != -1) && (this.points[point.neighbors[ii]].state == PType.N))
                        {
                            nus++;
                        }
                        ii++;
                    }
                    if (maxNeighbors <= (prs + nus))
                    {
                        if (maxNeighbors < (prs + nus))
                        {
                            maxNeighbors = (int) (prs + nus);
                            bestFit = point.index;
                        }
                        float ratio = nextIsProton ? (nus + 0.01f) / (prs + 0.01f) : (prs + 0.01f) / (nus + 0.01f);
                        if (maxRatio < ratio)
                        {
                            maxRatio = ratio;
                            bestFit = point.index;
                        }
                    }
                }
                i++;
            }
            this.points[bestFit].state = nextIsProton ? PType.P : PType.N;
            this.colliders[bestFit].SetActive(true);//GameObject
            NucleusDisplay.Nucleon np = new NucleusDisplay.Nucleon();
   
            np.pos = this.points[bestFit].pos;
            np.color = nextIsProton ? this.protonColor : this.neutronColor;
            displayNucleons[added] = np;
            added++;
            centerPoint = centerPoint + this.points[bestFit].pos;
            if (nextIsProton)
            {
                curP--;
            }
            else
            {
                curN--;
            }
            if (availableInRank == 1)
            {
                curRank++;
            }
        }
        centerPoint = centerPoint / (this.p + this.n);
        this.transform.position = -centerPoint * this.overallScale;
        this.display.SetNucleons(displayNucleons);
        this.transform.localScale = Vector3.one * this.overallScale;
    }

    public virtual void CopyPoints()
    {
        int i = 0;
        int ii = 0;
        NucleusPoint[] ps = new NucleusPoint[NucleusSpacing.points.Length];
        i = 0;
        while (i < ps.Length)
        {
            NucleusPoint p = new NucleusPoint();
            p.index = i;
            p.neighbors = new int[12];
            ii = 0;
            while (ii < 12)
            {
                p.neighbors[ii] = NucleusSpacing.points[i].neighbors[ii];
                ii++;
            }
            p.pos = this.flipPositions ? -NucleusSpacing.points[i].pos : NucleusSpacing.points[i].pos;
            p.distRank = NucleusSpacing.points[i].distRank;
            ps[i] = p;
            i++;
        }
        int maxRank = ps[ps.Length - 1].distRank;
        NucleusRank[] rs = new NucleusRank[maxRank + 1];
        int[] rankCounts = new int[rs.Length];
        i = 0;
        while (i < ps.Length)
        {
            rankCounts[ps[i].distRank]++;
            i++;
        }
        int curRank = 0;
        i = 0;
        while (i < rs.Length)
        {
            NucleusRank r = new NucleusRank();
            r.points = new NucleusPoint[rankCounts[i]];
            rs[i] = r;
            i++;
        }
        rankCounts = new int[rs.Length];
        i = 0;
        while (i < rs.Length)
        {
            rankCounts[i] = 0;
            i++;
        }
        i = 0;
        while (i < ps.Length)
        {
            rs[ps[i].distRank].points[rankCounts[ps[i].distRank]] = ps[i];
            rankCounts[ps[i].distRank]++;
            i++;
        }
        this.points = ps;
        this.ranks = rs;
    }

    public Nucleus()
    {
        this.p = 2;
        this.n = 2;
        this.particleRadius = 1f;
        this.colliderRadius = 1.3f;
        this.overallScale = 0.2f;
        this.flipPositions = true;
        this.changeInterval = 0.1f;
        this.movementSpeed = 1f;
        this.movementAmount = 0.1f;
        this.zSpacingInMesh = 1.631544f;
        this.mouseOver = -1;
        this.mouseDown = -1;
    }

}