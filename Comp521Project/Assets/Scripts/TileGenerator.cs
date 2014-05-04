using UnityEngine;
using System.Collections;
using Hexagon;
using System.Diagnostics;

public class TileGenerator : MonoBehaviour {

	public GameObject tile; // Default tile
	public GameObject grass; // Grass tile
	public GameObject water; // Water tile
	public GameObject lava; // Lava tile
	public GameObject ice; // Ice tile
	public GameObject rockBig; // Big Rock tile
	public GameObject rockMedium; // Medium Rock tile
	public GameObject rockSmall; // Small Rock tile
	public GameObject tree; // Tree tile
	public GameObject mushroom; // Mushroom tile
	public static GameObject[,] tiles; // Array storing tiles by indices
	public static int gridSize = 16; // Grid size
	public bool proceduralMap = false; // Boolean for procedural generation
	public bool perlinNoise = false; // Boolean for Perlin Noise map generation
	public float scale = 2.0f; // Scale factor of Perlin Noise

	// Use this for initialization
	void Start () {
	
		if(proceduralMap)
		{
			gridSize = 32;
		}

		tiles = new GameObject[gridSize,gridSize];

		// Timer for map generation running time
		Stopwatch watch = new Stopwatch();

		watch.Start();

		// Generate map
		GenerateMap();

		watch.Stop();

		UnityEngine.Debug.Log("Time: " + watch.ElapsedMilliseconds + " ms");

		GameObject map = GameObject.Find("Map");

		if(!proceduralMap)
		{
			IntVector2 origin = new IntVector2(0,0);

			map.SendMessage("paintRangeTiles", origin);
			map.SendMessage("paintCurrentTile", origin);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Generates a procedural map
	private void GenerateMap () {

		Vector3 prefabPos = tile.transform.position;
		Quaternion prefabRot = tile.transform.rotation;
		
		Vector3 offsetX = new Vector3(7,0,4);
		Vector3 offsetY = new Vector3(-7,0,4);

		// Choose random start coordinates on Perlin Noise
		float randX = Random.Range(0.0f, 100.0f);
		float randY = Random.Range(0.0f, 100.0f);

		for(int i = 0; i < gridSize; i++)
		{
			for(int j = 0; j < gridSize; j++)
			{
				GameObject t = null;

				if(proceduralMap)
				{
					float x = randX + (float)i / (float)gridSize * scale;
					float y = randY + (float)j / (float)gridSize * scale;

					// Generate noise based on grid size and scale parameter
					float noise = Mathf.PerlinNoise(x,y);

					// Assign random values if random generation
					if(!perlinNoise)
					{
						noise = Random.value;
					}

					// Select tile type based on height map values
					if(noise < 0.1f)
					{
						prefabPos = ice.transform.position;
						prefabRot = ice.transform.rotation;

						t = (GameObject)Instantiate(ice, prefabPos + offsetX*i + offsetY*j, prefabRot);
					}
					else if(noise < 0.3f)
					{
						prefabPos = water.transform.position;
						prefabRot = water.transform.rotation;
						
						t = (GameObject)Instantiate(water, prefabPos + offsetX*i + offsetY*j, prefabRot);
					}
					else if(noise < 0.4f)
					{
						prefabPos = tree.transform.position;
						prefabRot = tree.transform.rotation;
						
						t = (GameObject)Instantiate(tree, prefabPos + offsetX*i + offsetY*j, prefabRot);
					}
					else if(noise < 0.45f)
					{
						prefabPos = mushroom.transform.position;
						prefabRot = mushroom.transform.rotation;
						
						t = (GameObject)Instantiate(mushroom, prefabPos + offsetX*i + offsetY*j, prefabRot);
					}
					else if(noise < 0.7f)
					{
						prefabPos = grass.transform.position;
						prefabRot = grass.transform.rotation;
						
						t = (GameObject)Instantiate(grass, prefabPos + offsetX*i + offsetY*j, prefabRot);
					}
					else if(noise < 0.8f)
					{
						prefabPos = lava.transform.position;
						prefabRot = lava.transform.rotation;
						
						t = (GameObject)Instantiate(lava, prefabPos + offsetX*i + offsetY*j, prefabRot);
					}
					else if(noise < 0.85f)
					{
						prefabPos = rockSmall.transform.position;
						prefabRot = rockSmall.transform.rotation;
						
						t = (GameObject)Instantiate(rockSmall, prefabPos + offsetX*i + offsetY*j, prefabRot);
					}
					else if(noise < 0.9f)
					{
						prefabPos = rockMedium.transform.position;
						prefabRot = rockMedium.transform.rotation;
						
						t = (GameObject)Instantiate(rockMedium, prefabPos + offsetX*i + offsetY*j, prefabRot);
					}
					else
					{
						prefabPos = rockBig.transform.position;
						prefabRot = rockBig.transform.rotation;

						t = (GameObject)Instantiate(rockBig, prefabPos + offsetX*i + offsetY*j, prefabRot);
					}

					// Randomly rotate tiles for smoother effect of neighboring tiles
					t.transform.Rotate(Vector3.up, Random.Range(0, 6) * 60, Space.Self);
				}
				else
				{
					// Generate default tiles if no map generation procedure
					t = (GameObject)Instantiate(tile, prefabPos + offsetX*i + offsetY*j, prefabRot);
				}

				t.transform.parent = transform;
				tiles[i,j] = t;
				t.GetComponent<TileSelector>().index = new IntVector2(i,j);
			}
		}

	}
}
