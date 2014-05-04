using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hexagon;
using System.Diagnostics;

public class PathFinder : MonoBehaviour {

	private IntVector2 currentIndex; // Current index tile
	private bool isMoving; // Boolean for movement
	private GameObject map; // Map reference
	public static int range = 4; // Max range distance for highlighting
	public float speed = 4.0f; // Speed of character movement
	private float startTime; // Start time of unit movement
	private Vector3 startTile; // Start tile of movement
	private Vector3 endTile; // End tile of movement
	private ArrayList pathTiles; // Array of tiles on path
	public bool useDFS = false; // Boolean for DFS
	public bool useBFS = false; // Boolean for BFS
	public bool useAStar = true; // Boolean for AStar

	// Use this for initialization
	void Start () {
	
		currentIndex = new IntVector2(0,0);
		isMoving = false;

		map = GameObject.Find("Map");

		pathTiles = new ArrayList();
	}
	
	// Update is called once per frame
	void Update () {
	
		// Only move if movement initiated
		if(isMoving)
		{
			float percentage = (Time.time - startTime) * speed;

			// Linear Interpolation of position for movement
			transform.position = Vector3.Lerp(startTile, endTile, percentage);

			// Update variables if checkpoint reached
			if(percentage >= 1.0f)
			{
				if(pathTiles.Count == 0)
				{
					isMoving = false;
					resetState();
				}
				else
				{
					startTime = Time.time;

					IntVector2 path = (IntVector2)pathTiles[0];
					pathTiles.RemoveAt(0);

					startTile = endTile;
					endTile = TileGenerator.tiles[path.x,path.y].transform.position;
					endTile.y = 5.0f;

					currentIndex = path;
				}
			}
		}

	}

	// Computes the line of sight between two tiles
	private void LineOfSight (IntVector2 start, IntVector2 end) {

		int dx = end.x - start.x;
		int dy = end.y - start.y;

		bool sign = Mathf.Sign(dx) == Mathf.Sign(dy);

		int xOne;
		int yOne;

		if(dx < 0)
		{
			xOne = -1;
		}
		else
		{
			xOne = 1;
		}
		if(dy < 0)
		{
			yOne = -1;
		}
		else
		{
			yOne = 1;
		}

		if(Mathf.Abs(dx) >= Mathf.Abs(dy))
		{
			if(dx % 2 == 0)
			{
				dx *= 2;
				dy *= 2;
			}
		}
		else
		{
			if(dy % 2 == 0)
			{
				dx *= 2;
				dy *= 2;
			}
		}

		dx = Mathf.Abs(dx);
		dy = Mathf.Abs(dy);

		int factor;

		if(dx >= dy)
		{
			factor = dx/2;
		}
		else
		{
			factor = dy/2;
		}

		int x = start.x;
		int y = start.y;

		IntVector2 newPath = new IntVector2(x,y);
		
		pathTiles.Add(newPath);
		map.SendMessage("paintCurrentTile", newPath);

		while(x != end.x || y != end.y)
		{
			if(dx >= dy)
			{
				factor += dy;

				if(factor >= dx)
				{
					factor -= dx;

					if(sign)
					{
						x += xOne;
						y += yOne;
					}
					else
					{
						x += xOne;

						newPath = new IntVector2(x,y);

						pathTiles.Add(newPath);
						map.SendMessage("paintTargetTile", newPath);

						y += yOne;
					}
				}
				else
				{
					x += xOne;
				}
			}
			else
			{
				factor += dx;
				
				if(factor >= dy)
				{
					factor -= dy;
					
					if(sign)
					{
						x += xOne;
						y += yOne;
					}
					else
					{
						y += yOne;

						newPath = new IntVector2(x,y);

						pathTiles.Add(newPath);
						map.SendMessage("paintTargetTile", newPath);
						
						x += xOne;
					}
				}
				else
				{
					y += yOne;
				}
			}

			newPath = new IntVector2(x,y);

			pathTiles.Add(newPath);
			map.SendMessage("paintTargetTile", newPath);
		}
	}

