using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Activator : MonoBehaviour, IEventSubscriber
{
    public string Event;
    public List<string> DeactivateEvents;
    private void Start()
    {
        EventController.Instance.Subscribe(Event, this);
        foreach (string deactivateEvent in DeactivateEvents)
        {
            EventController.Instance.Subscribe(deactivateEvent, this);
            
        }
    }

    public void OnEvent(string EventName, GameObject Sender)
    {
        if (EventName == Event)
        {
            GetComponent<Camera>().enabled = true;
        }

        if (DeactivateEvents.Find(p => EventName == p) != null)
        {
            GetComponent<Camera>().enabled = false;
        }
    }
}
