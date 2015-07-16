using UnityEngine;
using System.Collections;

public class AttackButton : MonoBehaviour, IEventSubscriber
{
    private float _timer;
    public float DistanceToAtack = 5000;
    public float TimeDelay = 0.5f;

    public bool _update = false;

    #region IEventSubscriber implementation

    public void OnEvent(string EventName, GameObject Sender)
    {
        if (EventName == "MissionFinished")
        {
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            _update = true;
            return;
        }

        if (_timer + TimeDelay < Time.time)
        {
            switch (EventName)
            {
                case "AttackButtonPressed":
                    _timer = Time.time;
                    EventController.Instance.PostEvent("StartRocket", null);
                    break;
            }
        }
    }

    void Awake()
    {
        _timer = Time.time;
        EventController.Instance.SubscribeToAllEvents(this);
    }

    #endregion

    void Update()
    {
        if (_update)
            return;
        if (Radar.Instance.DistanceToTarget < this.DistanceToAtack && !this.GetComponent<Renderer>().enabled
            && MissionController.Instance.CurrentState is DestroyTargetState)
        {
            GetComponent<Renderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
        }

        if ((Radar.Instance.DistanceToTarget > this.DistanceToAtack && this.GetComponent<Renderer>().enabled) 
            || !(MissionController.Instance.CurrentState is DestroyTargetState))
        {
            if (this.GetComponent<Renderer>().enabled)
            {
                GetComponent<Renderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
            }
        }
    }
}
