using UnityEngine;
using System.Collections;

public class Coathook : MonoBehaviour {

	public GUIText message;
	public GUIText inventory;
	public GameObject cloak;

	public static bool onCoathook;
	private bool inTrigger;
	private GameObject cloth;

	// Use this for initialization
	void Start () {
	
		onCoathook = false;
		inTrigger = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		// player press e and is in range of trigger
		if(Input.GetButtonDown("Use") && inTrigger)
		{
			// remove cloak
			if(onCoathook)
			{
				onCoathook = false;
				inventory.text = "Cloak";
				Destroy(cloth);
			}
			// put cloak
			else
			{
				onCoathook = true;
				inventory.text = "Empty";
				cloth = (GameObject)Instantiate(cloak);
			}
		}
	}

	void OnTriggerEnter (Collider other) {

		// player in trigger and corresponding message shown
		inTrigger = true;
		if(onCoathook)
		{
			message.text = "Press E to remove Cloak from Coathook.";
		}
		else
		{
			message.text = "Press E to put Cloak on Coathook.";
		}
	}

	void OnTriggerExit (Collider other) {

		// player not in trigger and hide message
		inTrigger = false;
		message.text = "";
	}
}
