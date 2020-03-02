using UnityEngine;
using System.Collections;

[System.Serializable]
public class NShell : object
{
    public int possible;
    public int count;
    public float radius;
    public float rotation;
    public Renderer renderer;
    public GameObject[] electrons;
}
[System.Serializable]
public class Orbitals : DraggableType
{
    public Material material;
    public Color textColor;
    public GameObject electronPrefab;
    public int totalElectrons;
    public float smallestScale;
    public float largestScale;
    public float width;
    public float segments;
    public float rotationSpeed;
    public CreatorBackground background;
    public int outerShell;
    public NShell[] shells;
    private Color normalColor;
    private GUIStyle style;
    private int mouseOver;
    public virtual void Start()
    {
        int i = 0;
        int ii = 0;
        this.shells = new NShell[7];
        i = 0;
        while (i < this.shells.Length)
        {
            this.shells[i] = new NShell();
            i++;
        }
        this.shells[0].possible = 2;
        this.shells[1].possible = 8;
        this.shells[2].possible = 8;
        this.shells[3].possible = 18;
        this.shells[4].possible = 18;
        this.shells[5].possible = 32;
        this.shells[6].possible = 32;
        UnityEngine.Texture2D electronTexture = (Texture2D)Resources.Load("Electron_TIF");
        this.normalColor = this.material.GetColor("_TintColor");
        this.style = new GUIStyle();
        this.style.normal.background = (Texture2D)Resources.Load("Shadow_TIF");
        this.style.normal.textColor = this.textColor;
        this.style.font = (Font)Resources.Load("MainFont");
        this.style.alignment = TextAnchor.UpperCenter;
        this.style.border.top = this.style.border.bottom = this.style.border.left = this.style.border.right = 11;
        this.style.padding.top = 1;
        int eAdded = 0;
        i = 0;
        while (i < this.shells.Length)
        {
            float frac = i;
            frac = frac / (this.shells.Length - 1);
            GameObject chObj = new GameObject("Orbital " + i);
            Transform trans = chObj.transform;
            trans.position = this.transform.position;
            trans.parent = this.transform;
            float radius = Mathf.Lerp(this.smallestScale, this.largestScale, frac);
            float verts = this.segments + 1;
            LineRenderer line = (LineRenderer) chObj.AddComponent(typeof(LineRenderer));
            line.useWorldSpace = false;
            line.SetWidth(this.width, this.width);
            line.SetColors(new Color(0.5f, 0.5f, 0.5f, 1), new Color(0.5f, 0.5f, 0.5f, 1));
            line.SetVertexCount((int) verts);
            ii = 0;
            while (ii < verts)
            {
                line.SetPosition(ii, (Quaternion.AngleAxis((UnityScript.Lang.UnityBuiltins.parseFloat(ii) / this.segments) * 360, Vector3.forward) * -Vector3.right) * radius);
                ii++;
            }
            line.material = this.material;
            line.material.SetColor("_TintColor", Color.clear);
            this.shells[i].radius = radius;
            this.shells[i].renderer = line;
            this.shells[i].electrons = new GameObject[this.shells[i].possible];
            ii = 0;
            while (ii < this.shells[i].possible)
            {
                GameObject obj = UnityEngine.Object.Instantiate(this.electronPrefab);
                obj.transform.parent = this.transform;
                DraggableCollider collider = (DraggableCollider) obj.GetComponent(typeof(DraggableCollider));
                collider.id = eAdded;
                collider.parent = this;
                this.shells[i].electrons[ii] = obj;
                eAdded++;
                ii++;
            }
            i++;
        }
        this.totalElectrons = UserAtom.e;
        int toAdd = UserAtom.e;
        UserAtom.e = 0; // because AddElectron increments this
        i = 0;
        while (i < toAdd)
        {
            this.AddElectron();
            i++;
        }
    }

    public virtual void AddElectron()
    {
        int i = 0;
        while (i < this.shells.Length)
        {
            if (this.shells[i].count < this.shells[i].possible)
            {
                this.shells[i].count++;
                if (this.shells[i].count == this.shells[i].possible)
                {
                    this.outerShell = i + 1;
                }
                else
                {
                    this.outerShell = i;
                }
                this.totalElectrons++;
                this.shells[i].renderer.material.SetColor("_TintColor", this.normalColor);
                i = this.shells.Length;
            }
            i++;
        }
        UserAtom.e++;
    }

