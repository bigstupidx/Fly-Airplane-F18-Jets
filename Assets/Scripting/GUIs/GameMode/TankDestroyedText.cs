using System;
using System.Timers;
using UnityEngine;
using System.Collections;
using Timer = DN.Timer;

public class TankDestroyedText : MonoBehaviour, IEventSubscriber
{
    public AnimationCurve TextAlphaCurve;

    private Timer _hideRendererTimer;
    private TextMesh mText;

	// Use this for initialization
	void Start () 
    {
        EventController.Instance.Subscribe("MissionObjectDestroyed", this);
        _hideRendererTimer = new Timer {Duration = 1.5f};
	    _hideRendererTimer.OnTick += OnHideRendererTimer;

	    mText = GetComponent<TextMesh>();
	    mText.GetComponent<Renderer>().enabled = false;
    }



    public void OnEvent(string EventName, GameObject Sender)
    {
        if (EventName == "MissionObjectDestroyed")
        {
            ShowText();
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        _hideRendererTimer.Update(Time.deltaTime);
	    if (_hideRendererTimer.IsRunning)
	    {
	        Color color = mText.color;
	        color.a = TextAlphaCurve.Evaluate(_hideRendererTimer.Elapsed);
	        mText.color = color;
	    }

    }

    private void ShowText()
    {
        GetComponent<Renderer>().enabled = true;
        _hideRendererTimer.Run(reset: true);
    }

    private void OnHideRendererTimer(object sender, EventArgs eventArgs)
    {
        GetComponent<Renderer>().enabled = false;
    }
}
