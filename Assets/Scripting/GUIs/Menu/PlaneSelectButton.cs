using UnityEngine;
using System.Collections;

public class PlaneSelectButton : Button
{
    public Airplanes SelectAirplane;
    public bool Selected = false;
    private bool _locket = false;
    private bool _purchase = false;
    private int _starsCount = 1;
    private string _planeName = "";
    public GameObject StarPrefab;
    public GameObject LockPrefab;
    public GameObject BuyoutPrefab;
    public GameObject PlaneNamePrefab;

    protected override void AwakeProc()
    {
        base.AwakeProc();
        SubscrabeOnEvents.Add("OnShowAirplaneSelecting");
        SubscrabeOnEvents.Add("OnUnactivatePlane");
    }
    
    protected override void EventProc(string EventName, GameObject Sender)
    {
//        for (int i = 0; i < TransportGOController.Instance.PlanesInfo.Length; i++)
//        {
//            {
//                TransportGOController.Instance.PlanesInfo[i].Locked = true;
//                TransportGOController.Instance.PlanesInfo[i].Buyout = true;
//            }
//        }
//        TransportGOController.Instance.PlanesInfo[0].Locked = false;
//        TransportGOController.Instance.PlanesInfo[0].Buyout = false;
//       EventController.Instance.PostEvent("OnSaveData", null);
        //base.EventProc(EventName, Sender);
        switch (EventName)
        {
            case "OnShowAirplaneSelecting":

			TransportGOController.Instance.SelectedPlane = Airplanes.F_16;
                Selected = TransportGOController.Instance.SelectedPlane == SelectAirplane;

                for (int i=0;i<transform.childCount;i++)
                    Destroy(transform.GetChild(i).gameObject);

                if (!Selected)
                    GetComponent<Renderer>().material.SetTexture("_MainTex", MainTexture);
                else
                {
                    GetComponent<Renderer>().material.SetTexture("_MainTex", ActiveTexture);
                    EventController.Instance.PostEvent("OnShowBuyPlanePopup",gameObject);
                }
                
                if (SelectAirplane == Airplanes.None)
                {
                    _locket = true;
                } else
                {
                    AirplaneInfo info = TransportGOController.GetPlaneInfo(SelectAirplane);
                    this._starsCount = 5;//info.Stars;
                    this._locket = info.Locked;
                    this._purchase = info.Buyout;
                    this._planeName = info.Name;
                }
                
                if (_locket)
                {
                    GameObject prefab;
                    if (_purchase)
                        prefab = BuyoutPrefab;
                    else 
                        prefab = LockPrefab;
                    prefab = GameObject.Instantiate(prefab) as GameObject;
                    prefab.transform.parent = transform;
                    prefab.transform.localPosition = new Vector3(0,0,-1);
                    prefab.transform.localScale *=2;
                }
                for (int i=0; i<_starsCount; i++)
                {
                    GameObject prefab = GameObject.Instantiate(StarPrefab) as GameObject;
                    prefab.transform.parent = transform;
                    prefab.transform.localPosition = new Vector3(-0.52f, 0.35f - i * 0.17f, -1);
                    prefab.transform.localScale = new Vector3(0.13f,0.13f,1);
                }
                if (Selected)
                    TransportGOController.Instance.SelectedPlane = SelectAirplane;
                if (this._planeName != "")
                {
                    GameObject prefab = GameObject.Instantiate(PlaneNamePrefab) as GameObject;
                    prefab.transform.parent = transform;
                    prefab.transform.localPosition = new Vector3(-0.33f, 0.76f, -1);
                    prefab.transform.localScale = new Vector3(0.05f,0.05f,1);
                    prefab.GetComponent<TextMesh>().text = this._planeName;
                }

                GetComponent<Renderer>().enabled =true;
                GetComponent<Collider>().enabled = true;
                break;
                
            case "OnReleaseObject":
                if (Sender == gameObject)
                {
                    TransportGOController.Instance.SelectedPlane = SelectAirplane;
                }
                break;

            case "OnPressObject":
                if (Sender == gameObject && !Selected)
                {
                    EventController.Instance.PostEvent("OnUnactivatePlane",null);
                    GetComponent<Renderer>().material.SetTexture("_MainTex", ActiveTexture);
                    TransportGOController.Instance.SelectedPlane = SelectAirplane;
                    EventController.Instance.PostEvent("OnShowPlane",null);
                    Selected = true;
                    GetComponent<Renderer>().material.SetTexture("_MainTex", ActiveTexture);

                    EventController.Instance.PostEvent("OnShowBuyPlanePopup", gameObject);
                    EventController.Instance.PostEvent("OnPlayButtonPress",null);

                }
                break;

            case "OnUnactivatePlane":
                Selected = false;
                GetComponent<Renderer>().material.SetTexture("_MainTex", MainTexture);
                break;
        }
    }
}
