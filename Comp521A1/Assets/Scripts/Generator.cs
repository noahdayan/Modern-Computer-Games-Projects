using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Generator : MonoBehaviour {

	private GameObject[] grid;
	private List<GameObject> walls;
	private bool[] inMaze;

	// Use this for initialization
	void Start () {
	
		// Cells are numbered from 0 to 24 from bottom left to top right.
		// first row: 0-4, second row: 5-9, third row: 10-14, forth row: 15-19, fifth row: 20-24.
		// Walls are named using adjacent cell numbers. e.g.: Wall05-10 adjacent to cell 5 and 10.
		// Using Randomized Prim's Algorithm

		// get all wall gameobjects in grid from transforms.
		Transform[] children = gameObject.GetComponentsInChildren<Transform>();

		grid = new GameObject[40];
		int i = 0;
		foreach(Transform t in children)
		{
			if(t.name.Contains("Wall"))
			{
				grid[i] = t.gameObject;
				i++;
			}
		}

		// array to store if cell is in maze.
		inMaze = new bool[25];
		for(int j = 0; j < 25; j++)
		{
			inMaze[j] = false;
		}

		// select random starting wall and put in temporary list of processed walls.
		walls = new List<GameObject>();

		int r = Random.Range(0,25);
		inMaze[r] = true;
		foreach(GameObject g in grid)
		{
			if(r < 10)
			{
				if(g.name.Contains("0" + r.ToString()))
				{
					walls.Add(g);
				}
			}
			else
			{
				if(g.name.Contains(r.ToString()))
				{
					walls.Add(g);
				}
			}
		}

		// repeat until all walls processed.
		while(walls.Count != 0)
		{
			// pick random wall from list and find adjacent cells.
			GameObject w = walls[Random.Range(0,walls.Count)];
			int first = int.Parse(w.name.Substring(4,2));
			int second = int.Parse(w.name.Substring(7,2));

			// if both cells are already in maze, keep wall and remove from processed walls.
			if(inMaze[first] && inMaze[second])
			{
				walls.Remove(w);
			}
			// if first one is in maze then make second one in maze and destroy wall between them.
			else if(inMaze[first])
			{
				inMaze[second] = true;
				w.SetActive(false);

				foreach(GameObject g in grid)
				{
					if(second < 10)
					{
						if(g.name.Contains("0" + second.ToString()))
						{
							walls.Add(g);
						}
					}
					else
					{
						if(g.name.Contains(second.ToString()))
						{
							walls.Add(g);
						}
					}
				}
			}
			// if second one is in maze then make first one in maze and destroy wall between them
			else if(inMaze[second])
			{
				inMaze[first] = true;
				w.SetActive(false);
				
				foreach(GameObject g in grid)
				{
					if(first < 10)
					{
						if(g.name.Contains("0" + first.ToString()))
						{
							walls.Add(g);
						}
					}
					else
					{
						if(g.name.Contains(first.ToString()))
						{
							walls.Add(g);
						}
					}
				}
			}
		}

		// get all remaining active walls
		List<GameObject> active = new List<GameObject>();
		foreach(GameObject g in grid)
		{
			if(g.activeSelf)
			{
				active.Add(g);
			}
		}

		// pick 5-10 random walls and make them breakable.
		r = Random.Range(5,11);
		Debug.Log(r);
		while(r > 0)
		{
			int rand = Random.Range(0,active.Count);
			active[rand].tag = "Wall";
			active.RemoveAt(rand);

			r--;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
