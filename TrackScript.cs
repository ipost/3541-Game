using UnityEngine;
using System.Collections;

public class TrackScript : MonoBehaviour {
	
	private string[] courseNames = {"Figure 8", "Another Course"};

	// Use this for initialization
	void Start () {
		//int selection = PlayerPrefs.GetInt ("courseSelection");
		int selection = 0;
		string courseName = courseNames [selection].Replace (" ", "");

		string floorPath = "Courses/" + courseName + "floor";
		MeshFilter courseFloorMesh = ((GameObject)Resources.Load (floorPath)).GetComponent<MeshFilter> ();
		GetComponent<MeshFilter>().mesh = courseFloorMesh.mesh;
		GetComponent<MeshCollider>().sharedMesh = courseFloorMesh.mesh;

		string wallsPath = "Courses/" + courseName + "walls";
		MeshFilter courseWallsMesh = ((GameObject)Resources.Load (wallsPath)).GetComponent<MeshFilter> ();
		GameObject walls = GameObject.Find ("walls");
		walls.GetComponent<MeshFilter>().mesh = courseWallsMesh.mesh;
		walls.GetComponent<MeshCollider>().sharedMesh = courseWallsMesh.mesh;

		Application.LoadLevelAdditive ("playerVehicle");
	}

	// Update is called once per frame
	void Update () {
	
	}
}
