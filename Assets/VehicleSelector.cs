using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VehicleSelector : MonoBehaviour {
	
	private int selection = 0;
	
	//TODO: figure out where this info should go
	private string[] vehicleNames = {"Orion", "Wraith"};
	private int vehicleCount = 2;
	
	Text vehicleNameDisplay;
	
	void Start() {
		vehicleNameDisplay = GameObject.Find ("VehicleNameDisplay").GetComponent<Text>();
		vehicleNameDisplay.text = vehicleNames [selection];
	}
	
	public void change(int direction) {
		selection = (selection + direction) % vehicleCount;
		if (selection < 0)
			selection += vehicleCount;
		vehicleNameDisplay.text = vehicleNames [selection];
	}
	
	public string getVehicleSelection() {
		return vehicleNames [selection];
	}
	public void exitGame() {
		Application.Quit ();
	}
}
