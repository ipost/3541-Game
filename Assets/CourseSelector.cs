using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CourseSelector : MonoBehaviour {

	private int selection = 0;

	//TODO: figure out where this info should go
	private string[] courseNames = {"Figure 8", "Cyclone"};
	private int levelCount = 2;

	Text courseNameDisplay;

	void Start() {
		courseNameDisplay = GameObject.Find ("CourseNameDisplay").GetComponent<Text>();
		courseNameDisplay.text = courseNames [selection];
	}

	public void change(int direction) {
		selection = (selection + direction) % levelCount;
		if (selection < 0)
						selection += levelCount;
		courseNameDisplay.text = courseNames [selection];
	}

	public int getCourseSelection() {
		return selection;
	}
	public void exitGame() {
		Application.Quit ();
	}
}
