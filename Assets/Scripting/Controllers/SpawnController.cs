using UnityEngine;
using System.Collections;

public enum Airplanes
{
    None,
    F_16,
    FA_22,
    FA_38,
    Mirage,
    SAAB
}

public class SpawnController : MonoBehaviour
{
    public SpawnController Instance;

    public Airplanes SpawnAirplane;
    public static Transform AirplaneSpawnPoint;

    public GameObject F16_Prefab;
    public GameObject FA22_Prefab;
    public GameObject FA38_Prefab;
    public GameObject Mirage_Prefab;
    public GameObject SAAB_Prefab;
    public GameObject[] Cabin;

//    private void Awake()
//    {
//        Instance = this;
//    }

    void Awake()
    {
        if (TransportGOController.Instance &&
            TransportGOController.Instance.SelectedPlane != Airplanes.None)
            SpawnAirplane = TransportGOController.Instance.SelectedPlane;

		int cab = 0;

        GameObject prefab = null;
        switch (SpawnAirplane)
        {
            case Airplanes.F_16:
                prefab = F16_Prefab;
				cab = 0;
				break;
            case Airplanes.FA_22:
                prefab = FA22_Prefab;
				cab = 1;
				break;
            case Airplanes.FA_38:
                prefab = FA38_Prefab;
				cab = 2;
				break;
            case Airplanes.Mirage:
                prefab = Mirage_Prefab;
                break;
            case Airplanes.SAAB:
                prefab = SAAB_Prefab;
                break;
        }

        prefab = GameObject.Instantiate(prefab, AirplaneSpawnPoint.position, AirplaneSpawnPoint.localRotation) as GameObject;
        CameraController.Instance.Target = prefab.transform;

		var cabin = GameObject.Instantiate(Cabin[cab]) as GameObject;
		cabin.transform.parent = prefab.transform;
        cabin.transform.position = prefab.GetComponent<AirplaneDriver>().InsideViewPosition.position;
        cabin.transform.localRotation = prefab.GetComponent<AirplaneDriver>().InsideViewPosition.localRotation;
        prefab.GetComponent<AirplaneDriver>().InsideView = cabin.transform;

        GameObject.Destroy(this);
    }
}
