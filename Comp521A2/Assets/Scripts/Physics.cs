using UnityEngine;
using System.Collections;

public class Physics : MonoBehaviour {

	public float gravity; // gravity at 9.8
	public float mass; // cannonball mass
	public float resistence; // wind resistence
	private Vector3 velocity; // cannonball velocity
	private Vector3 acceleration; // cannonball acceleration
	private Vector3[] vertices; // vertices from mountain mesh
	private Mesh mesh; // mountain mesh
	private bool collided; // whether cannonball hit initial collision

	// Use this for initialization
	void Start () {
	
		velocity = Vector3.zero; // initialize velocity
		acceleration = Vector3.zero; // initialize acceleration

		// set initial velocity of projectile based on angle and speed
		velocity.x = Shooter.speed/4 * Mathf.Cos(Shooter.angle * Mathf.Deg2Rad);
		velocity.y = Shooter.speed/4 * Mathf.Sin(Shooter.angle * Mathf.Deg2Rad);

		// Find collidable mountain and get vertices as well as mesh
		GameObject terrain = GameObject.Find("Terrain1");
		vertices = terrain.GetComponent<Builder>().newVertices;
		mesh = terrain.GetComponent<MeshFilter>().mesh;

		collided = false; // initialize initial collision

	}
	
	// Update is called once per frame
	void Update () {
	
		// assign gravity and wind resistence
		acceleration.y -= mass * gravity * Time.deltaTime;
		acceleration -= velocity * resistence * Time.deltaTime;

		// get wind index of perlin noise and remap to screen height
		int index = (Mathf.Clamp((int)((transform.position.y + 5.0f) * 10.0f), 0, 99) + (int)Wind.counter) % Wind.noise.Length;
		if(index < 0)
		{
			index += Wind.noise.Length; // loop the index out of range
		}

		// remap wind value to [-1;1] with -1 strongest right and 1 strongest left
		float wind = 2*Wind.noise[index] - 1;
		if(transform.position.y > 5.0f)
		{
			wind = 0.0f; // no wind outside of screen
		}

		// assign wind force and direction
		acceleration.x -= wind * 10.0f * Time.deltaTime;

		// compute velocity from acceleration
		velocity += acceleration * Time.deltaTime;

		// compute position from velocity
		transform.position += velocity * Time.deltaTime;

		// destroy cannonball when off-screen
		if(transform.position.y < -5.0f)
		{
			Destroy(gameObject);
		}

		// check for collision detection against all vertices of mesh
		for(int i = 0; i < vertices.Length; i++)
		{
			// check if distance between cannonball and some vertex is less than epsilon
			if(Vector3.Distance(vertices[i], transform.position - new Vector3(6.0f,0,0)) < 0.1f)
			{
				// check if already collided before
				if(!collided)
				{
					// deform closest vertex
					vertices[i] += velocity * 0.01f;

					// deform surrounding vertices for visuals
					for(int j = 0; j < 20; j++)
					{
						//check for boundaries
						if(i-j >= 0)
						{
							vertices[i-j] += velocity * 0.01f;
						}
						if(i+j < vertices.Length)
						{
							vertices[i+j] += velocity * 0.01f;
						}
					}

					// reassign mesh vertices and recompute mesh boundaries
					mesh.vertices = vertices;
					mesh.RecalculateBounds();

					collided = true; // cannonball has collided
				}
				else
				{
					// reset velocity and acceleration
					velocity = Vector3.zero;
					acceleration = Vector3.zero;

					// allow the cannonball to roll down the mountain
					velocity += new Vector3(vertices[i].x - vertices[0].x, 0, 0) * 10.0f * Time.deltaTime;
				}

				break; // get out of loop after one collision in time step
			}
		}

	}
}
