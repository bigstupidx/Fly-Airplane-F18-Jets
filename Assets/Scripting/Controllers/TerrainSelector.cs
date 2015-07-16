using System.Linq;
using MilitaryDemo;
using UnityEngine;
using System.Collections;

public class TerrainSelector : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    int indexToActivate = RandomTool.NextInt(0, transform.childCount - 1);
        transform.GetChild(indexToActivate).gameObject.SetActive(true);
	}

}
