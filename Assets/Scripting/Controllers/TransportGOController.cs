using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class AirplaneInfo
{
    public Airplanes ID;
    public string Name;
    public string FullName;
    public int Speed;
    public int Control;
    public int Acceleration;
    public int Stars;
    public bool Locked;
    public bool Buyout;
    public int BuyBack;
}

[Serializable]
public class MissionInfo
{
    public string MissionTitle;
    public string MissionText;
    public int Distance;
    public int Payment;
    public bool Blocked;
    public MissionObjectData[] Targets;
}

public class TransportGOController : Singleton<TransportGOController>
{
	void Start () 
    {
		DontDestroyOnLoad(this);
	}

    public Airplanes SelectedPlane;
    public AirplaneInfo[] PlanesInfo;
    public int SelectedMissionID;
    public MissionInfo[] Missions;

    public static AirplaneInfo GetPlaneInfo(Airplanes ID)
    {
        foreach (AirplaneInfo i in Instance.PlanesInfo)
            if (i.ID == ID)
                return i;
        return null;
    }
}
