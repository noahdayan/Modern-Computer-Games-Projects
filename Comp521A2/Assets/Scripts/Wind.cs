using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour {
	
	private Texture2D[] textures; // textures for wind bar
	public static float[] noise; // perlin noise values
	private float[] random; // random values for perlin noise
	public static float counter; // counter for wind shift

	// Use this for initialization
	void Start () {
	
		// set wind description
		gameObject.guiText.text = "Wind:\n    Red: Left\n    Green: Right";

		counter = 0.0f; // initialize shift counter

		noise = new float[100]; // initialize noise array
		textures = new Texture2D[noise.Length]; // initialize texture array

		random = new float[10]; // initialize random value array

		// set random values
		for(int i = 0; i < random.Length; i++)
		{
			random[i] = Random.value;
		}

		// generate perlin noise and set color to textures
		float x = 0.0f;
		for(int i = 0; i < textures.Length; i++)
		{
			noise[i] = PerlinNoise(x);
			textures[i] = MakeTexture(Color.Lerp(Color.green, Color.red, noise[i]));
			x += 0.1f;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// GUI display
	void OnGUI () {

		// create style for boxes
		GUIStyle style = new GUIStyle(GUI.skin.box);

		// generates boxes for color-coded vertical wind bar
		for(int i = 0; i < textures.Length; i++)
		{
			// shift wind gradually
			int index = (i + (int)counter) % textures.Length;
			if(index < 0)
			{
				index += textures.Length;
			}

			// assign textures and create boxes
			style.normal.background = textures[index];
			GUI.Box(new Rect(950.0f, i * 5.0f, 10.0f, 5.0f), "", style);
		}

		// randomly shift the wind
		if(Random.value > 0.6f)
		{
			counter += 0.1f;
		}
		else
		{
			counter -= 0.05f;
		}

	}

	// 1D Perlin Noise generator
	float PerlinNoise (float x) {

		int xi = (int)x; // get closest integer lower bound
		int xMin = xi % random.Length; // map to range of random values
		float t = x - xi; // get delta from lower bound
		int xMax = xMin + 1; // get closest integer upper bound
		if(xMin == random.Length - 1)
		{
			xMax = 0; // loop index out of range
		}

		t = t * t * (3 - 2 * t); // smooth step function

		return random[xMin] * (1 - t) + random[xMax] * t; // return interpolated noise value
	}

	// make simple texture
	Texture2D MakeTexture (Color c) {

		Color[] pixel = new Color[4]; // initialize 2 by 2 pixels

		// assign same color to all pixels
		for(int i = 0; i < 4; i++)
		{
			pixel[i] = c;
		}

		// create texture and assign pixels
		Texture2D texture = new Texture2D(2,2);
		texture.SetPixels(pixel);
		texture.Apply();

		return texture; // return simple texture

	}
}
