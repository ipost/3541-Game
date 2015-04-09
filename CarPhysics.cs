using UnityEngine;
using System.Collections;

public class CarPhysics : MonoBehaviour {

	float mass = 1.0f;
	float thrustForce = 7.0f;
	float brakeForce = 10.0f;
	float dampingPerSec = 0.2f;
	float hoverHeight = 0.05f;
	float hoverForce = 80000.0f;
	float thrustSpeedCap = 50.0f;
	float turnRate = 45f;
	float dim;
	Vector3 velocity;

	Vector3[] sensors;
	public Collider track;
	Vector3[,] courseTriangles;
	Vector3[,] courseTrianglesGlobal;
	public GameObject course;

	void Start () {
		velocity = Vector3.zero;
		track = GameObject.Find ("course").GetComponent<Collider> ();
		dim = 0.5f;
		sensors = new Vector3[4];
		sensors[0] = new Vector3(dim, -dim, dim);
		sensors[1] = new Vector3(dim, -dim, -dim);
		sensors[2] = new Vector3(-dim, -dim, dim);
		sensors[3] = new Vector3(-dim, -dim, -dim);
	}

	Vector3 getUserForce () {
		Vector3 force = Vector3.zero;
		if (Input.GetKey ("w")) {
			force += new Vector3(thrustForce,0,0);
		} else if (Input.GetKey ("s")) {
			force -= new Vector3(brakeForce,0,0);
		}
		return force;
	}

	Vector3 vector3Average (params Vector3[] vectors) {
		float xavg = 0, yavg = 0, zavg = 0, vCount = (float) vectors.Length;
		for (int i = 0; i < vectors.Length; i++) {
			xavg += (vectors[i].x / vCount);
			yavg += (vectors[i].y / vCount);
			zavg += (vectors[i].z / vCount);
		}
		return new Vector3 (xavg, yavg, zavg);
	}

	//		0   fp  1
	//	^	
	//	^	lp     rip
	//  ^
	//		2  rep  3
	void applyHoverForce(){
		float t = Time.deltaTime;
		
		Vector3 down = transform.TransformDirection(Vector3.down);
		float[] sensorDistances = new float[4];
		for (int i = 0; i < 4; i++) {
			Ray myray = new Ray (transform.TransformPoint(sensors[i]), down);
			RaycastHit hit;
			if (track.Raycast (myray, out hit, 3.0f)) {
				sensorDistances[i] = hit.distance;
			}
		}

		//TODO: DRY this up
		Vector3 frontPoint = transform.TransformPoint(vector3Average(sensors[0],sensors[1]));
		float heightDiff = ((sensorDistances [2] + sensorDistances [3]) / 2.0f) - (hoverHeight);
		float rotateAmount = t * hoverForce * heightDiff;
		transform.RotateAround (frontPoint, transform.TransformDirection(sensors[0] - sensors[1]), rotateAmount);
		
		Vector3 rearPoint = transform.TransformPoint(vector3Average(sensors[2],sensors[3]));
		heightDiff = ((sensorDistances [0] + sensorDistances [1]) / 2.0f) - (hoverHeight);
		rotateAmount = t * hoverForce * heightDiff;
		transform.RotateAround (rearPoint, transform.TransformDirection(sensors[3] - sensors[2]), rotateAmount);
		
		Vector3 leftPoint = transform.TransformPoint(vector3Average(sensors[0],sensors[2]));
		heightDiff = ((sensorDistances [1] + sensorDistances [3]) / 2.0f) - (hoverHeight);
		rotateAmount = t * hoverForce * heightDiff;
		transform.RotateAround (frontPoint, transform.TransformDirection(sensors[3] - sensors[1]), rotateAmount);
		
		Vector3 rightPoint = transform.TransformPoint(vector3Average(sensors[1],sensors[3]));
		heightDiff = ((sensorDistances [0] + sensorDistances [2]) / 2.0f) - (hoverHeight);
		rotateAmount = t * hoverForce * heightDiff;
		transform.RotateAround (rearPoint, transform.TransformDirection(sensors[0] - sensors[2]), rotateAmount);
	}
	
	void Update () {
		float t = Time.deltaTime;

		Vector3 force = Vector3.zero;
		force += getUserForce ();
		force += new Vector3 (0, -50, 0);
		if (Input.GetKey ("d")) {
			transform.Rotate(new Vector3(0,turnRate * t,0));
		} else if (Input.GetKey ("a")) {
			transform.Rotate(new Vector3(0,-turnRate * t,0));
		}
		
		velocity = new Vector3 (velocity.x, velocity.y * 0.1f, velocity.z);
		velocity = velocity + t * (force / mass);
		//velocity = velocity * (1.0f - (dampingPerSec * t));
		
		//		if (isAtGround ()) {
		//			velocity = new Vector3(velocity.x, 0.5f, velocity.z);
		//		}
		
		//		applyHoverForce ();
		transform.Translate (t * velocity);

		float start = Time.realtimeSinceStartup;
		applyHoverForce ();
		float stop = Time.realtimeSinceStartup;
		//Debug.Log ("AHF execution time: " + (stop - start).ToString("0.00000000"));
	}
}
