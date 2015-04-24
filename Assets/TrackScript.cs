using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackScript : MonoBehaviour {
	
	private string[] courseNames = {"Figure 8", "Another Course"};
	private static Queue<Transform> spawns;
	private int numAI = 2;

	// Use this for initialization
	void Start () {
		int selection = PlayerPrefs.GetInt ("courseSelection");
		string courseName = courseNames [selection].Replace (" ", "");

		Application.LoadLevelAdditive ("playerVehicle");
		for (int i = 0; i < numAI; i++) {
			Application.LoadLevelAdditive ("aiVehicle");
		}
		Application.LoadLevelAdditive ("HUD");

		//queue up spawns for the vehicles to fetch
		spawns = new Queue<Transform> ();
		foreach (Transform t in GameObject.Find ("VehicleSpawns").transform)
						spawns.Enqueue (t.transform);
	}

	public static Transform getSpawn() {
		return spawns.Dequeue ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
