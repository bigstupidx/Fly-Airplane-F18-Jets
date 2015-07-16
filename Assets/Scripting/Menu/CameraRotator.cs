using UnityEngine;
using System.Collections;

public class CameraRotator : MonoBehaviour {


	public Vector3 rotate;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (rotate * Time.deltaTime);
	}
}
