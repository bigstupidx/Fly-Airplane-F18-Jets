using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyTargetState : State {
    private readonly List<MissionObject> _targets;
    private int _current;

    public DestroyTargetState(List<MissionObject> targets)
    {
        _targets = targets;
        MissionStateText = "Destroy target";

        if (_targets.Count == 0)
        {
            Ended = true;
        }
    }

    public override void Start()
    {
        

    }

    public override void Update()
    {
        if (CurrentTarget.Destroyed)
        {
            _current++;

            if (CurrentTarget == null)
            {
                Ended = true;
            }
        }
    }

    private MissionObject CurrentTarget
    {
        get
        {
            if (_current > _targets.Count - 1) return null;
            return _targets[_current];
        }
    }

    public override MissionObject GetTarget()
    {
        return CurrentTarget;
    }
}
