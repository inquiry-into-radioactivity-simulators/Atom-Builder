using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NucleusDisplay : MonoBehaviour {

	public class Nucleon {
		public Vector3 pos;
		public Color color;
	}


	public Material[] materials;
	public float size;

	private Mesh mesh;
	private Nucleon[] myNucleons;

	void Start () {
		var o = new GameObject("NucleusDisplay ");
		var mf = o.AddComponent<MeshFilter>();
		var r = o.AddComponent<MeshRenderer>();
		r.materials = materials;
		mesh = new Mesh();
		mf.mesh = mesh;
		o.transform.parent = transform;
		myNucleons = new Nucleon[0];
	}

	public Nucleon[] GetNucleons() {
		return myNucleons;
	}

	public void SetNucleons(Nucleon[] nucleons) {
		myNucleons = nucleons;
		RebuildMesh(myNucleons);
	}

	private void RebuildMesh (Nucleon[] nucleons) {
		var vertexColors = new Color[nucleons.Length*4];
		var vertices = new Vector3[nucleons.Length*4];
		var uvs = new Vector2[nucleons.Length*4];
		var triangles = new int[nucleons.Length*6];

		//  0----1
		//  |  / |
		//  | /  |
		//  2----3

		for(var i = 0; i < nucleons.Length; i++) {
			var vi = i * 4;
			var ti = i * 6;

			vertices[vi  ] = nucleons[i].pos + transform.InverseTransformDirection(new Vector3(-1, 1,0)) * size;
			vertices[vi+1] = nucleons[i].pos + transform.InverseTransformDirection(new Vector3( 1, 1,0)) * size;
			vertices[vi+2] = nucleons[i].pos + transform.InverseTransformDirection(new Vector3(-1,-1,0)) * size;
			vertices[vi+3] = nucleons[i].pos + transform.InverseTransformDirection(new Vector3( 1,-1,0)) * size;

			uvs[vi  ] = new Vector2(0, 1);
			uvs[vi+1] = new Vector2(1, 1);
			uvs[vi+2] = new Vector2(0, 0);
			uvs[vi+3] = new Vector2(1, 0);

			vertexColors[vi  ] = nucleons[i].color;
			vertexColors[vi+1] = nucleons[i].color;
			vertexColors[vi+2] = nucleons[i].color;
			vertexColors[vi+3] = nucleons[i].color;

			triangles[ti  ] = vi  ;
			triangles[ti+1] = vi+1;
			triangles[ti+2] = vi+2;

			triangles[ti+3] = vi+1;
			triangles[ti+4] = vi+3;
			triangles[ti+5] = vi+2;
			
		}

		mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.colors = vertexColors;
		mesh.triangles = triangles;


	}
}
