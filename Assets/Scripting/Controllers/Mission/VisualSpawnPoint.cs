using UnityEngine;
using System.Collections;

public class VisualSpawnPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.2f, 0.2f, 0.7f);
        Gizmos.DrawSphere(transform.position, 1.6f);
    }
}
