using UnityEngine;
using System.Collections;

// source: http://www.nndc.bnl.gov/chart/
 // decaymode
 // half life
 // gamma y/n
[System.Serializable]
public class Element : object
{
    public Isotope[] nuclides;
    public int p;
    public string name;
    public string symbol;
    public bool grabElectron;
    public float size;
    public float electronAffinity;
    public Element()
    {
        this.name = "";
        this.symbol = "";
    }

}
[System.Serializable]
public class Isotope : object
{
    public int p;
    public int n;
    public float halfLife;
    public DecayMode decayMode;
    public bool gamma;
    public bool onlyGamma;
    public Isotope()
    {
        this.decayMode = DecayMode.Stable;
        this.gamma = true;
        this.onlyGamma = true;
    }

}
public enum DecayMode
{
    Stable = 0,
    ECPE = 1,
    Beta = 2,
    Alpha = 3,
    Proton = 4,
    Neutron = 5,
    Fission = 6,
    Unknown = 7
}

[System.Serializable]
public partial class AtomChart : MonoBehaviour
{
    public Texture2D im1;
    public Texture2D im2;
    public Texture2D im3;
    public TextAsset elementTable;
    public static Element[] elements;
    public static AtomChart ins;
    public int totalDecayModes;
    public static float[] halfLives;
    public virtual void Awake()
    {
        if ((AtomChart.elements != null) && (AtomChart.elements.Length > 0))
        {
            return;
        }
        AtomChart.halfLives[0] = 10 * Mathf.Pow(10, 15);
        AtomChart.halfLives[1] = 10 * Mathf.Pow(10, 10);
        AtomChart.halfLives[2] = 10 * Mathf.Pow(10, 7);
        AtomChart.halfLives[3] = 10 * Mathf.Pow(10, 5);
        AtomChart.halfLives[4] = 10 * Mathf.Pow(10, 4);
        AtomChart.halfLives[5] = 10 * Mathf.Pow(10, 3);
        AtomChart.halfLives[6] = 10 * Mathf.Pow(10, 2);
        AtomChart.halfLives[7] = 10 * Mathf.Pow(10, 1);
        AtomChart.halfLives[8] = 10 * Mathf.Pow(10, 0);
        AtomChart.halfLives[9] = 10 * Mathf.Pow(10, -1);
        AtomChart.halfLives[10] = 10 * Mathf.Pow(10, -2);
        AtomChart.halfLives[11] = 10 * Mathf.Pow(10, -3);
        AtomChart.halfLives[12] = 10 * Mathf.Pow(10, -4);
        AtomChart.halfLives[13] = 10 * Mathf.Pow(10, -5);
        AtomChart.halfLives[14] = 10 * Mathf.Pow(10, -6);
        AtomChart.halfLives[15] = 10 * Mathf.Pow(10, -7);
        AtomChart.halfLives[16] = 10 * Mathf.Pow(10, -15);
        AtomChart.halfLives[17] = -1;
        float x = 0f;
        float y = 0f;
        float i = 0f;
        int sy = 0;
        int sx = 0;
        int si = 0;
        AtomChart.elements = new Element[this.im1.height];
        y = 0;
        while (y < this.im1.height)
        {
            Element elem = new Element();
            if (sy < this.elementTable.text.Length)
            {
                int newsy = this.elementTable.text.IndexOf(" ", sy);
                if (newsy == -1)
                {
                    newsy = this.elementTable.text.Length;
                }
                string line = this.elementTable.text.Substring(sy, newsy - sy);
                sy = newsy + 2;
                string[] values = new string[5];
                sx = 0;
                si = 0;
                while (si < 5)
                {
                    int newsx = line.IndexOf(",", sx);
                    if (newsx == -1)
                    {
                        newsx = line.Length;
                    }
                    values[si] = line.Substring(sx, newsx - sx);
                    sx = newsx + 1;
                    si++;
                }
                elem.symbol = values[0];
                elem.name = values[1];
                elem.grabElectron = values[2] == "1";
                if (values[3] != string.Empty)
                {
                    elem.size = (float) System.Convert.ToDouble(values[3]);
                }
                if (values[4] != string.Empty)
                {
                    elem.electronAffinity = (float) System.Convert.ToDouble(values[4]);
                }
            }
            elem.p = (int) (y + 1);
            elem.nuclides = new Isotope[this.im1.width];
            x = 0;
            while (x < this.im1.width)
            {
                float p1 = this.im1.GetPixel((int) x, (int) y).a;
                float p2 = this.im2.GetPixel((int) x, (int) y).a;
                float p3 = this.im3.GetPixel((int) x, (int) y).a;
                if (p1 != 1)
                {
                    Isotope nuc = new Isotope();
                    nuc.n = (int) x;
                    nuc.p = (int) (y + 1);
                    i = 0f;
                    while (i < this.totalDecayModes)
                    {
                        if (p1 < (0.07f + (i * 0.1f)))
                        {
                            i = this.totalDecayModes;
                        }
                        else
                        {
                            nuc.decayMode = (DecayMode) UnityScript.Lang.UnityBuiltins.parseInt(i + 1);
                        }
                        i++;
                    }
                    i = 0f;
                    while (i < AtomChart.halfLives.Length)
                    {
                        if (p2 < (i * 0.055556f))
                        {
                            i = AtomChart.halfLives.Length;
                        }
                        else
                        {
                            nuc.halfLife = AtomChart.halfLives[Mathf.RoundToInt(i)];
                        }
                        i++;
                    }
                    nuc.gamma = ((p3 > 0.1f) && !(((y + 1) == 84) && (x == 126))) && !(((y + 1) == 95) && (x == 146));
                    nuc.onlyGamma = ((y + 1) == 27) && (x == 33);
                    //if(!nuc.gamma) Debug.Log(elem.name + (x+y+1));
                    elem.nuclides[Mathf.RoundToInt(x)] = nuc;
                }
                AtomChart.elements[Mathf.RoundToInt(y)] = elem;
                x++;
            }
            y++;
        }
        AtomChart.ins = this;
    }

