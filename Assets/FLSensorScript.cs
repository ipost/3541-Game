using UnityEngine;
using System.Collections;

public class FLSensorScript : MonoBehaviour {

	public float downDist;
	public float leftDist;

	// Use this for initialization
	void Start () {
		downDist = 0;
		leftDist = 0;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 down = transform.TransformDirection(Vector3.down);
		Vector3 left = transform.TransformDirection (Vector3.left);
		Ray downray = new Ray (transform.position, down);
		Ray leftray = new Ray (transform.position, left);
		Debug.DrawRay (transform.position, down);
		Debug.DrawRay (transform.position, left);
		RaycastHit hit;
		if (Physics.Raycast (downray, out hit, 20)) {
			downDist = hit.distance;
		}
		if (Physics.Raycast (leftray, out hit, 20)) {
			leftDist = hit.distance;
		}
	}

}
