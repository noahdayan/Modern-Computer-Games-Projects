using UnityEngine;
using System.Collections;

public class Achiever : MonoBehaviour {

	private GameObject[] waypoints;
	private ArrayList treasures;
	private int currentWaypoint;
	private int previousWaypoint;
	public float speed;

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
		
		currentWaypoint = 0; // Current target waypoint index
		previousWaypoint = 0; // Previous target waypoint index

		treasures = new ArrayList(); // Array of uncollected treasures

		// Store all treasures by pair
		for(int i = 0; i < 10; i++)
		{
			treasures.Add(i+15);
		}

	}
	
	// Update is called once per frame
	void Update () {
	
		// Set movement parameters from waypoint position
		Vector3 target = waypoints[currentWaypoint].transform.position;
		Vector3 moveDirection = target - transform.position;
		Vector3 velocity = rigidbody.velocity;

		// Move condition for Selector
		System.Func<bool> moveCond = () => moveDirection.magnitude < 0.1;

		// Center condition for Selector
		System.Func<bool> centerCond = () => currentWaypoint == 0;

		// Action to follow guard closely from behind
		System.Action followGuard = () => {
			
			previousWaypoint = currentWaypoint; // Set previous waypoint
			if(treasures.Count == 0)
			{
				Destroy(gameObject); // Destroy adventurer if no more treasure and at the center
			}
			
			int index = 0; // First waypoint index
			if(Spawner.guards.Count == 0)
			{
				index = 1; // Go to any waypoint on outer circle if no guards
			}
			else if(Spawner.guards.Count == 1)
			{
				// Get waypoint of guard
				index = ((GameObject)Spawner.guards[0]).GetComponent<Patroller>().currentWaypoint;

				if(index <= 2)
				{
					index += 14; // Remap if out of range
				}
				
				index -= 2; // Create distance of 2 waypoints

				// Make sure the waypoint is not on the corners
				if(index == 3 || index == 6 || index == 10 || index == 13)
				{
					index--;
				}
			}
			else
			{
				// Sum up the indices of guards
				foreach(GameObject guard in Spawner.guards)
				{
					index += guard.GetComponent<Patroller>().currentWaypoint;
				}
				index /= Spawner.guards.Count; // Average the indices
				
				index %= 14; // Remap if out of range

				// Make sure the waypoint is not on the corners
				if(index == 3 || index == 6 || index == 10 || index == 13)
				{
					index--;
				}

				index = ((index + 6) % 14) + 1; // Set the opposite waypoint
			}
			
			currentWaypoint = index; // Set first waypoint
			
		};

		// Out of room condition for Selector
		System.Func<bool> outCond = () => currentWaypoint > 14;

		// Action to get out of room
		System.Action getOutRoom = () => {
			
			previousWaypoint = currentWaypoint; // Set previous waypoint
			if(treasures.Contains(currentWaypoint))
			{
				treasures.Remove(currentWaypoint); // Remove treasure if uncollected
				currentWaypoint = ((currentWaypoint - 15) * 4) + 25; // Set waypoint in room left
			}
			else
			{
				// If waypoint in room right
				if(currentWaypoint < 25 || currentWaypoint == ((int.Parse(waypoints[currentWaypoint].transform.parent.gameObject.name.Substring(8)) - 15) * 4) + 28)
				{
					currentWaypoint = int.Parse(waypoints[currentWaypoint].transform.parent.gameObject.name.Substring(8)); // Set waypoint out of room
				}
				else
				{
					currentWaypoint++; // Set next waypoint in room
				}
			}
			
		};

		// In room condition for Selector
		System.Func<bool> inCond = () => (currentWaypoint != 3 && currentWaypoint != 6 && currentWaypoint != 10 && currentWaypoint != 13);

		// Action to get in room
		System.Action getInRoom = () => {

			bool set = false; // Boolean check for waypoint setting
			bool same = (currentWaypoint == ((previousWaypoint + 6) % 14) + 1); // Boolean check for repeated crossings

			previousWaypoint = currentWaypoint; // Set previous waypoint

			// Only if one guard
			if(Spawner.guards.Count == 1)
			{
				// Get the guard
				GameObject guard = (GameObject)Spawner.guards[0];

				// If not already crossed, and guard not on opposite side, and guard crossing
				if(!same && guard.GetComponent<Patroller>().currentWaypoint != ((currentWaypoint + 6) % 14) + 1 && guard.GetComponent<Patroller>().currentWaypoint == ((guard.GetComponent<Patroller>().previousWaypoint + 6) % 14) + 1)
				{
					currentWaypoint = ((currentWaypoint + 6) % 14) + 1; // Set waypoint to opposite side
					set = true; // Check the waypoint setting
					if(treasures.Count == 0)
					{
						currentWaypoint = 0; // Stop at center if no more treasures
					}
				}
			}

			// If waypoint not already set
			if(!set)
			{
				// If there are uncollected treasures and room still contains uncollected treasures
				if(treasures.Count != 0 && treasures.Contains(int.Parse(waypoints[currentWaypoint].GetComponentsInChildren<Transform>()[1].gameObject.name.Substring(8))))
				{
					currentWaypoint = int.Parse(waypoints[currentWaypoint].GetComponentsInChildren<Transform>()[1].gameObject.name.Substring(8)); // Go in room
				}
				else if(treasures.Count == 0)
				{
					currentWaypoint = 0; // Return to center if no more treasures
				}
				else
				{
					currentWaypoint++; // Set next waypoint on path
					if(currentWaypoint == 15)
					{
						currentWaypoint = 1; // Remap if out of range
					}
				}
			}

		};

		// Action to continue on clockwise circle
		System.Action continueOnPath = () => {

			previousWaypoint = currentWaypoint; // Set previous waypoint
			currentWaypoint++; // Set next waypoint on path
			if(currentWaypoint == 15)
			{
				currentWaypoint = 1; // Remap if out of range
			}

		};

		// Action to update guard velocity direction and speed
		System.Action updateVelocity = () => {

			velocity = moveDirection.normalized * speed; // Update velocity

		};

		// Action to watch for guards
		System.Action watch = () => {

			// Strategy for one guard
			if(Spawner.guards.Count == 1)
			{
				// Get the guard
				GameObject guard = (GameObject)Spawner.guards[0];

				int newerIndex = currentWaypoint + 2; // Stores 2 waypoints behind guard
				int newIndex = currentWaypoint + 1; // Stores 1 waypoint behind guard
				if(newerIndex > 14)
				{
					newerIndex -= 14; // Remap if out of range
				}
				if(newIndex > 14)
				{
					newIndex -= 14; // Remap if out of range
				}
				if(newerIndex == 3 || newerIndex == 6 || newerIndex == 10 || newerIndex == 13)
				{
					newerIndex++; // Make sure waypoint is not corner
				}
				if(newIndex == 3 || newIndex == 6 || newIndex == 10 || newIndex == 13)
				{
					newIndex++; // Make sure waypoint is not corner
				}

				// If close to guard, or going to center but guard is either crossing or getting out of room, or there are less than 2 waypoints between the current or previous guard waypoint
				if(Vector3.Distance(transform.position, guard.transform.position) < 15.0f || (currentWaypoint == 0 && (guard.GetComponent<Patroller>().previousWaypoint > 14 || (guard.GetComponent<Patroller>().currentWaypoint == ((guard.GetComponent<Patroller>().previousWaypoint + 6) % 14) + 1))) || (currentWaypoint != 0 && previousWaypoint != 0 && currentWaypoint < 15 && (currentWaypoint != ((previousWaypoint + 6) % 14) + 1) && ((guard.GetComponent<Patroller>().currentWaypoint > 14 && (guard.GetComponent<Patroller>().previousWaypoint == newIndex || guard.GetComponent<Patroller>().previousWaypoint == newerIndex)) || (guard.GetComponent<Patroller>().currentWaypoint == newIndex || guard.GetComponent<Patroller>().currentWaypoint == newerIndex))))
				{
					velocity = Vector3.zero; // Stop collecting
				}
			}
			else
			{
				// Iterate through all guards
				foreach(GameObject guard in Spawner.guards)
				{
					// Get next waypoint on outer path
					int nextIndex = 0;
					if(guard.GetComponent<Patroller>().currentWaypoint < 15)
					{
						nextIndex = (guard.GetComponent<Patroller>().currentWaypoint % 14) + 1;
					}

					Vector3 origin = waypoints[0].transform.position; // Origin position
					Vector3 vel = guard.rigidbody.velocity; // Guard velocity
					Vector3 between = guard.transform.position - transform.position; // Direction between guard and adventurer

					Vector3 dir = waypoints[guard.GetComponent<Patroller>().currentWaypoint].transform.position - target; // Direction to current guard waypoint
					Vector3 prevDir = waypoints[guard.GetComponent<Patroller>().previousWaypoint].transform.position - target; // Direction to previous guard waypoint
					Vector3 nextDir = waypoints[nextIndex].transform.position - target; // Direction to next guard waypoint

					float dist = Vector3.Distance(waypoints[guard.GetComponent<Patroller>().currentWaypoint].transform.position, target); // Distance to current guard waypoint
					float prevDist = Vector3.Distance(waypoints[guard.GetComponent<Patroller>().previousWaypoint].transform.position, target); // Distance to previous guard waypoint
					float nextDist = Vector3.Distance(waypoints[nextIndex].transform.position, target); // Distance to next guard waypoint
					float centerDist = Vector3.Distance(origin, target); // Distance from center

					bool across = (guard.GetComponent<Patroller>().currentWaypoint == ((guard.GetComponent<Patroller>().previousWaypoint + 6) % 14) + 1); // Boolean check for crossing

					Vector3 centerAdv = target - origin; // Direction from origin to waypoint
					Vector3 centerDir = origin - waypoints[guard.GetComponent<Patroller>().currentWaypoint].transform.position; // Direction from current guard waypoint to origin
					Vector3 centerPrevDir = origin - waypoints[guard.GetComponent<Patroller>().previousWaypoint].transform.position; // Direction from previous guard waypoint to origin

					Ray ray = new Ray(target, dir); // Ray to current guard waypoint
					Ray prevRay = new Ray(target, prevDir); // Ray to previous guard waypoint
					Ray nextRay = new Ray(target, nextDir); // Ray to next guard waypoint
					Ray centerRay = new Ray(origin, target - origin); // Ray from center waypoint

					// If visible from current, previous, next or center waypoint
					if(!Physics.Raycast(ray, dist) || !Physics.Raycast(prevRay, prevDist) || (nextIndex != 0 && !Physics.Raycast(nextRay, nextDist)) || (across && Vector3.Angle(target - origin, vel) < 30.0f && !Physics.Raycast(centerRay, centerDist)))
					{
						// If treasures remaining and in room
						if(treasures.Count != 0 && (previousWaypoint > 24 && currentWaypoint > 24))
						{
							velocity = Vector3.zero; // Stop collecting
							break;
						}
					}

					// If guard facing adventurer
					if(Vector3.Dot(vel, between) < 0)
					{
						vel.Scale(new Vector3(-1, 0, -1));

						// If in angle of vision
						if(Vector3.Angle(vel, velocity) < 30.0f)
						{
							// If hidden in room
							if(previousWaypoint > 24 && currentWaypoint > 24)
							{
								velocity = Vector3.zero; // Stop collecting
								break;
							}
						}
					}

					// If visible from center when coming from current or previous waypoint
					if(Vector3.Angle(centerAdv, centerDir) < 30.0f || Vector3.Angle(centerAdv, centerPrevDir) < 30.0f)
					{
						// If hidden in room
						if(previousWaypoint > 24 && currentWaypoint > 24)
						{
							velocity = Vector3.zero; // Stop collecting
							break;
						}
					}
				}
			}
		};

		// Behaviour Tree for adventurer
		System.Action root = 
			BehaviourTree.Sequencer(
				BehaviourTree.Selector(moveCond, 
			    	BehaviourTree.Selector(centerCond, followGuard, 
			        	BehaviourTree.Selector(outCond, getOutRoom, 
			            	BehaviourTree.Selector(inCond, getInRoom, continueOnPath))), 
			        updateVelocity), 
				watch);

		root(); // Call the root to initiate collecting

		rigidbody.velocity = velocity; // Velocity of adventurer

		Debug.DrawRay(transform.position, velocity, Color.red); // Ray of forward direction
	}
}
