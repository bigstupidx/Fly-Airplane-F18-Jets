using UnityEngine;
using System.Collections;

public class HIddenPlane : GUIObject 
{
    public Color StartColor;
    public Color EndColor;
    public float AnimTime = 3;

    private bool shown;

    protected override void EventProc(string EventName, GameObject Sender)
    {
        if(shown)
        {
            return;
        }
        
        base.EventProc(EventName, Sender);

        if (EventName == base.ShowOnEvent)
        {
            GetComponent<Renderer>().material.SetColor("_Color",StartColor);
            StartCoroutine(Anim());
            shown = true;
        }
    }

    IEnumerator Anim()
    {
        float time = UnityEngine.Time.time;
        float delta = 0;
        while ((delta = Time.time - time) < AnimTime)
        {
            Color col = Color.Lerp(StartColor,EndColor,delta/AnimTime);
            GetComponent<Renderer>().material.SetColor("_Color",col);
            yield return new WaitForEndOfFrame();
        }
        GetComponent<Renderer>().material.SetColor("_Color",EndColor);
    }
}
