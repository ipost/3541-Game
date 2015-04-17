﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

	float dim;
	float velocityMagnificationFactor = 0.02f;
	float thrusterLifeTime = 0.04f;
	float thrusterBaseLifeTime = 0.01f;
	float thrusterParticleSize = 0.004f;
	float thrusterBoostParticleSize = 0.008f;
	float boostLifeTimeAddition = 0.03f;
	Vector3 velocity;

	ParticleSystem[] thrusters;
	Vector3[] sensors;
	public Collider track;
	public GameObject course;
	public bool isAI;
	Transform[] checkpoints;
	int lastCheckpoint;
	int lap;

	Text lapDisplay;
	Text speedometer;

	void Start () {
		Transform spawn = TrackScript.getSpawn ();
		transform.position = spawn.position;
		transform.rotation = spawn.rotation;
		Debug.Log ("Spawned at: " + spawn.position + " --- " + isAI);

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

		speedometer = GameObject.Find ("Speedometer").GetComponent<Text>();

		thrusters = transform.GetComponentsInChildren<ParticleSystem>();
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

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.name == "checkpoint") {
			for (int i = 0; i < checkpoints.Length; i++) {
				if (checkpoints[i] == col.gameObject.transform) {
					if (isAI) {
						Debug.Log ("AI hit checkpoint " + (i + 1) + " of " + checkpoints.Length);
						Vector3 direction = (checkpoints[i + 1].position - transform.position).normalized;
						Quaternion rotation = Quaternion.LookRotation(direction);
						Debug.Log ("Next checkpoint: " + checkpoints[i + 1].gameObject.name);
						Debug.Log (rotation);
					}
					if (lastCheckpoint == i - 1 || (lastCheckpoint == checkpoints.Length - 1 && i == 0)) {
						lastCheckpoint = i;
						if (i == checkpoints.Length - 1) {
							lap++;
							setLapDisplay();
							//Debug.Log ("Lap " + lap);
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
			if (track.Raycast (myray, out hit, 1f)) {
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
	
	void Update () {
		float t = Time.deltaTime;

		Vector3 force = Vector3.zero;
		if (isAI) {
			int nextCheckpoint = (lastCheckpoint + 1) % checkpoints.Length;
			Transform checkpoint = checkpoints[nextCheckpoint];
			transform.position = Vector3.MoveTowards (transform.position, checkpoint.position, t);
			force += new Vector3(thrustForce * .01f,0,0);
		} else {
			force += getUserForce ();
			if (Input.GetKey ("d")) {
				transform.Rotate(new Vector3(0,turnRate * t,0));
			} else if (Input.GetKey ("a")) {
				transform.Rotate(new Vector3(0,-turnRate * t,0));
			}
		}
		
		if (remainingBoostDuration > 0f) {
			force += new Vector3(boostForce,0,0);
			remainingBoostDuration -= Time.deltaTime;
		} else if (Input.GetKeyDown ("space") && !isAI) {
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
		//float stop = Time.realtimeSinceStartup;
		//Debug.Log ("AHF execution time: " + (stop - start).ToString("0.00000000"));
	}
}
