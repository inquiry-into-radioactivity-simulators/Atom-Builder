using UnityEngine;
using System.Collections;
using System.Collections.Generic;

 // number of neighbors in 3d hexagonal closest packing times two triangles per edge
public enum PType
{
    Empty = 0,
    P = 1,
    N = 2,
    E = 3
}


[System.Serializable]
public partial class NucleusSpacing : MonoBehaviour
{
    class VertexMapping {
        public Vector3 pos;
        public ArrayList ids;
    }

    public int maxTotalNucleons;
    public int prefilterNeighborSpaces;
    public float smallFudge;
    public float vertexDictionaryUnit;
    public static NucleusPoint[] points;
    public virtual void Awake()
    {
        if (NucleusSpacing.points != null)
        {
            return;
        }

        // Copy all the points from the mesh to a new list which we will work on
        MeshFilter meshFilter = (MeshFilter) this.GetComponent(typeof(MeshFilter));
        Vector3[] verts = meshFilter.mesh.vertices;
        Dictionary<string, VertexMapping> vertHash = new Dictionary<string, VertexMapping>();
        Dictionary<int, int> vertexIdMapping = new Dictionary<int, int>();

        for(int vi=0; vi < verts.Length; vi++) {

            string key = Mathf.RoundToInt(verts[vi].x/vertexDictionaryUnit)
            +","+Mathf.RoundToInt(verts[vi].y/vertexDictionaryUnit)
            +","+Mathf.RoundToInt(verts[vi].z/vertexDictionaryUnit);
            
            if(!vertHash.ContainsKey(key)) {
                vertHash[key] = new VertexMapping();
                vertHash[key].pos = verts[vi];
                vertHash[key].ids = new ArrayList();
            }
            vertHash[key].ids.Add(vi);
        }

        int i = 0;
        int ii = 0;

        NucleusPoint[] allPoints = new NucleusPoint[vertHash.Count];
        i = 0;
        foreach(KeyValuePair<string, VertexMapping> entry in vertHash) {
            NucleusPoint np = new NucleusPoint();
            np.neighbors = new int[this.prefilterNeighborSpaces];
            np.pos =  entry.Value.pos;
            np.dist = entry.Value.pos.sqrMagnitude;
            for(int j=0; j < entry.Value.ids.Count; j++) {
                vertexIdMapping[(int)entry.Value.ids[j]] = i;
            }
            allPoints[i] = np;
            i++;
        }

        // Copy all neighbor relationships from the triangles to the new list
        int[] tris = meshFilter.mesh.triangles;
        i = 0;
        while (i < tris.Length)
        {
            int t0 = vertexIdMapping[tris[i]];
            int t1 = vertexIdMapping[tris[i+1]];
            int t2 = vertexIdMapping[tris[i+2]];
            NucleusPoint p0 = allPoints[t0];
            NucleusPoint p1 = allPoints[t1];
            NucleusPoint p2 = allPoints[t2];
            p0.neighbors[p0.tempIndex] = t1;
            p0.neighbors[p0.tempIndex + 1] = t2;
            p1.neighbors[p1.tempIndex] = t0;
            p1.neighbors[p1.tempIndex + 1] = t2;
            p2.neighbors[p2.tempIndex] = t1;
            p2.neighbors[p2.tempIndex + 1] = t0;
            p0.tempIndex = p0.tempIndex + 2;
            p1.tempIndex = p1.tempIndex + 2;
            p2.tempIndex = p2.tempIndex + 2;
            i = i + 3;
        }
        // reset tempIndex to -1 so we can later identify neighbor references to unused points
        i = 0;
        while (i < allPoints.Length)
        {
            allPoints[i].tempIndex = -1;
            i++;
        }
        // copy the needed points to the final list, and store the new index in tempIndex
        int added = 0;
        float lastDistance = -1f;
        int distRank = 0;
        NucleusSpacing.points = new NucleusPoint[this.maxTotalNucleons];
        while (added < this.maxTotalNucleons)
        {
            float nextDistance = 1000f;
            i = 0;
            while (i < allPoints.Length)
            {
                if ((allPoints[i].dist < nextDistance) && (allPoints[i].dist > (lastDistance + this.smallFudge)))
                {
                    nextDistance = allPoints[i].dist;
                }
                i++;
            }
            i = 0;
            while (i < allPoints.Length)
            {
                float delta = allPoints[i].dist - nextDistance;
                if (delta < 0)
                {
                    delta = -delta;
                }
                if ((delta < this.smallFudge) && (added < this.maxTotalNucleons))
                {
                    allPoints[i].dist = -10;
                    allPoints[i].distRank = distRank;
                    allPoints[i].tempIndex = added;
                    NucleusSpacing.points[added] = allPoints[i];
                    added++;
                }
                i++;
            }
            lastDistance = nextDistance;
            distRank++;
        }
        // Remove redundant neighbor relationships and update neighbor relationships to new index. points on the edge will have some neighbor spaces == -1
        i = 0;
        while (i < NucleusSpacing.points.Length)
        {
            int nni = 0;
            int nnAdded = 0;
            int[] nn = new int[12]; // 12 neighbors in 3d hexagonal closest packing
            ii = 0;
            while (ii < NucleusSpacing.points[i].neighbors.Length)
            {
                if (NucleusSpacing.points[i].neighbors[ii] != 0)
                {
                    bool found = false;
                    nni = 0;
                    while (nni < 12)
                    {
                        if (NucleusSpacing.points[i].neighbors[ii] == nn[nni])
                        {
                            found = true;
                        }
                        nni++;
                    }
                    if (!found)
                    {
                        nn[nnAdded] = NucleusSpacing.points[i].neighbors[ii];
                        nnAdded++;
                    }
                }
                ii++;
            }
            nni = 0;
            while (nni < 12)
            {
                nn[nni] = allPoints[nn[nni]].tempIndex;
                nni++;
            }
            NucleusSpacing.points[i].neighbors = nn;
            i++;
        }
    }

    public virtual void OnDrawGizmosSelected()
    {
        if (NucleusSpacing.points != null)
        {
            int i = 0;
            while (i < NucleusSpacing.points.Length)
            {
                float h = NucleusSpacing.points[i].distRank * 0.1f;
                while (h > 1)
                {
                    h = h - 1;
                }
                Color color = HSBColor.ToColor(new HSBColor(h, 1, 0.2f, 1));
                Gizmos.color = color;
                Gizmos.DrawSphere(NucleusSpacing.points[i].pos, 0.1f);
                int nni = 0;
                while (nni < 12)
                {
                    if (NucleusSpacing.points[i].neighbors[nni] != -1)
                    {
                        Gizmos.color = new Color(color.r * 3, color.g * 3, color.b * 3, color.a * 0.8f);
                        Gizmos.DrawLine((NucleusSpacing.points[NucleusSpacing.points[i].neighbors[nni]].pos + NucleusSpacing.points[i].pos) * 0.5f, NucleusSpacing.points[i].pos);
                    }
                    nni++;
                }
                i++;
            }
        }
    }

    public NucleusSpacing()
    {
        this.maxTotalNucleons = 300;
        this.prefilterNeighborSpaces = 24;
        this.smallFudge = 0.12f;
        this.vertexDictionaryUnit = 0.001f;
    }

}