using UnityEngine;
using System.Collections;

public class ProgressBar_Music : ProgressBar 
{
    protected override void AwakeProc()
    {
        SubscrabeOnEvents.Add("OnUpdateOptions");
        base.AwakeProc();
    }

    void Start()
    {
        Progress = OptionsController.Instance.MusicLevel;
        EventController.Instance.PostEvent("OnUpdateGUI", null);
    }

    protected override void EventProc(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "OnUpdateGUI":
                Progress = OptionsController.Instance.MusicLevel;
                break;

            case "OnUpdateOptions":
                if (Sender == gameObject)
                    OptionsController.Instance.MusicLevel = Progress;
                break;
        }
        base.EventProc(EventName, Sender);
    }
}
