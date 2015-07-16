using UnityEngine;
using System.Collections;

public class NextMissionHider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (MissionController.Instance.Failed)
	    {
	        GetComponent<Renderer>().enabled = false;   
	    }

    }
}
