using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour {

	public static int speed; // cannon speed
	public static float angle; // cannon angle
	public GUIText speedAngleText; // GUI text for cannon
	public GameObject projectile; // cannonball
	public float fireRate; // fire rate of cannon
	private float nextFire; // time before next fire

	// Use this for initialization
	void Start () {
	
		speed = 50; // initialize speed
		nextFire = 0.0f; // initialize time before next fire

	}
	
	// Update is called once per frame
	void Update () {
	
		float delta = 0.5f; // amount of rotation

		// rotate cannon on arrow input
		transform.Rotate(Vector3.forward, Input.GetAxis("Vertical") * delta);

		// make sure rotation does not proceed outside of quadrant
		transform.eulerAngles = new Vector3(0.0f,0.0f, Mathf.Clamp(transform.rotation.eulerAngles.z, 270.0f, 359.0f));

		// increase/decrease speed on arrow input
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			speed--;
		}
		else if(Input.GetKey(KeyCode.RightArrow))
		{
			speed++;
		}

		// set limits for speed
		speed = Mathf.Clamp(speed, 0, 100);

		// convert angle between 0 and 90
		angle = transform.rotation.eulerAngles.z - 270.0f;

		// update GUI text
		speedAngleText.text = "Cannon:\n    Speed: " + speed.ToString() + "\n    Angle: " + (Mathf.Round(angle)).ToString();

		// fire cannonball if time before next fire has passed
		if(Input.GetKeyDown(KeyCode.Space) && Time.time > nextFire)
		{
			nextFire = Time.time + fireRate; // increase time before next fire
			Instantiate(projectile, transform.position, Quaternion.identity); // instantiate projectile
		}

	}
}
