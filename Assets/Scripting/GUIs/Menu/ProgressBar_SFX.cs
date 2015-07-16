using UnityEngine;
using System.Collections;

public class ProgressBar_SFX : ProgressBar 
{
    protected override void AwakeProc()
    {
        SubscrabeOnEvents.Add("OnUpdateOptions");
        base.AwakeProc();
    }

    void Start()
    {
        Progress = OptionsController.Instance.SFXLevel;
        EventProc("OnUpdateGUI", null);
    }

    protected override void EventProc(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "OnUpdateGUI":
                Progress = OptionsController.Instance.SFXLevel;
                break;

            case "OnUpdateOptions":
                if (Sender == gameObject)
                    OptionsController.Instance.SFXLevel = Progress;
                break;
        }
        base.EventProc(EventName, Sender);
    }
}