	// Pathfinding using Depth First Search
	private void DepthFirstSearch(IntVector2 start, IntVector2 destination)
	{
		Stack<Node> s = new Stack<Node>();
		List<IntVector2> visited = new List<IntVector2>();

		visited.Add(start);

		foreach(IntVector2 n in HexUtility.Neighbours(start))
		{
			if(n != new IntVector2(-1,-1) && TileGenerator.tiles[n.x,n.y].tag == "Tile")
			{
				s.Push(new Node(n, new Node(start, null)));
			}
		}
		
		while(s.Count != 0)
		{
			var node = s.Pop();
			
			visited.Add(node.LastNode);
			
			foreach(IntVector2 n in HexUtility.Neighbours(node.LastNode))
			{
				var child = new Node(n, node);
				
				if(n.Equals(destination))
				{
					while(child.PreviousNode != null)
					{
						pathTiles.Add(child.LastNode);
						map.SendMessage("paintTargetTile", child.LastNode);
						
						child = child.PreviousNode;
					}
					
					pathTiles.Add(start);
					map.SendMessage("paintCurrentTile", start);
					
					pathTiles.Reverse();
					
					return;
				}
				else if(!visited.Contains(n))
				{
					if(n != new IntVector2(-1,-1) && TileGenerator.tiles[n.x,n.y].tag == "Tile")
					{
						s.Push(child);
					}
				}
			}
			
		}
	}

	// Pathfinding using Breadth First Search
	private void BreadthFirstSearch(IntVector2 start, IntVector2 destination)
	{
		Queue<Node> q = new Queue<Node>();
		List<IntVector2> visited = new List<IntVector2>();

		q.Enqueue(new Node(start, null));

		while(q.Count != 0)
		{
			var node = q.Dequeue();

			visited.Add(node.LastNode);

			foreach(IntVector2 n in HexUtility.Neighbours(node.LastNode))
			{
				var child = new Node(n, node);

				if(n.Equals(destination))
				{
					while(child.PreviousNode != null)
					{
						pathTiles.Add(child.LastNode);
						map.SendMessage("paintTargetTile", child.LastNode);

						child = child.PreviousNode;
					}

					pathTiles.Add(start);
					map.SendMessage("paintCurrentTile", start);

					pathTiles.Reverse();

					return;
				}
				else if(!visited.Contains(n))
				{
					if(n != new IntVector2(-1,-1) && TileGenerator.tiles[n.x,n.y].tag == "Tile")
					{
						q.Enqueue(child);
					}
				}
			}

		}
	}

	// Pathfinding using AStar
	private static Path AStar(IntVector2 start, IntVector2 destination)
	{
		var closed = new HashSet<IntVector2>();
		var queue = new PriorityQueue<int, Path>();

		queue.Enqueue(0, new Path(start));

		while(!queue.isEmpty)
		{
			var path = queue.Dequeue();

			if(closed.Contains(path.LastStep))
			{
				continue;
			}
			if(path.LastStep.Equals(destination))
			{
				return path;
			}

			closed.Add(path.LastStep);

			foreach(IntVector2 n in HexUtility.Neighbours(path.LastStep))
			{
				if(n != new IntVector2(-1,-1) && TileGenerator.tiles[n.x,n.y].tag == "Tile")
				{
					var newPath = path.AddStep(n, 1);

					queue.Enqueue(newPath.TotalCost + HexUtility.Distance(n, destination), newPath);
				}
			}
		}

		return null;
	}

	// Function to initiate movement and choose pathfinding algorithm
	public void moveToTile (IntVector2 index) {

		// Only move if not moving
		if(!isMoving)
		{
			map.SendMessage("unpaintTiles");

			// Timer for pathfinding running time
			Stopwatch watch = new Stopwatch();

			watch.Start();

			// AStar
			if(useAStar)
			{
				Path p = AStar(currentIndex, index);
				while(p.PreviousSteps != null)
				{
					pathTiles.Add(p.LastStep);
					map.SendMessage("paintTargetTile", p.LastStep);
					p = p.PreviousSteps;
				}

				pathTiles.Add(currentIndex);
				map.SendMessage("paintCurrentTile", currentIndex);

				pathTiles.Reverse();
			}
			else if(useDFS)
			{
				// DFS
				DepthFirstSearch(currentIndex, index);
			}
			else if(useBFS)
			{
				//BFS
				BreadthFirstSearch(currentIndex, index);
			}
			else
			{
				// LOS if no boolean checked
				LineOfSight(currentIndex, index);
			}

			watch.Stop();

			UnityEngine.Debug.Log("Time: " + watch.ElapsedMilliseconds + " ms");

			IntVector2 path = (IntVector2)pathTiles[0];
			pathTiles.RemoveAt(0);
			
			startTile = TileGenerator.tiles[path.x,path.y].transform.position;
			startTile.y = 5.0f;
			
			path = (IntVector2)pathTiles[0];
			pathTiles.RemoveAt(0);
			
			endTile = TileGenerator.tiles[path.x,path.y].transform.position;
			endTile.y = 5.0f;

			currentIndex = path;

			startTime = Time.time;
			isMoving = true;
		}

	}

	// Reset grid color after each state update
	public void resetState () {

		map.SendMessage("unpaintTiles");
		map.SendMessage("paintRangeTilesWithObstacles", currentIndex);
		map.SendMessage("paintCurrentTile", currentIndex);

	}
}
