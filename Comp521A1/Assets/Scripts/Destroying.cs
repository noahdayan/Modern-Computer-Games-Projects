using UnityEngine;
using System.Collections;

public class Destroying : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		// destroy after one second to avoid unnecessary behavior.
		Destroy(gameObject, 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter (Collision collision) {

		// if projectile collides with a destroyable wall then destroy both.
		if(collision.gameObject.tag == "Wall")
		{
			Destroy(collision.gameObject);
			Destroy(gameObject);
		}
	}
}
