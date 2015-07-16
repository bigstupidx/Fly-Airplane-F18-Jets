using UnityEngine;
using System.Collections;

public abstract class State
{
    public bool Ended { get; protected set; }
    public string MissionStateText { get; protected set; }

    public virtual void Start()
    {
        Debug.Log("ENTER " + GetType().Name);

    }

    public abstract void Update();


    public abstract MissionObject GetTarget();
}