    public virtual void RemoveElectron(bool click)
    {
        float i = 0f;
        float ii = 0f;
        i = this.shells.Length - 1;
        while (i >= 0)
        {
            if (this.shells[Mathf.RoundToInt(i)].count > 0)
            {
                this.shells[Mathf.RoundToInt(i)].count--;
                this.totalElectrons--;
                this.outerShell = (int) i;
                if (this.shells[Mathf.RoundToInt(i)].count == 0)
                {
                    this.shells[Mathf.RoundToInt(i)].rotation = 0;
                    this.shells[Mathf.RoundToInt(i)].renderer.material.SetColor("_TintColor", new Color(this.normalColor.r, this.normalColor.g, this.normalColor.b, 0));
                }
                i = -1;
            }
            i--;
        }
        if (click)
        {
            int takenFromShell = 0;
            int taken = 0;
            float takenPos = 0f;
            int outerShell = 0;
            i = 0;
            while (i < this.shells.Length)
            {
                ii = 0;
                while (ii < this.shells[Mathf.RoundToInt(i)].electrons.Length)
                {
                    if (((DraggableCollider) this.shells[Mathf.RoundToInt(i)].electrons[Mathf.RoundToInt(ii)].GetComponent(typeof(DraggableCollider))).id == this.mouseOver)
                    {
                        takenFromShell = (int) i;
                        taken = (int) ii;
                        takenPos = ii / this.shells[Mathf.RoundToInt(i)].count;
                    }
                    ii++;
                }
                if ((this.shells[Mathf.RoundToInt(i)].count > 0) && ((i == (this.shells.Length - 1)) || (this.shells[Mathf.RoundToInt(i) + 1].count == 0)))
                {
                    outerShell = (int) i;
                }
                i++;
            }
            CreatorParticle p = (CreatorParticle) this.shells[takenFromShell].electrons[taken].GetComponent(typeof(CreatorParticle));
            p.fromLerp = this.GetPosition(outerShell, takenPos);
            p.lerpEndTime = Time.time + 1;
        }
        UserAtom.e--;
    }

    /*
	for(i = takenFromShell; i < shells.length-1; i++) {
		var chosen = Mathf.Round(shellPos * shells[i].count);
		var nextChosen = Mathf.Round(shellPos * shells[i+1].count);
		var p : CreatorParticle = shells[i].electrons[chosen].GetComponent(CreatorParticle);
		p.fromLerp = shells[i+1].electrons[nextChosen].transform.position;
		p.lerpEndTime = Time.time + 1;
	}
	*/
    public override Draggable OnDragEvent(DragEventType type, int id, Draggable current, DraggableType target)
    {
        if (type == DragEventType.Down)
        {
            GameObject obj = new GameObject("Draggable");
            Draggable_Nucleon dr = (Draggable_Nucleon) obj.AddComponent(typeof(Draggable_Nucleon));
            dr.type = PType.E;
            this.RemoveElectron(true);
            return dr;
        }
        else
        {
            if (type == DragEventType.Up)
            {
                if (target && target.CanDrop(current))
                {
                    target.Drop(current);
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
            }
            else
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
            }
        }
        return current;
    }

    public override bool CanDrop(Draggable d)
    {
        return d as Draggable_Nucleon ? true : false;
    }

    public override void Drop(Draggable d)
    {
        int oldMouseOver = this.background.mouseOver;
        this.background.mouseOver = 0;
        this.background.Drop(d);
        this.background.mouseOver = oldMouseOver;
    }

    public virtual void Update()
    {
        float i = 0f;
        while (i < this.shells.Length)
        {
            if (this.shells[Mathf.RoundToInt(i)].count > 0)
            {
                this.shells[Mathf.RoundToInt(i)].rotation = this.shells[Mathf.RoundToInt(i)].rotation + (this.rotationSpeed * Time.deltaTime); // * (1.0/(i*0.3+1))
            }
            int ii = 0;
            while (ii < this.shells[Mathf.RoundToInt(i)].possible)
            {
                if ((ii + 1) <= this.shells[Mathf.RoundToInt(i)].count)
                {
                    Vector3 pos = this.GetPosition(i, (UnityScript.Lang.UnityBuiltins.parseFloat(ii) / this.shells[Mathf.RoundToInt(i)].count) - (i * 0.022f));
                    this.shells[Mathf.RoundToInt(i)].electrons[ii].GetComponent<Renderer>().enabled = true;
                    this.shells[Mathf.RoundToInt(i)].electrons[ii].transform.position = pos;
                }
                else
                {
                    this.shells[Mathf.RoundToInt(i)].electrons[ii].GetComponent<Renderer>().enabled = false;
                }
                ii++;
            }
            i++;
        }
    }

    public virtual Vector3 GetPosition(float i, float ii)
    {
        return ((Quaternion.AngleAxis((ii * 360) + this.shells[Mathf.RoundToInt(i)].rotation, Vector3.forward) * Vector3.right) * this.shells[Mathf.RoundToInt(i)].radius) + (Vector3.forward * -0.1f);
    }

    public virtual void OnGUI()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        int i = 0;
        while (i < this.shells.Length)
        {
            if (this.shells[Mathf.RoundToInt(i)].count > 0)
            {
                float screenWidthFloat = Screen.width;
                Vector3 sPos = Camera.main.WorldToScreenPoint(Vector3.right * this.shells[Mathf.RoundToInt(i)].radius) * (screenSize.x / screenWidthFloat);
                sPos.y = screenSize.y - sPos.y;
                GUIContent content = new GUIContent();
                string daString = "" + this.shells[Mathf.RoundToInt(i)].count;
                content.text = daString;
                Vector2 size = this.style.CalcSize(content);
                size = new Vector2(size.x + 15, 22);
                GUI.Label(new Rect(sPos.x - (size.x * 0.5f), sPos.y - (size.y * 0.5f), size.x, size.y), daString, this.style);
            }
            i++;
        }
    }

    public Orbitals()
    {
        this.smallestScale = 2f;
        this.largestScale = 5f;
        this.width = 0.5f;
        this.segments = 100f;
        this.rotationSpeed = 100f;
        this.mouseOver = -1;
    }

}