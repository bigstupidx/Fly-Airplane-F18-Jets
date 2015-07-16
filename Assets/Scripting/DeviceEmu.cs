using UnityEngine;
using System.Collections;

public class DeviceEmu : MonoBehaviour
{
    public static DeviceEmu Instance { get; private set; }
    public bool Gyroscope;
	// Use this for initialization
	void Start ()
	{
	    Instance = this;
	}

}
