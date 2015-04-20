using UnityEngine;
using System.Collections;

public class FRSensorScript : MonoBehaviour {
	public float downDist;
	public float rightDist;
	public float rightAngle;
	public float frontDist;
	public float frontAngle;
	
	// Use this for initialization
	void Start () {
		downDist = 0;
		rightDist = 0;
		rightAngle = 0;
		frontDist = 0;
		frontAngle = 0;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 down = transform.TransformDirection(Vector3.down);
		Vector3 right = transform.TransformDirection (Vector3.right);
		Vector3 forward = transform.TransformDirection (Vector3.forward);
		Ray downray = new Ray (transform.position, down);
		Ray rightray = new Ray (transform.position, right);
		Ray forwardray = new Ray (transform.position, forward);
		Debug.DrawRay (transform.position, down);
		Debug.DrawRay (transform.position, right);
		Debug.DrawRay (transform.position, forward);
		RaycastHit hit;
		if (Physics.Raycast (downray, out hit, 20, 1 << 8)) {
			downDist = hit.distance;
		}
		if (Physics.Raycast (rightray, out hit, 20, 1 << 8)) {
			rightDist = hit.distance;
			rightAngle = Vector3.Angle(rightray.direction, hit.normal);
		}
		if (Physics.Raycast (forwardray, out hit, 20, 1 << 8)) {
			frontDist = hit.distance;
			frontAngle = Vector3.Angle(forwardray.direction, hit.normal);
		}
	}

}
