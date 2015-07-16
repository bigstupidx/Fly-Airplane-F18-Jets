using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{
    public TextMesh Speed;
    public TextMesh Height;
    public Transform Rotator;
    public Transform Center;
    public Transform Top;
    public Transform Bottom;

    public Radar Radar;

	public Color green;
	public Color blue;
	public Color yellow;

	void Start ()
	{
		// selected colours
		TextMesh[] textcolors = GetComponentsInChildren<TextMesh>();
		MeshRenderer[] meshcolors = GetComponentsInChildren<MeshRenderer>();

		switch (PlayerPrefs.GetInt("HUDcolor"))
		{
		case 0:
				foreach (TextMesh textcolor in textcolors)
					textcolor.color = green;
				foreach (MeshRenderer meshcolor in meshcolors)
					meshcolor.material.SetColor("_TintColor",green);
			break;
		case 1:
				foreach (TextMesh textcolor in textcolors)
					textcolor.color = blue;
				foreach (MeshRenderer meshcolor in meshcolors)
					meshcolor.material.SetColor("_TintColor",blue);
			break;
		case 2:
				foreach (TextMesh textcolor in textcolors)
					textcolor.color = yellow;
				foreach (MeshRenderer meshcolor in meshcolors)
					meshcolor.material.SetColor("_TintColor",yellow);
			break;
		}
	}
	
	void LateUpdate ()
	{
	    Speed.text = AirplaneController.Instance.CurrentSpeed.ToString("0.0");
	    Height.text = ((int)(AirplaneController.Instance.Height)).ToString();

        var rotation = Center.rotation.eulerAngles;
	    rotation.z = -transform.rotation.z*0.5f;
        Center.eulerAngles = rotation;


	    if (rotation.x < 360 && rotation.x > 260)
	    {
	        rotation.x = rotation.x - 360;

	    }
        else
	    if (rotation.x > 20)
	    {
	        rotation.x = 20;

	    }

        float ceof = ((rotation.x + 20f)/40);

        var pos = Center.localPosition;

	    pos.y = Mathf.Lerp(Top.localPosition.y, Bottom.localPosition.y, 1f - ceof)*1f;

        Center.localPosition = pos;
	}


}
