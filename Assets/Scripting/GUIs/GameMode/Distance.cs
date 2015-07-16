using UnityEngine;
using System.Collections;

public class Distance : MonoBehaviour 
{

    public TextMesh DistText;

	void Update () 
    {
        DistText.text = "Distance: " + ((int)Radar.Instance.DistanceToTarget).ToString() + " m";	
	}
}
