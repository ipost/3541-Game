using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {
	private string[] courseNames = {"Figure 8", "Another Course"};

	// Use this for initialization
	void Start () {
	
	}
	
	public void returnToMenu () {
		Application.LoadLevel ("menu");
		Time.timeScale = 1;
	}

	public void restartRace () {
		Application.LoadLevel (courseNames[PlayerPrefs.GetInt ("courseSelection")].Replace(" ",""));
		resumeRace ();
	}

	public void resumeRace(){
		transform.gameObject.SetActive(false);
		Time.timeScale = 1;
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
