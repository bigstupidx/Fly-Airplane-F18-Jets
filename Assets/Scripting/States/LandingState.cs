using UnityEngine;
using System.Collections;

public class LandingState : State, IEventSubscriber
{
    private readonly MissionObject _target;

    public LandingState(MissionObject target)
    {
        _target = target;
        MissionStateText = "Land";
    }

    public override void Start()
    {
        base.Start();
        EventController.Instance.Subscribe("Landing", this);
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
            case "Landing":
                EventController.Instance.Unsubscribe("Takeoff", this);
                Ended = true;
                break;
        }
    }
}
