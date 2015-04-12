using UnityEngine;
using System.Collections;

public class TrackScript : MonoBehaviour {
	
	private string[] courseNames = {"Figure 8", "Another Course"};

	// Use this for initialization
	void Start () {
		int selection = PlayerPrefs.GetInt ("courseSelection");
		string courseName = courseNames [selection].Replace (" ", "");

		Application.LoadLevelAdditive ("playerVehicle");
	}

	// Update is called once per frame
	void Update () {
	
	}
}
