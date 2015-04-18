using UnityEngine;
using System.Collections;

public class FRSensorScript : MonoBehaviour {
	public float downDist;
	public float rightDist;
	
	// Use this for initialization
	void Start () {
		downDist = 0;
		rightDist = 0;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 down = transform.TransformDirection(Vector3.down);
		Vector3 right = transform.TransformDirection (Vector3.right);
		Ray downray = new Ray (transform.position, down);
		Ray rightray = new Ray (transform.position, right);
		Debug.DrawRay (transform.position, down);
		Debug.DrawRay (transform.position, right);
		RaycastHit hit;
		if (Physics.Raycast (downray, out hit, 20)) {
			downDist = hit.distance;
		}
		if (Physics.Raycast (rightray, out hit, 20)) {
			rightDist = hit.distance;
		}
	}

}
