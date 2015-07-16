using UnityEngine;
using System.Collections;

public class TakeoffState : State, IEventSubscriber
{
    private readonly MissionObject _target;

    public TakeoffState(MissionObject target)
    {
        _target = target;

        MissionStateText = "Take off";
    }

    public override void Start()
    {
        base.Start();
        EventController.Instance.Subscribe("Takeoff", this);
    }

    public override void Update()
    {
    }

    public override MissionObject GetTarget()
    {
        return _target;
    }

    public void OnEvent(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "Takeoff":
                Ended = true;
                EventController.Instance.Unsubscribe("Takeoff", this);
                break;
        }
    }
}
