using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public static ArrayList guards;
	public GameObject guard;
	public int numberOfGuards;
	private ArrayList indices;

	// Use this for initialization
	void Start () {
	
		// Get all waypoints and store transforms
		GameObject[] temp = GameObject.FindGameObjectsWithTag("Waypoint");
		Transform[] children = new Transform[temp.Length];

		// Sort the waypoints in ascending order
		for(int i = 0; i < temp.Length; i++)
		{
			int number = int.Parse(temp[i].name.Substring(8));
			
			children[number] = temp[i].transform;
		}

		indices = new ArrayList(); // Array of guard positions
		guards = new ArrayList(); // Array of guards

		// Place guards at waypoints without overlap
		for(int i = 0; i < numberOfGuards; i++)
		{
			int index = Random.Range(1, 15); // Choose waypoint on outer circle
			if(!indices.Contains(index) && index != 3 && index != 6 && index != 10 && index != 13)
			{
				// Instantiate guard and add index if no overlap
				indices.Add(index);
				GameObject g = (GameObject)Instantiate(guard, children[index].position, Quaternion.identity);
				g.GetComponent<Patroller>().position = index;
				guards.Add(g);
			}
			else
			{
				i--; // Decrement if ovelap
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
