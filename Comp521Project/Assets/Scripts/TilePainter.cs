using UnityEngine;
using System.Collections;
using Hexagon;

public class TilePainter : MonoBehaviour {

	public Material grey; // Default material
	public Material blue; // Range material
	public Material red; // Path material
	public Material green; // Obstacle material
	public Material yellow; // Start material

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Paints the start tile in yellow
	public void paintCurrentTile (IntVector2 index) {

		GameObject t = TileGenerator.tiles[index.x,index.y];

		if(t.tag == "Tile")
		{
			if(t.renderer)
			{
				t.renderer.material = yellow;
			}
			else
			{
				t.GetComponentsInChildren<Renderer>()[1].material = yellow;
			}
		}

	}

	// Paints the path tiles in red
	public void paintTargetTile (IntVector2 index) {

		GameObject t = TileGenerator.tiles[index.x,index.y];

		if(t.tag == "Tile")
		{
			if(t.renderer)
			{
				t.renderer.material = red;
			}
			else
			{
				t.GetComponentsInChildren<Renderer>()[1].material = red;
			}
		}

	}

	// Paints the range tiles in blue
	public void paintRangeTiles (IntVector2 index) {

		foreach(GameObject g in TileGenerator.tiles)
		{
			if(HexUtility.Distance(index, g.GetComponent<TileSelector>().index) <= PathFinder.range)
			{
				if(g.tag == "Tile")
				{
					if(g.renderer)
					{
						g.renderer.material = blue;
					}
					else
					{
						g.GetComponentsInChildren<Renderer>()[1].material = blue;
					}
				}
			}
		}

	}

	// Paints the obstacle tiles in green
	public void paintObstacle (IntVector2 index) {

		GameObject t = TileGenerator.tiles[index.x,index.y];

		if(t.tag == "Tile")
		{
			if(t.renderer)
			{
				t.renderer.material = green;
			}
			else
			{
				t.GetComponentsInChildren<Renderer>()[1].material = green;
			}

			t.tag = "Obstacle";
		}
		else
		{
			if(t.renderer)
			{
				t.renderer.material = grey;
			}
			else
			{
				t.GetComponentsInChildren<Renderer>()[1].material = grey;
			}

			t.tag = "Tile";
		}

	}

	// Paints the range tiles in blue with the presence of obstacles
	public void paintRangeTilesWithObstacles (IntVector2 index) {

		ArrayList visited = new ArrayList();
		ArrayList[] fringes = new ArrayList[PathFinder.range+1];
		fringes[0] = new ArrayList();

		visited.Add(index);
		fringes[0].Add(index);

		for(int i = 1; i <= PathFinder.range; i++)
		{
			fringes[i] = new ArrayList();

			foreach(IntVector2 v in fringes[i-1])
			{
				foreach(IntVector2 t in HexUtility.Neighbours(v))
				{
					if(t != new IntVector2(-1,-1))
					{
						GameObject g = TileGenerator.tiles[t.x,t.y];

						if(!visited.Contains(t) && g.tag == "Tile")
						{
							visited.Add(t);
							fringes[i].Add(t);

							if(g.renderer)
							{
								g.renderer.material = blue;
							}
							else
							{
								g.GetComponentsInChildren<Renderer>()[1].material = blue;
							}
						}
					}
				}
			}
		}
	}

	// Resets tiles to their default material
	public void unpaintTiles () {

		foreach(GameObject g in TileGenerator.tiles)
		{
			if(g.tag == "Tile")
			{
				if(g.renderer)
				{
					g.renderer.material = grey;
				}
				else
				{
					g.GetComponentsInChildren<Renderer>()[1].material = grey;
				}
			}
		}

	}
}
