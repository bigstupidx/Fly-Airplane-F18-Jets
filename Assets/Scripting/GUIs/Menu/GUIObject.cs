using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GUIObject : MonoBehaviour, IEventSubscriber
{
    protected virtual void AwakeProc() {}
    protected virtual void EventProc(string EventName, GameObject Sender) {}
    protected List<string> SubscrabeOnEvents;

    public string ShowOnEvent = "";

    void Awake()
    {
        SubscrabeOnEvents = new List<string>();
        SubscrabeOnEvents.Add("OnHideGUI");
        SubscrabeOnEvents.Add(ShowOnEvent);
        AwakeProc();
        SubscrabeOnEvents = new List<string>(SubscrabeOnEvents.Distinct());
        foreach (string tag in SubscrabeOnEvents)
            EventController.Instance.Subscribe(tag, this);
    }

    public void OnEvent(string EventName, GameObject Sender)
    {
        EventProc(EventName, Sender);
        if (EventName == "OnHideGUI")
        {
            if (GetComponent<Renderer>())
                GetComponent<Renderer>().enabled = false;
            if (GetComponent<Collider>())
                GetComponent<Collider>().enabled = false;
			if (GetComponent<BoxCollider2D>())
				GetComponent<BoxCollider2D>().enabled = false;
        } else if (EventName == ShowOnEvent)
        {
            if (GetComponent<Renderer>())
                GetComponent<Renderer>().enabled = true;
            if (GetComponent<Collider>())
                GetComponent<Collider>().enabled = true;
			if (GetComponent<BoxCollider2D>())
				GetComponent<BoxCollider2D>().enabled = true;
		}
    }

    void OnDestroy() 
    {
        EventController.Instance.Unsubscribe(this);
    }

}
