using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirplaneStatsPlane : GUIObject 
{
    public TextMesh AirplaneName;
    public GameObject ActiveRectPrefab;
    public GameObject UnactiveRectPrefab;

    private List<GameObject> _statsRects;


    public GameObject Back;
    protected override void AwakeProc()
    {
        base.AwakeProc();
        SubscrabeOnEvents.Add("OnShowPlane");
    }

    void Start()
    {
        _statsRects = new List<GameObject>();
        this.EventProc("OnShowPlane", null);
    }
    
    protected override void EventProc(string EventName, GameObject Sender)
    {
        base.EventProc(EventName, Sender);
        if (EventName == "OnShowPlane")
        {
            if (TransportGOController.Instance.SelectedPlane != Airplanes.None)
            {
                AirplaneInfo info = TransportGOController.GetPlaneInfo(TransportGOController.Instance.SelectedPlane);
                AirplaneName.text = info.FullName;
                foreach (GameObject g in _statsRects)
                {
                    EventController.Instance.Unsubscribe(g.GetComponent<GUIObject>() as IEventSubscriber);
                    Destroy(g);
                }
                _statsRects.Clear();
                int i;
                for (i = 0; i < 5; i++)
                {
                    GameObject rect = GameObject.Instantiate(i < info.Speed ? ActiveRectPrefab : UnactiveRectPrefab) as GameObject;
                    rect.transform.parent = transform;
                    _statsRects.Add(rect);
                    rect.transform.localPosition = new Vector3(0.08f*i,0.12f,-1);
                    rect.transform.localScale = new Vector3(0.07f,0.07f,1);
                    rect.GetComponent<Renderer>().enabled = GetComponent<Renderer>().enabled;
                }
                for (i = 0; i < 5; i++)
                {
                    GameObject rect = GameObject.Instantiate(i < info.Control ? ActiveRectPrefab : UnactiveRectPrefab) as GameObject;
                    rect.transform.parent = transform;
                    _statsRects.Add(rect);
                    rect.transform.localPosition = new Vector3(0.08f*i,-0.096f,-1);
                    rect.transform.localScale = new Vector3(0.07f,0.07f,1);
                    rect.GetComponent<Renderer>().enabled = GetComponent<Renderer>().enabled;
                }
                for (i = 0; i < 5; i++)
                {
                    GameObject rect = GameObject.Instantiate(i < info.Acceleration ? ActiveRectPrefab : UnactiveRectPrefab) as GameObject;
                    rect.transform.parent = transform;
                    _statsRects.Add(rect);
                    rect.transform.localPosition = new Vector3(0.08f*i,-0.306f,-1);
                    rect.transform.localScale = new Vector3(0.07f,0.07f,1);
                    rect.GetComponent<Renderer>().enabled = GetComponent<Renderer>().enabled;
                }
            }
           // Back.renderer.enabled = renderer.enabled;

        }
    }
}
