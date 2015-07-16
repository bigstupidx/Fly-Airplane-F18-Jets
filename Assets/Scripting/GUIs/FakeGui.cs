using UnityEngine;
using System.Collections;

public class FakeGui : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start ()
	{
	    yield return new WaitForEndOfFrame();
        Destroy(gameObject);
	}
}
