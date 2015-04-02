using UnityEngine;
using System.Collections;

public class TrackScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("Entered the collider!");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
