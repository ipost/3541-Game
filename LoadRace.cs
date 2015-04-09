using UnityEngine;
using System.Collections;

public class LoadRace : MonoBehaviour {
	private string[] courseNames = {"Figure 8", "Another Course"};
	public void LoadScene (int course) {
		CourseSelector cs = GameObject.Find ("CourseSelectPanel").GetComponent<CourseSelector>();
		PlayerPrefs.SetInt ("courseSelection", cs.getCourseSelection());
		Application.LoadLevel (courseNames[cs.getCourseSelection()].Replace(" ",""));
	}
}
