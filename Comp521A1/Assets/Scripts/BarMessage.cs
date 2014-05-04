using UnityEngine;
using System.Collections;

public class BarMessage : MonoBehaviour {

	private int count;

	// Use this for initialization
	void Start () {
	
		count = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		// if cloak is on coathook and player entered bar at most 2 times then win.
		if(Coathook.onCoathook && count < 3)
		{
			((TextMesh)GetComponent(typeof(TextMesh))).text = "You Have Won!";
		}
		// first time player enters with cloak.
		else if(count == 1)
		{
			((TextMesh)GetComponent(typeof(TextMesh))).text = "You...";
		}
		// second time player enters with cloak.
		else if(count == 2)
		{
			((TextMesh)GetComponent(typeof(TextMesh))).text = "You Have...";
		}
		// if cloak is still on player and entered bar more than 2 times then lose.
		else
		{
			((TextMesh)GetComponent(typeof(TextMesh))).text = "You Have Lost!";
		}
	}

	void OnTriggerEnter (Collider other) {

		// increment only when cloak on player
		if(!Coathook.onCoathook)
		{
			count++;
		}
	}
}
