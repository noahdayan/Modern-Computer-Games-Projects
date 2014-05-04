using UnityEngine;
using System.Collections;

public class Builder : MonoBehaviour {
	
	public Vector3[] newVertices; // list of vertices in mesh
	public Vector2[] newUV; // list of texture coordinates in mesh
	public int[] newTriangles; // list of triangles in mesh
	public Vector3[] newNormals; // list of normals in mesh

	// Use this for initialization
	void Start () {
	
		// set new mesh in mesh filter
		GetComponent<MeshFilter>().mesh = CreateMesh();

		// get the tank gameobject and place it 2/3 of the way up on the mountain
		GameObject tank = GameObject.Find("Tank");
		tank.transform.position = newVertices[1300];
		tank.transform.Translate(-6.0f,0,0);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// creates the mountain mesh
	Mesh CreateMesh () {

		int k = 10; // number of recursion
		int n = 4; // number of vertices

		// sum up the powers of two
		for(int i = 1; i <= k; i++)
		{
			n += (int)Mathf.Pow(2.0f,(float)i);
		}

		int t = 3 * (n - 2); // number of triangle vertices

		Mesh mesh = new Mesh(); // instantiate new mesh

		// instantiate all mesh arrays
		newVertices = new Vector3[n];
		newUV = new Vector2[newVertices.Length];
		newTriangles = new int[t];
		newNormals = new Vector3[newVertices.Length];

		float width = Random.Range(4.0f,6.0f); // set width of mountain randomly

		// set the four principle vertices
		newVertices[0] = new Vector3(0,-5,0); // bottom center
		newVertices[1] = new Vector3(-width,-5,0); // bottom left
		newVertices[n/2] = new Vector3(0,Random.Range(2.0f,4.0f),0); // random top center
		newVertices[n - 1] = new Vector3(width,-5,0); // bottom right

		// set two triangles based on bottom center vertex clockwise
		// left triangle
		newTriangles[0] = 0;
		newTriangles[1] = 1;
		newTriangles[2] = n/2;

		//right triangle
		newTriangles[t/2] = 0;
		newTriangles[t/2 + 1] = n/2;
		newTriangles[t/2 + 2] = n - 1;

		int newN = n - (int)Mathf.Pow(2.0f,(float)k); // recalculate number of vertices for recursion

		RecursiveMesh(newN, k - 1, t/2, 1, 0); // recursively call on left subtriangle
		RecursiveMesh(newN, k - 1, t/2, n/2, t/2); // recursively call on right subtriangle

		// set same texture coordinates for all vertices
		for(int i = 0; i < newUV.Length; i++)
		{
			newUV[i] = new Vector2(0,0);
		}

		// set same normal for all vertices pointing out of screen
		for(int i = 0; i < newNormals.Length; i++)
		{
			newNormals[i] = - Vector3.forward;
		}

		// assign arrays to mesh
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
		mesh.normals = newNormals;

		// assign mesh name
		mesh.name = "Mountain";

		// return mountain mesh
		return mesh;

	}

	// recursive call to create submeshes
	void RecursiveMesh (int n, int k, int t, int nIndex, int tIndex) {

		int newIndex = nIndex + (int)Mathf.Pow(2.0f,(float)(k+1)); // calculate index of right vertex

		// calculate midpoint and normal
		Vector3 midpoint = Vector3.Lerp(newVertices[nIndex], newVertices[newIndex], 0.5f);
		Vector3 normal = new Vector3(newVertices[nIndex].y - newVertices[newIndex].y, newVertices[newIndex].x - newVertices[nIndex].x, 0);

		// randomly negate the normal
		if(Random.value > 0.5f)
		{
			normal = -normal;
		}

		// assign proportional distance in normal direction to bend
		midpoint += normal/(20 * Vector3.Distance(newVertices[nIndex], newVertices[newIndex]));

		// assign midpoint to appropriate index
		newVertices[nIndex + (int)Mathf.Pow(2.0f,(float)k)] = midpoint;

		// reassign triangle vertices correctly
		newTriangles[tIndex] = 0;
		newTriangles[tIndex + 1] = nIndex;
		newTriangles[tIndex + 2] = nIndex + 1;
		newTriangles[tIndex + 3] = 0;
		newTriangles[tIndex + 4] = nIndex + 1;
		newTriangles[tIndex + 5] = nIndex + 2;

		// recursively call if recursion depthnot attained
		if(k != 0)
		{
			int newN = n - (int)Mathf.Pow(2.0f,(float)k); // recalculate number of vertices for recursion

			RecursiveMesh(newN, k - 1, t/2, nIndex, tIndex); // recursively call on left subtriangle
			RecursiveMesh(newN, k - 1, t/2, nIndex - 1 + n/2, tIndex + t/2); // recursively call on right subtriangle
		}
	}
}