    public static int ClosestExistingNuclide(int p, int n)
    {
        int distance = 1000;
        int cur = Mathf.Clamp(n, 0, AtomChart.elements[p - 1].nuclides.Length - 1);
        int i = 0;
        while (i < AtomChart.elements[p - 1].nuclides.Length)
        {
            int d = Mathf.Abs(i - n);
            if ((distance > d) && (AtomChart.elements[p - 1].nuclides[i] != null))
            {
                distance = d;
                cur = i;
            }
            i++;
        }
        return cur;
    }

    public static Isotope ClosestStableNuclide(int p, int n)
    {
        if (p == 61)
        {
            p = p + 1;
        }
        int distance = 1000;
        Isotope cur = AtomChart.elements[p - 1].nuclides[Mathf.Clamp(n, 0, AtomChart.elements[p - 1].nuclides.Length - 1)];
        int i = 0;
        while (i < AtomChart.elements[p - 1].nuclides.Length)
        {
            int d = Mathf.Abs(i - n);
            if (((distance > d) && (AtomChart.elements[p - 1].nuclides[i] != null)) && (AtomChart.elements[p - 1].nuclides[i].halfLife > (10 * Mathf.Pow(10, 7))))
            {
                distance = d;
                cur = AtomChart.elements[p - 1].nuclides[i];
            }
            i++;
        }
        return cur;
    }

    public virtual void OnDrawGizmosSelected()
    {
        if (AtomChart.elements != null)
        {
            int y = 0;
            while (y < AtomChart.elements.Length)
            {
                int x = 0;
                while (x < AtomChart.elements[y].nuclides.Length)
                {
                    Isotope nuc = AtomChart.elements[y].nuclides[x];
                    if (nuc != null)
                    {
                        int z = 1;
                        Random.seed = UnityScript.Lang.UnityBuiltins.parseInt((int) nuc.decayMode) * 36214;
                        Gizmos.color = HSBColor.ToColor(new HSBColor(Random.value, Random.value, Random.value, 1));
                        Gizmos.DrawLine(new Vector3(x, y, -(z + 15) * 0.05f) * 0.1f, new Vector3(x, y, (z + 15) * 0.05f) * 0.1f);
                    }
                    x++;
                }
                y++;
            }
        }
    }

    public AtomChart()
    {
        this.totalDecayModes = 8;
    }

    static AtomChart()
    {
        AtomChart.halfLives = new float[18];
    }

}