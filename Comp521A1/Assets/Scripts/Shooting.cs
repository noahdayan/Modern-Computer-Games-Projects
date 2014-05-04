using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {

	public GameObject projectile;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		// shoot projectile when clicked on left mouse.
		if(Input.GetButtonDown("Fire1"))
		{
			GameObject p = (GameObject)Instantiate(projectile, Camera.main.transform.position, Quaternion.identity);
			p.rigidbody.AddForce(transform.forward * 1000);
		}
	}
}
