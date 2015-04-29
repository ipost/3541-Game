using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadRace : MonoBehaviour {
	private string[] courseNames = {"Figure 8", "Cyclone"};
	public void LoadScene (int course) {
		Slider slider = GameObject.Find ("Slider").GetComponent<Slider>();
		PlayerPrefs.SetInt ("numAI", (int)slider.value);
		CourseSelector cs = GameObject.Find ("CourseSelectPanel").GetComponent<CourseSelector>();
		PlayerPrefs.SetInt ("courseSelection", cs.getCourseSelection());
		Application.LoadLevel (courseNames[cs.getCourseSelection()].Replace(" ",""));
	}
}
