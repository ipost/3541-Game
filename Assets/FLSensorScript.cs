using UnityEngine;
using System.Collections;

public class FLSensorScript : MonoBehaviour {
	
	public float downDist;
	public float leftDist;
	public float leftAngle;
	public float frontDist;
	public float frontAngle;
	public float maxSenseDist;

	// Use this for initialization
	void Start () {
		downDist = 0;
		leftDist = 0;
		leftAngle = 0;
		frontDist = 0;
		frontAngle = 0;
		maxSenseDist = 20;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 down = transform.TransformDirection(Vector3.down);
		Vector3 left = transform.TransformDirection (Vector3.left);
		Vector3 forward = transform.TransformDirection (Vector3.forward);
		Ray downray = new Ray (transform.position, down);
		Ray leftray = new Ray (transform.position, left);
		Ray forwardray = new Ray (transform.position, forward);
		Debug.DrawRay (transform.position, down);
		Debug.DrawRay (transform.position, left);
		Debug.DrawRay (transform.position, forward);
		RaycastHit hit;
		if (Physics.Raycast (downray, out hit, maxSenseDist, 1 << 8)) {
			downDist = hit.distance;
		} else {
			downDist = maxSenseDist;
		}
		if (Physics.Raycast (leftray, out hit, maxSenseDist, 1 << 8)) {
			leftDist = hit.distance;
			leftAngle = Vector3.Angle (leftray.direction, hit.normal);
		} else {
			leftDist = maxSenseDist;
			leftAngle = 180;
		}
		if (Physics.Raycast (forwardray, out hit, maxSenseDist, 1 << 8)) {
			frontDist = hit.distance;
			frontAngle = Vector3.Angle (forwardray.direction, hit.normal);
		} else {
			frontDist = maxSenseDist;
			frontAngle = maxSenseDist;
		}
	}

}
