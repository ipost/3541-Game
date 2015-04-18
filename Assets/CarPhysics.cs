using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CarPhysics : MonoBehaviour {

	float mass = 1.0f;
	float thrustForce = 450.0f;
	float brakeForce = 1250.0f;
	float dampingPerSec = 0.2f;
	float hoverHeight = 0.05f;
	float hoverForce = 40000.0f;
	float thrustSpeedCap = 1200.0f;
	float brakeSpeedCap = -75.0f;
	float turnRate = 45f;
	float boostDuration = 1.5f;
	float boostForce = 300f;
	float remainingBoostDuration = 0.0f;
	float maxAccel = 10.0f;

	float dim;
	float velocityMagnificationFactor = 0.02f;
	float thrusterLifeTime = 0.04f;
	float thrusterBaseLifeTime = 0.01f;
	float thrusterParticleSize = 0.004f;
	float thrusterBoostParticleSize = 0.008f;
	float boostLifeTimeAddition = 0.03f;

	float rollRateModifier = 2.5f;
	float maxRollAngle = 55f;

	float fovDefault = 60f;
	float fovMax = 85f;
	float fovVariance;
	float fovSpeedThreshold = 200f;
	float fovSpeedMax = 1600f;

	Vector3 velocity;

	Transform vehicleModel;
	ParticleSystem[] thrusters;
	Vector3[] sensors;
	public Collider track;
	public GameObject course;
	public bool isAI;
	Transform[] checkpoints;
	Transform nextCheckpoint;
	int lastCheckpoint;
	int lap;
	float accel;

	Text lapDisplay;
	Text speedometer;

	void Start () {
		Transform spawn = TrackScript.getSpawn ();
		transform.position = spawn.position;
		transform.rotation = spawn.rotation;

		velocity = Vector3.zero;
		track = GameObject.Find ("course").GetComponent<Collider> ();

		dim = 0.5f;
		sensors = new Vector3[4];
		sensors[0] = new Vector3(dim, -dim, dim);
		sensors[1] = new Vector3(dim, -dim, -dim);
		sensors[2] = new Vector3(-dim, -dim, dim);
		sensors[3] = new Vector3(-dim, -dim, -dim);

		lap = 1;
		//Debug.Log ("Lap " + lap);
		lapDisplay = GameObject.Find ("LapDisplay").GetComponent<Text>();
		setLapDisplay ();
		lastCheckpoint = -1;
		Transform courseTrans = GameObject.Find ("course").transform;
		foreach (Transform child in courseTrans) {
			if (child.gameObject.name == "checkpoint list") {
				Transform checkpointParent = child;
				int children = checkpointParent.childCount;
				checkpoints = new Transform[children];
				for (int i = 0; i < children; i++) {
					checkpoints[i] = checkpointParent.GetChild(i);
				}
			}
		}
		nextCheckpoint = checkpoints [0];
		accel = 1.0f;

		speedometer = GameObject.Find ("Speedometer").GetComponent<Text>();

		thrusters = transform.GetComponentsInChildren<ParticleSystem>();

		vehicleModel = transform.FindChild ("VehicleModel").transform;
		fovVariance = fovMax - fovDefault;
	}

	void setLapDisplay() {
		lapDisplay.text = "Lap " + lap + " / 3";
	}

	void setSpeedometer() {
		speedometer.text = ((int) velocity.x) + " KM/h";
	}

	void updateThrusters() {
		foreach (ParticleSystem ps in thrusters) {
			ps.startLifetime = thrusterBaseLifeTime;
			if (Input.GetKey("w")) {
				ps.startLifetime += thrusterLifeTime * (velocity.x / thrustSpeedCap);
			}
			if (remainingBoostDuration > 0f) {
				float effectFactor = Mathf.Pow(remainingBoostDuration / boostDuration, 2);
				ps.startLifetime += effectFactor * boostLifeTimeAddition;
				ps.startSize = thrusterParticleSize + thrusterBoostParticleSize * effectFactor;

			}
		}
	}

	Transform getNextCheckpoint (int i) {
		int next = i + 1;
		if (next == checkpoints.Length) {
			next = 0;
		}
		return checkpoints [next];
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.name.Equals ("checkpoint")) {
			for (int i = 0; i < checkpoints.Length; i++) {
				if (checkpoints[i] == col.gameObject.transform) {
					if (lastCheckpoint == i - 1 || (lastCheckpoint == checkpoints.Length - 1 && i == 0)) {
						lastCheckpoint = i;
						nextCheckpoint = getNextCheckpoint (i);
						if (isAI) {
							Debug.Log ("AI hit checkpoint " + (i + 1) + " of " + checkpoints.Length);
						} else {
							Debug.Log ("You hit checkpoint " + (i + 1) + " of " + checkpoints.Length);
						}
						if (i == checkpoints.Length - 1) {
							lap++;
							if (isAI) {
								Debug.Log ("AI finished a lap");
							} else {
								Debug.Log ("You finished a lap");
							}
							setLapDisplay();
						}
					}
				}
			}
		}
	}

	Vector3 getUserForce () {
		Vector3 force = Vector3.zero;
		if (Input.GetKey ("w")) {
			float proportionOfMax = (thrustSpeedCap - velocity.x) / thrustSpeedCap;
			float appliedThrust = thrustForce * proportionOfMax;
			force += new Vector3(appliedThrust,0,0);
		} else if (Input.GetKey ("s") && velocity.x > brakeSpeedCap) {
			force -= new Vector3(brakeForce,0,0);
		} else if (velocity.x > 0){
			force -= new Vector3(thrustForce * .4f,0,0);
		} else {
			force += new Vector3(thrustForce * .2f,0,0);
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
			if (track.Raycast (myray, out hit, 10f * hoverHeight)) {
				sensorDistances[i] = hit.distance;
			} else {
				sensorDistances[i] = hoverHeight;
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
		transform.RotateAround (leftPoint, transform.TransformDirection(sensors[3] - sensors[1]), rotateAmount);
		
		Vector3 rightPoint = transform.TransformPoint(vector3Average(sensors[1],sensors[3]));
		heightDiff = ((sensorDistances [0] + sensorDistances [2]) / 2.0f) - (hoverHeight);
		rotateAmount = t * hoverForce * heightDiff;
		transform.RotateAround (rightPoint, transform.TransformDirection(sensors[0] - sensors[2]), rotateAmount);
		
	}

	void handleTurns() {
		float t = Time.deltaTime;
		float rollAmount = t * rollRateModifier;
		float zRot = vehicleModel.localEulerAngles.z;
		if (Input.GetKey ("d")) {
			rollAmount *= (zRot >= 180f) ? -(zRot - (360f - maxRollAngle)) : -(zRot + maxRollAngle);
			transform.Rotate(new Vector3(0,turnRate * t,0));
		} else if (Input.GetKey ("a")) {
			rollAmount *= (zRot >= 180f) ? ((maxRollAngle + 360f) - zRot) : (maxRollAngle - zRot);
			transform.Rotate(new Vector3(0,-turnRate * t,0));
		} else {
			rollAmount *= (zRot >= 180f) ? (360f - zRot) : (0f - zRot);
		}
		vehicleModel.Rotate(new Vector3(0,0,rollAmount));
	}

	void applyFoVSpeedEffect(){
		if (velocity.x > fovSpeedThreshold) {
			float v = velocity.x > fovSpeedMax ? fovSpeedMax : velocity.x;
			Camera.main.fieldOfView = fovDefault
				+ (fovVariance* Mathf.Pow((v - fovSpeedThreshold) / (fovSpeedMax - fovSpeedThreshold), 2));
		}
	}

	void Update () {
		float t = Time.deltaTime;

		Vector3 force = Vector3.zero;
		//force += new Vector3 (0,-3,0);
		if (isAI) {
			Vector3 target = Vector3.MoveTowards (transform.position, nextCheckpoint.position, accel * t);
			transform.position = target;
			Vector3 direction = (nextCheckpoint.position - transform.position).normalized;
			Quaternion rotation = Quaternion.LookRotation (direction);
			transform.rotation = Quaternion.Slerp (transform.rotation, rotation, t);
			if (accel < maxAccel) {
				accel += 0.01f;
			}
		} else {
			force += getUserForce ();
			handleTurns ();
		}

		if (remainingBoostDuration > 0f) {
			force += new Vector3(boostForce,0,0);
			remainingBoostDuration -= Time.deltaTime;
		} else if (Input.GetKeyDown ("space")) {
			remainingBoostDuration = boostDuration;
		}
		
		velocity += t * (force / mass);

		if (!isAI) {
			updateThrusters ();
			setSpeedometer ();
		}
		transform.Translate (t * velocityMagnificationFactor * velocity);
		//float start = Time.realtimeSinceStartup;
		applyHoverForce ();
		applyFoVSpeedEffect();
		//float stop = Time.realtimeSinceStartup;
		//Debug.Log ("AHF execution time: " + (stop - start).ToString("0.00000000"));
	}
}
