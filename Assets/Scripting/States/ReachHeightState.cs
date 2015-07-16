using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ReachHeightState:State
{
    private readonly float _height;

    public ReachHeightState(float height)
    {
        _height = height;
        if (height == 0)
        {
            Ended = true;
        }

        MissionStateText = "Reach attitude of " + height + " ft";
    }

    public override void Update()
    {
        if (AirplaneController.Instance.Height > _height)
        {
            Ended = true;
        }
    }

    public override MissionObject GetTarget()
    {
        return null;
    }
}
