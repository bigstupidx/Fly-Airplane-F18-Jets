using UnityEngine;
using System.Collections;

public class NavigationButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (OptionsController.Instance.Tilt)
	    {
	        if (GetComponent<Renderer>().enabled)
	        {
	            GetComponent<Renderer>().enabled = false;
	            GetComponent<Collider>().enabled = false;
	        }
	    }
	    else
	    {
	        if (!GetComponent<Renderer>().enabled)
	        {
	            GetComponent<Renderer>().enabled = true;
	            GetComponent<Collider>().enabled = true;
	        }
	    }
	}
}
