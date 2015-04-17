using UnityEngine;
using System.Collections;

public class SensorScript : MonoBehaviour {

	public bool isColliding;
	public float distance;
	public Collider track;
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void FixedUpdate () {
		Vector3 down = transform.TransformDirection(Vector3.down);
		Ray myray = new Ray (transform.position, down);
		RaycastHit hit;
		if (track.Raycast (myray, out hit, Mathf.Infinity)) {

			isColliding = true;
			distance = hit.distance;

			if (distance < 1) {
				//Debug.Log ("There is something underneath the object!");
			}
		} else {
			isColliding = false;
			distance = 50000;
		}
	}
}
