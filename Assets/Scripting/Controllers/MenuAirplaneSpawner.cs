using System;
using UnityEngine;

public class MenuAirplaneSpawner : MonoBehaviour, IEventSubscriber
{
    public Transform AirplaneSpawnPoint;
    public GameObject F16_Prefab;
    public GameObject FA22_Prefab;
    public GameObject FA38_Prefab;
    public GameObject Mirage_Prefab;
    public GameObject SAAB_Prefab;

    private GameObject plane = null;

    void Start()
    {
        EventController.Instance.Subscribe("OnShowPlane", this);

        if (TransportGOController.Instance.SelectedPlane != Airplanes.None)
        {
            GameObject prefab = null;
            switch (TransportGOController.Instance.SelectedPlane)
            {
                case Airplanes.F_16:
                    prefab = F16_Prefab;
                    break;
                case Airplanes.FA_22:
                    prefab = FA22_Prefab;
                    break;
                case Airplanes.FA_38: 
                    prefab = FA38_Prefab;
                    break;
                case Airplanes.Mirage:
                    prefab = Mirage_Prefab;
                    break;
                case Airplanes.SAAB:
                    prefab = SAAB_Prefab;
                    break;
            }
            plane = GameObject.Instantiate(prefab, AirplaneSpawnPoint.position, AirplaneSpawnPoint.rotation) as GameObject;
        }
    }

    #region IEventSubscriber implementation

    public void OnEvent(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "OnShowPlane":
                if (TransportGOController.Instance.SelectedPlane != Airplanes.None)
                {
                    if (plane)
                        GameObject.Destroy(plane);

                    GameObject prefab = null;
                    switch (TransportGOController.Instance.SelectedPlane)
                    {
                        case Airplanes.F_16:
                            prefab = F16_Prefab;
                            break;
                        case Airplanes.FA_22:
                            prefab = FA22_Prefab;
                            break;
                        case Airplanes.FA_38:
                            prefab = FA38_Prefab;
                            break;
                        case Airplanes.Mirage:
                            prefab = Mirage_Prefab;
                            break;
                        case Airplanes.SAAB:
                            prefab = SAAB_Prefab;
                            break;
                    }
                    plane = GameObject.Instantiate(prefab, AirplaneSpawnPoint.position, AirplaneSpawnPoint.rotation) as GameObject;
                }
                break;
        }
    }

    #endregion
}
