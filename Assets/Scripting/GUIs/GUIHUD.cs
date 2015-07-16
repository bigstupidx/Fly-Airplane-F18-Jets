using UnityEngine;
using System.Collections;

public class GUIHUD : MonoBehaviour
{
    public TextMesh TextMesh;
	
	// Update is called once per frame
	void Update ()
	{
	    TextMesh.text = "KN: " + AirplaneController.Instance.CurrentSpeed.ToString("0.0") + " FT: " +
	                    ((int) (AirplaneController.Instance.Height));
	}
}
