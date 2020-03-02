using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class UserAtom : MonoBehaviour
{
    public static int p;
    public static int n;
    public static int e;
    static UserAtom()
    {
        UserAtom.p = 3;
        UserAtom.n = 3;
        UserAtom.e = 3;
    }

}