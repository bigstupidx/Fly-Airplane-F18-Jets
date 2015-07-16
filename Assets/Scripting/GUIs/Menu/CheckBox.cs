using UnityEngine;
using System.Collections;

public class CheckBox : Button 
{
    public bool Checked = false;

    void Start()
    {
        EventController.Instance.Subscribe("OnUpdateGUI", this);
    }

    protected override void EventProc(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "OnPressObject":
                if (Sender == gameObject)
                {
                    if (Sender == gameObject)
                        EventController.Instance.PostEvent("OnPlayButtonPress",null);
                }
                break;

            case "OnReleaseObject":
                if (Sender == gameObject)
                {
                    EventController.Instance.PostEvent("OnPlayButtonRelease",null);
                    Checked = !Checked;
                    OptionsController.Instance.InvertAxisY = Checked;
                    EventController.Instance.PostEvent("OnUpdateOptions",gameObject);
                    if (Checked)
                        GetComponent<Renderer>().material.SetTexture("_MainTex", ActiveTexture);
                    else
                        GetComponent<Renderer>().material.SetTexture("_MainTex", MainTexture);
                }
                break;

            case "OnUpdateGUI":
                Checked = OptionsController.Instance.InvertAxisY;
                if (Checked)
                    GetComponent<Renderer>().material.SetTexture("_MainTex", ActiveTexture);
                else
                    GetComponent<Renderer>().material.SetTexture("_MainTex", MainTexture);
                break;
        }
    }
}
