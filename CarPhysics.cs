using UnityEngine;
using System.Collections;

public class CarPhysics : MonoBehaviour {

	float mass = 1.0f;
	float thrustForce = 7.0f;
	float dampingPerSec = 0.2f;
	float hoverHeight = 3f;
	float hoverForce = 2500.0f;
	float thrustSpeedCap = 50.0f;
	float turnRate = 45f;
	Vector3 velocity;

	Vector3[,] courseTriangles;
	Vector3[,] courseTrianglesGlobal;
	GameObject course;

	void Start () {
		velocity = Vector3.zero;
		course = GameObject.Find ("course");
		Mesh courseMesh = course.GetComponent<MeshFilter> ().mesh;
		int[] meshTriangles = courseMesh.triangles;
		courseTriangles = new Vector3[meshTriangles.Length / 3, 3];
		courseTrianglesGlobal = new Vector3[meshTriangles.Length / 3, 3];
		for (int i = 0; i < meshTriangles.Length / 3; i++) {
			courseTriangles[i, 0] = courseMesh.vertices[meshTriangles[3*i]];
			courseTriangles[i, 1] = courseMesh.vertices[meshTriangles[3*i + 1]];
			courseTriangles[i, 2] = courseMesh.vertices[meshTriangles[3*i + 2]];
			courseTrianglesGlobal[i, 0] = course.transform.TransformPoint(courseTriangles[i, 0]);
			courseTrianglesGlobal[i, 1] = course.transform.TransformPoint(courseTriangles[i, 1]);
			courseTrianglesGlobal[i, 2] = course.transform.TransformPoint(courseTriangles[i, 2]);
		}
	}

	Vector3 getUserForce () {
		Vector3 force = Vector3.zero;
		if (Input.GetKey ("w")) {
			force += new Vector3(0,0,thrustForce);
		} else if (Input.GetKey ("s")) {
			force -= new Vector3(0,0,thrustForce);
		}
		return force;
	}

	float getHoverForce(Vector3 carPosition) {
		int tIndex = findNearestTriangle (carPosition);
		Vector3 triangleNormal = getTriangleNormal (tIndex).normalized;
		Vector3 projectedPoint = carPosition
			- (Vector3.Dot((carPosition - courseTrianglesGlobal[tIndex,0]), triangleNormal)
			   * triangleNormal);
		float distanceFromTriangle = Vector3.Distance (projectedPoint, carPosition);
		Plane p = new Plane (courseTrianglesGlobal[tIndex,0],courseTrianglesGlobal[tIndex,1],courseTrianglesGlobal[tIndex,2]);
		float sign = 1.0f;
		if (p.GetSide (carPosition)) {
			sign = -1.0f;
		}
		return hoverForce * distanceFromTriangle * sign;
	}

	int findNearestTriangle(Vector3 position){
		int closestTriangle = -1;
		float closestTriangleDistance = float.MaxValue;
		for (int i = 0; i < courseTriangles.GetLength(0); i++) {
			Vector3 triangleCenter = vector3Average(
				courseTrianglesGlobal[i,0],courseTrianglesGlobal[i,1],courseTrianglesGlobal[i,2]);
			float distance = Vector3.Distance(position, triangleCenter);
			if (distance < closestTriangleDistance) {
				closestTriangle = i;
				closestTriangleDistance = distance;
			}
		}
		return closestTriangle;
	}

	Vector3 getTriangleNormal (int triangleIndex) {
		return Vector3.Cross (
			courseTrianglesGlobal[triangleIndex,1] - courseTrianglesGlobal[triangleIndex,0],
			courseTrianglesGlobal[triangleIndex,2] - courseTrianglesGlobal[triangleIndex,0]);
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
	
	// Update is called once per frame
	void Update () {
		float t = Time.deltaTime;

		Vector3 force = Vector3.zero;
		Vector3 carPosition = transform.TransformPoint(new Vector3(0,-hoverHeight,0));
		force += getUserForce ();
		force += new Vector3(0, getHoverForce(carPosition), 0);

		if (Input.GetKey ("d")) {
			transform.Rotate(new Vector3(0,turnRate * t,0));
		} else if (Input.GetKey ("a")) {
			transform.Rotate(new Vector3(0,-turnRate * t,0));
		}

		velocity = new Vector3 (velocity.x, velocity.y * 0.1f, velocity.z);
		velocity = velocity + t * (force / mass);
		//velocity = velocity * (1.0f - (dampingPerSec * t));
		transform.Translate (t * velocity);
	}
}
