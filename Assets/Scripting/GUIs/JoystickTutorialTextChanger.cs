using UnityEngine;
public class JoystickTutorialTextChanger : MonoBehaviour 
{
	void Update () 
    {
	    if (OptionsController.Instance.Tilt)
	    {
	        GetComponent<TextMesh>().text = "Tilt device to lift off";
	    }
	    else
	    {
	        GetComponent<TextMesh>().text = "Pull back to take off";
	    }
	}
}
