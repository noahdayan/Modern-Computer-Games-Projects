using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Namespace to call utility functions in other classes
namespace Hexagon
{
	// Custom structure for int Vector2
	public struct IntVector2
	{
		public int x;
		public int y;
		
		public IntVector2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static bool operator == (IntVector2 v1, IntVector2 v2)
		{
			return ((v1.x == v2.x) && (v1.y == v2.y));
		}

		public static bool operator != (IntVector2 v1, IntVector2 v2)
		{
			return ((v1.x != v2.x) || (v1.y != v2.y));
		}

		public override bool Equals(object o)
		{
			try
			{
				return (bool) (this == (IntVector2) o);
			}
			catch
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}

	// Class for distance and neighbor utility functions
	public class HexUtility : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		// Calculates distance between two tiles
		public static int Distance (IntVector2 tile1, IntVector2 tile2) {
			
			int dx = tile2.x - tile1.x;
			int dy = tile2.y - tile1.y;
			
			if(Mathf.Sign(dx) == Mathf.Sign(dy))
			{
				return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
			}
			else
			{
				return Mathf.Abs(dx) + Mathf.Abs(dy);
			}
			
		}

		// Computes all neighbors of a specific tile
		public static IntVector2[] Neighbours (IntVector2 index) {
			
			IntVector2[] neighbours = new IntVector2[6];
			
			neighbours[0] = new IntVector2(index.x + 1, index.y    );
			neighbours[1] = new IntVector2(index.x + 1, index.y + 1);
			neighbours[2] = new IntVector2(index.x    , index.y + 1);
			neighbours[3] = new IntVector2(index.x - 1, index.y    );
			neighbours[4] = new IntVector2(index.x - 1, index.y - 1);
			neighbours[5] = new IntVector2(index.x    , index.y - 1);

			// Check for off-grid indices
			for(int i = 0; i < 6; i++)
			{
				if(neighbours[i].x < 0 || neighbours[i].y < 0 || neighbours[i].x > TileGenerator.gridSize - 1 || neighbours[i].y > TileGenerator.gridSize - 1)
				{
					neighbours[i] = new IntVector2(-1, -1);
				}
			}
			
			return neighbours;
			
		}
	}

	// Node class to store parent node in graph traversal algorithms
	public class Node
	{
		public IntVector2 LastNode
		{
			get;
			private set;
		}
		
		public Node PreviousNode
		{
			get;
			private set;
		}
		
		public Node(IntVector2 lastNode, Node previousNode)
		{
			LastNode = lastNode;
			PreviousNode = previousNode;
		}
	}

	// Path implementation for traceback in AStar algorithm
	public class Path
	{
		public IntVector2 LastStep
		{
			get;
			private set;
		}

		public Path PreviousSteps
		{
			get;
			private set;
		}

		public int TotalCost
		{
			get;
			private set;
		}

		private Path(IntVector2 lastStep, Path previousSteps, int totalCost)
		{
			LastStep = lastStep;
			PreviousSteps = previousSteps;
			TotalCost = totalCost;
		}

		public Path(IntVector2 start) : this(start, null, 0) {}

		public Path AddStep(IntVector2 step, int stepCost)
		{
			return new Path(step, this, TotalCost + stepCost);
		}
	}

	// Priority queue implementation for AStar Algorithm
	public class PriorityQueue <P,V> {

		private SortedDictionary<P,Queue<V>> list = new SortedDictionary<P, Queue<V>>();

		public void Enqueue (P priority, V value) {

			Queue<V> q;

			if(!list.TryGetValue(priority, out q))
			{
				q = new Queue<V>();
				list.Add(priority, q);
			}

			q.Enqueue(value);

		}

		public V Dequeue () {

			var pair = list.First();
			var v = pair.Value.Dequeue();

			if(pair.Value.Count == 0)
			{
				list.Remove(pair.Key);
			}

			return v;

		}

		public bool isEmpty {

			get
			{
				return !list.Any();
			}

		}

	}
}