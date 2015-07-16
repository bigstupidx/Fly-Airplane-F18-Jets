using UnityEngine;
using System.Collections;

public class GyroText : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    GetComponent<GUIText>().text = "Orientation: " + Input.gyro.attitude.eulerAngles;
	}
}
