using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelsStorage : MonoBehaviour
{
    public static LevelsStorage Instance;
    public Transform Water;
    public List<BaseLevel> Levels;

	// Use this for initialization
	void Awake () {


        int id = TransportGOController.Instance.SelectedMissionID;

	    SpawnController.AirplaneSpawnPoint = Levels[id].TakeOffPos;
	    Water.position = new Vector3( SpawnController.AirplaneSpawnPoint.position.x, 0,SpawnController.AirplaneSpawnPoint.position.z) ;
        Levels[id].gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
