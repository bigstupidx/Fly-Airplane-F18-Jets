using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour
{
	void Update ()
	{
		transform.rotation = Quaternion.LookRotation(-AirplaneController.Instance.transform.position + transform.position);
	}
}
