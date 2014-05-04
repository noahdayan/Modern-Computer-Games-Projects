using UnityEngine;
using System.Collections;
using Hexagon;

public class TileSelector : MonoBehaviour {

	public GameObject character; // Character reference
	public GameObject map; // Map reference
	public IntVector2 index; // Index of current tile

	// Use this for initialization
	void Start () {
	
		character = GameObject.Find("Character");
		map = GameObject.Find("Map");

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Left click behavior to specify target tile
	void OnMouseDown () {

		if(tag == "Tile")
		{
			character.SendMessage("moveToTile", index);
		}

	}

	// Right click behavior to create obstacles
	void OnMouseOver () {

		if(Input.GetMouseButtonDown(1))
		{
			map.SendMessage("paintObstacle", index);
			character.SendMessage("resetState");
		}

	}
}
