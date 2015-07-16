using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class MissionObjectData
{
    public int ID;
    public Vector3 Position;
    public string Name;
    public string Objective;

    public MissionObjectData()
    {
        ID = -1;
        Position = Vector3.zero;
        Name = "Unnamed";
        Objective = "Objective";
    }

    public MissionObjectData(MissionObjectData Original)
    {
        ID = Original.ID;
        Position = Original.Position;
        Name = Original.Name;
        Objective = Original.Objective;
    }
}

public class DataStorageController : Singleton<DataStorageController>
{
    public GameObject DeathPS;
    public GameObject PlaneDeathPS;
    public float ViewZoneDistance = 2000;

    public GameObject BaseDestroyPSPrefab;
    public GameObject RocketPrefab;

    public MissionObjectData[] MissionIslandsID;
    public MissionObjectData[] MissionRunwaysID;
    public MissionObjectData[] MissionTransportsID;
    public MissionObjectData[] MissionBasesID;

    public static MissionObjectData GetMissionObjectDataByID(int ID)
    {
        foreach (MissionObjectData d in Instance.MissionBasesID)
            if (d.ID == ID)
                return d;
        foreach (MissionObjectData d in Instance.MissionIslandsID)
            if (d.ID == ID)
                return d;
        foreach (MissionObjectData d in Instance.MissionRunwaysID)
            if (d.ID == ID)
                return d;
        foreach (MissionObjectData d in Instance.MissionTransportsID)
            if (d.ID == ID)
                return d;
        return null;
    }

    [ContextMenu("Collect Mission Data")]
    public void GetData()
    {
        List<GameObject> gosIslands = new List<GameObject>();
        List<GameObject> gosRunways = new List<GameObject>();
        List<GameObject> gosTransports = new List<GameObject>();
        List<GameObject> gosBases = new List<GameObject>();
        int currentID = 0;
        string[] tags = new string[3] {"Death","MissionObject","Runway"};
        foreach (string tag in tags)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag(tag))
            {
                MissionObject m = go.GetComponent<MissionObject>();
                if (m)
                {
                    m.ID = currentID;
                    currentID++;
                    switch (m.ObjectType)
                    {
                        case MissionObjectType.Island:
                            go.name = "Island ["+m.ID.ToString("000")+"]";
                            gosIslands.Add(go);
                            break;
                        case MissionObjectType.Base:
                            go.name = "Base ["+m.ID.ToString("000")+"]";
                            gosBases.Add(go);
                            break;
                        case MissionObjectType.Transport:
                            gosTransports.Add(go);
                            break;
                        case MissionObjectType.Runway:
                            go.name = "Runway ["+m.ID.ToString("000")+"]";
                            gosRunways.Add(go);
                            break;
                    }
                }
            }
        }
        MissionIslandsID = GetMissionObjectsData(gosIslands.ToArray());
        MissionBasesID = GetMissionObjectsData(gosBases.ToArray());
        MissionRunwaysID = GetMissionObjectsData(gosRunways.ToArray());
        MissionTransportsID = GetMissionObjectsData(gosTransports.ToArray());
    }

    private MissionObjectData[] GetMissionObjectsData(GameObject[] o)
    {
        MissionObjectData[] res = new MissionObjectData[o.Length];
        for (int i=0; i<o.Length; i++)
        {
            res [i] = new MissionObjectData();
            res[i].ID = o [i].GetComponent<MissionObject>().ID;
            res[i].Position = o[i].transform.position;
            res[i].Name = o[i].GetComponent<MissionObject>().Name;
        }
        return res;
    }

    public static MissionObject GetMissionObjectByID(int ID)
    {
        string[] tags = new string[3] {"Death","MissionObject","Runway"};
        foreach (string tag in tags)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag(tag))
            {
                MissionObject m = go.GetComponent<MissionObject>();
                if (m && m.ID == ID)
                        return m;
            }
        }
        return null;
    }

    [ContextMenu("Set random islands positions")]
    public void SetRandomIPositions()
    {
        foreach (MissionObjectData go in MissionIslandsID)
        {
            Vector2 pos = UnityEngine.Random.insideUnitCircle.normalized;
            float minDist = 5000;
            float maxDist = 20000;
            pos= pos*minDist + pos*UnityEngine.Random.value * (maxDist - minDist);
            MissionObject mo = GetMissionObjectByID(go.ID);
            print(string.Format("Island [{0}] set to {1} distance and {2} position",mo.name,
                                pos.magnitude,new Vector3(pos.x,0,pos.y)));

            mo.transform.position = new Vector3(pos.x,0,pos.y);
        }
    }
}
