using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Ngnome : MonoBehaviour
{
    public Nucleus nucleus;
    public GnomeDialog dialog;
    public Transform sfr;
    public DragNDrop dnd;
    public Texture2D[] frames;
    public Material[] nucMats;
    public Renderer nuc0;
    public Renderer nuc1;
    private bool going;
    public static bool nMGMT;
    private int gnomeClicks;
    private float guiAnim;
    private GUIStyle labelStyle1;
    private GUIStyle labelStyle2;
    public virtual void Start()
    {
        this.labelStyle1 = new GUIStyle();
        this.labelStyle1.normal.background = this.frames[0];
        this.labelStyle2 = new GUIStyle();
        this.labelStyle2.normal.background = this.frames[4];
        string[] theArguments = System.Environment.GetCommandLineArgs();
        int i = 0;
        while (i < theArguments.Length)
        {
            if (theArguments[i] == "--gnome")
            {
                Ngnome.nMGMT = false;
            }
            i++;
        }
    }

    public virtual void Update()
    {
        float target = !this.going && !GnomeDialog.instance.enabled ? 1f : 0f;
        if (this.guiAnim > target)
        {
            this.guiAnim = this.guiAnim - (Time.deltaTime * 2f);
        }
        if (this.guiAnim < target)
        {
            this.guiAnim = this.guiAnim + (Time.deltaTime * 2f);
        }
        this.guiAnim = Mathf.Clamp01(this.guiAnim);
    }

    public virtual void OnGUI()
    {
        float s = (Screen.height + Screen.width) * 0.08f;
        float a = this.guiAnim * this.guiAnim;
        a = 1f - a;
        a = a * a;
        a = (1f - a) * s;
        if (GUI.Button(new Rect(Screen.width - a, Screen.height - s, s, s), "", Ngnome.nMGMT ? this.labelStyle1 : this.labelStyle2))
        {
            this.gnomeClicks++;
            if (this.gnomeClicks > 10)
            {
                Ngnome.nMGMT = false;
            }
        }
    }

    public virtual void Go()
    {
        if (Ngnome.nMGMT)
        {
            Isotope iso = AtomChart.ClosestStableNuclide(UserAtom.p, UserAtom.n);
            int totalChanges = Mathf.Abs(iso.p - UserAtom.p) + Mathf.Abs(iso.n - UserAtom.n);
            if (totalChanges != 0)
            {
                this.dialog.enabled = true;
                this.dialog.mode = 0;
            }
            else
            {
                //dialog.message = "it is not stable";
                Application.LoadLevel(1);
            }
        }
        else
        {
            Application.LoadLevel(1);
        }
    }

    public virtual IEnumerator Go2()
    {
        if (this.going)
        {
            yield break;
        }
        this.going = true;
        this.dnd.enabled = false;
        Isotope iso = AtomChart.ClosestStableNuclide(UserAtom.p, UserAtom.n);
        int totalChanges = Mathf.Abs(iso.p - UserAtom.p) + Mathf.Abs(iso.n - UserAtom.n);
        if (totalChanges == 0)
        {
            Application.LoadLevel(1);
            yield break;
        }
        while ((iso.p != UserAtom.p) || (iso.n != UserAtom.n))
        {
            bool add = false;
            bool pr = false;
            if (iso.n < UserAtom.n)
            {
                add = false;
                pr = false;
            }
            if (iso.n > UserAtom.n)
            {
                add = true;
                pr = false;
            }
            if (iso.p < UserAtom.p)
            {
                add = false;
                pr = true;
            }
            if (iso.p > UserAtom.p)
            {
                add = true;
                pr = true;
            }
            float overallSpeed = Mathf.Max((0.5f + Mathf.Min(2, totalChanges / 6)) * (pr ? 0.3f : 1f), 0.25f);
            this.SetFrame(add ? 3 : 0, pr);
            float moveTime = 0.5f / overallSpeed;
            float leftPos = -3.25f;
            float rightPos = this.sfr.TransformPoint(new Vector3(-0.5f, 0, 0)).x + 1f;
            float lastTime = Time.time;
            while (Time.time < (lastTime + moveTime))
            {

                {
                    float _77 = this.SineLerp(leftPos, rightPos, (Time.time - lastTime) / moveTime);
                    Vector3 _78 = this.transform.position;
                    _78.x = _77;
                    this.transform.position = _78;
                }
                yield return null;
            }
            this.SetFrame(add ? 2 : 1, pr);
            yield return new WaitForSeconds(0.1f / overallSpeed);
            this.SetFrame(add ? 1 : 2, pr);
            if (add)
            {
                if (pr)
                {
                    this.nucleus.p++;
                }
                else
                {
                    this.nucleus.n++;
                }
            }
            else
            {
                if (pr)
                {
                    this.nucleus.p--;
                }
                else
                {
                    this.nucleus.n--;
                }
            }
            this.nucleus.Rebuild();
            lastTime = Time.time;
            while (Time.time < (lastTime + moveTime))
            {
                float amt = (Time.time - lastTime) / moveTime;
                if (amt > 0.2f)
                {
                    this.SetFrame(add ? 0 : 3, pr);
                }

                {
                    float _79 = this.SineLerp(rightPos, leftPos, amt);
                    Vector3 _80 = this.transform.position;
                    _80.x = _79;
                    this.transform.position = _80;
                }
                yield return null;
            }
        }
        this.dialog.enabled = true;
        this.dialog.mode = 1;
        this.going = false;
    }

    public virtual float SineLerp(float from, float to, float amt)
    {
        amt = (amt > 1 ? 1 : (amt < 0 ? 0 : amt)) - 0.5f;
        amt = amt * Mathf.PI;
        amt = (Mathf.Sin(amt) + 1f) * 0.5f;
        return (from * (1f - amt)) + (to * amt);
    }

    public virtual void SetFrame(int f, bool pr)
    {
        this.GetComponent<Renderer>().material.mainTexture = this.frames[f];
        this.nuc0.enabled = f == 2;
        this.nuc1.enabled = f == 3;
        this.nuc0.material = this.nuc1.material = this.nucMats[pr ? 0 : 1];
    }

    public static int MaxProtons()
    {
        return Ngnome.nMGMT ? 83 : 105;
    }

    public Ngnome()
    {
        this.frames = new Texture2D[3];
        this.nucMats = new Material[2];
        this.guiAnim = 1f;
    }

    static Ngnome()
    {
        Ngnome.nMGMT = true;
    }

}