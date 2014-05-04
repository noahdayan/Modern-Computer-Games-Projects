using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour {

	public GameObject cube;

	// Use this for initialization
	void Start () {
	
		// Generates two boxes
		for(int i = 0; i < 2; i++)
		{
			float offsetX = Random.Range(-1.5f, 1.5f); // X offset from center of room
			float offsetZ = Random.Range(-1.5f, 1.5f); // Z offset from center of room

			// Determine random position of box
			Vector3 pos = transform.position + new Vector3(offsetX, 0.5f, offsetZ);

			// Instantiate box
			GameObject box = (GameObject)Instantiate(cube, pos, Quaternion.identity);

			// Determine random rotation of box
			float rot = Random.Range(0.0f, 360.0f);

			// Rotate box
			box.transform.Rotate(Vector3.up, rot);
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
