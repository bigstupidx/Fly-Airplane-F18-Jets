using UnityEngine;
using System.Collections;

public class WayPoint : MonoBehaviour
{
    public float Radius = 2000;
    public string Text;
    public bool DeadlyZoneOutside = false;
    public float SafeZoneAround = 7000;

    private void Update()
    {
		// Rotate towards plane
        //transform.rotation = Quaternion.LookRotation(-AirplaneController.Instance.transform.position + transform.position);
    }
}
