using UnityEngine;
using System.Collections;

public class Patroller : MonoBehaviour {

	private GameObject[] waypoints;
	public int currentWaypoint;
	public int previousWaypoint;
	public int position;
	private int elapsedTime;
	private int roomTime;
	private int inspectTime;
	public int recoveryTime;
	public int waitingTime;
	public float speed;
	private bool inRoom;
	private bool across;
	private GameObject adventurer;

	// Use this for initialization
	void Start () {
	
		// Get all waypoints and store transforms
		GameObject[] temp = GameObject.FindGameObjectsWithTag("Waypoint");
		waypoints = new GameObject[temp.Length];

		// Sort the waypoints in ascending order
		for(int i = 0; i < temp.Length; i++)
		{
			int number = int.Parse(temp[i].name.Substring(8));

			waypoints[number] = temp[i];
		}

		currentWaypoint = position; // Current target waypoint index
		previousWaypoint = position; // Previous target waypoint index
		elapsedTime = 0; // Counter for conversation time
		inspectTime = 0; // Counter for inspection time
		roomTime = Random.Range(0, 100); // Inspection duration
		inRoom = false; // Boolean for room checking
		across = false; // Boolean for shortcut checking

		adventurer = GameObject.Find("Adventurer");

	}
	
	// Update is called once per frame
	void Update () {
	
		// Set movement parameters from waypoint position
		Vector3 target = waypoints[currentWaypoint].transform.position;
		Vector3 moveDirection = target - transform.position;
		Vector3 velocity = rigidbody.velocity;

		// Move condition for Selector
		System.Func<bool> moveCond = () => moveDirection.magnitude < 0.1;

		// Out of room condition for Selector
		System.Func<bool> outCond = () => currentWaypoint > 14;

		// Action to get out of room
		System.Action getOutRoom = () => {

			previousWaypoint = currentWaypoint; // Set previous waypoint
			currentWaypoint = int.Parse(waypoints[currentWaypoint].transform.parent.gameObject.name.Substring(8)); // Set out of room waypoint

		};

		// In room condition for Selector
		System.Func<bool> inCond = () => (!inRoom && currentWaypoint != 3 && currentWaypoint != 6 && currentWaypoint != 10 && currentWaypoint != 13);

		// Action to get in room
		System.Action getInRoom = () => {

			previousWaypoint = currentWaypoint; // Set previous waypoint
			inRoom = true; // Set room checking boolean
			currentWaypoint = int.Parse(waypoints[currentWaypoint].GetComponentsInChildren<Transform>()[1].gameObject.name.Substring(8)); // Set in room waypoint

		};

		// Shortcut crossing condition for Selector
		System.Func<bool> acrossCond = () => (inRoom && !across && Random.value < 0.3);

		// Action to move across
		System.Action moveAcross = () => {

			previousWaypoint = currentWaypoint; // Set previous waypoint
			currentWaypoint = ((currentWaypoint + 6) % 14) + 1; // Set opposite side waypoint
			inRoom = false; // Reset room checking boolean
			across = true; // Set shortcut checking boolean
			roomTime = Random.Range(0, 100); // Reassign random inspection duration
			inspectTime = 0; // Reset inspection time

		};

		// Action to continue on clockwise circle
		System.Action continueOnPath = () => {

			previousWaypoint = currentWaypoint; // Set previous waypoint
			currentWaypoint++; // Set next waypoint on path
			if(currentWaypoint == 15)
			{
				currentWaypoint = 1; // Remap if out of range
			}

			across = false; // Reset shortcut checking

			// If previously in room checking
			if(inRoom)
			{
				inRoom = false; // Reset room checking boolean
				roomTime = Random.Range(0, 100); // Reassign random inspection duration
				inspectTime = 0; // Reset inspection time
			}

		};

		// Action to update guard velocity direction and speed
		System.Action updateVelocity = () => {

			velocity = moveDirection.normalized * speed; // Update velocity

		};

		// Action to initiate conversation
		System.Action converse = () => {

			// Check if in range of other guard
			if(InRange())
			{
				// If time has not elapsed
				if(elapsedTime < waitingTime)
				{
					velocity = Vector3.zero; // Stop moving
				}
				
				elapsedTime++; // Increment time

				// If enough time elasped since last conversation
				if(elapsedTime > recoveryTime + waitingTime)
				{
					elapsedTime = 0; // Reset time
				}
			}
			else
			{
				elapsedTime = 0; // Reset time
			}

			// If time has not elapsed and in room
			if(currentWaypoint < 15 && inRoom && inspectTime < roomTime)
			{
				inspectTime++; // Increment time
				velocity = Vector3.zero; // Stop moving
			}

		};

		// Action to watch for adventurer
		System.Action watch = () => {

			// If adventurer still collecting
			if(adventurer)
			{
				// Set ray tracing direction
				Vector3 dir = adventurer.transform.position - transform.position;
				Ray ray = new Ray(transform.position, dir);
				RaycastHit hit;

				// If angle less than 60 degree vision
				if(Vector3.Angle(dir, velocity) < 30.0f)
				{
					// If ray hits something
					if(Physics.Raycast(ray, out hit))
					{
						// If ray hits adventurer
						if(hit.collider.name == "Adventurer")
						{
							Destroy(hit.collider.gameObject); // Destroy adventurer
						}
					}
				}
				
				Debug.DrawRay(transform.position, dir, Color.green); // Ray between adventurer and guard
			}

		};

		// Behaviour Tree for guards
		System.Action root = 
			BehaviourTree.Sequencer(
				BehaviourTree.Selector(moveCond, 
			        BehaviourTree.Selector(outCond, getOutRoom, 
			        	BehaviourTree.Selector(inCond, getInRoom, 
			            	BehaviourTree.Selector(acrossCond, moveAcross, continueOnPath))), 
			    	BehaviourTree.Sequencer(updateVelocity, converse)), 
				watch);

		root(); // Call the root to initiate patrolling

		rigidbody.velocity = velocity; // Velocity of guard

		Debug.DrawRay(transform.position, velocity, Color.blue); // Ray of forward vision
	}

	// Checks whether guard is in range to converse
	private bool InRange() {

		// Iterate through all guards
		foreach(GameObject guard in Spawner.guards)
		{
			// Check if in range
			if(gameObject != guard && Vector3.Distance(transform.position, guard.transform.position) < 2.0f)
			{
				return true;
			}
		}

		return false;
	}
}
