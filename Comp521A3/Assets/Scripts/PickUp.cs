using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Destroys treasure when picked-up
	void OnTriggerEnter (Collider c) {

		if(c.name == "Adventurer")
		{
			Destroy(gameObject);
		}

	}
}
