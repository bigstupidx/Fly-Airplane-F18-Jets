using UnityEngine;
using System.Collections;

public class loadlevel : MonoBehaviour
{
	float delay = 1.2f;

	void Start ()
	{
		if (!AdMob_Manager.Instance.IntIsReady)
			Application.LoadLevel("main");
	}

	void Update ()
	{
		delay -= Time.deltaTime;

		if (delay < 0)
			Application.LoadLevel("main");
	}
}
