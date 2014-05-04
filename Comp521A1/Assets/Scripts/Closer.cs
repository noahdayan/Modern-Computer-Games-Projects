using UnityEngine;
using System.Collections;

public class Closer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerExit (Collider other) {

		//close door when player enters trigger.
		gameObject.transform.position += new Vector3(0,4.0f,0);
	}
}
