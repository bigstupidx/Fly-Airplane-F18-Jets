using UnityEngine;
using System.Collections;

public class BriefScreen : MonoBehaviour, IEventSubscriber
{

    public TextMesh TextMesh;

    public bool Active = true;
    public bool ChangeTime = true;

    public bool ShowMainMenu = true;
    public bool DestroyThe = false;

	// Use this for initialization
	void Start ()
	{

	    if (!DestroyThe)
	    {
            EventController.Instance.Subscribe("OnBriefHide", this);
            EventController.Instance.Subscribe("OnBriefHideTilt", this);
            EventController.Instance.Subscribe("OnBriefHideJoy", this);
            EventController.Instance.Subscribe("OnShowControls", this);
            EventController.Instance.Subscribe("OnShowMainMenu", this);
	    }

        gameObject.SetActive(Active);


	    if (TransportGOController.Instance.SelectedMissionID == 0)
	    {
//	        if (ChangeTime)
//	            Time.timeScale = 0f;
	        EventController.Instance.Subscribe("OnBriefHide", this);
	        EventController.Instance.Subscribe("OnBriefHideTilt", this);
	        EventController.Instance.Subscribe("OnBriefHideJoy", this);
	        EventController.Instance.Subscribe("OnShowControls", this);
	        EventController.Instance.Subscribe("OnShowMainMenu", this);
	    }
	    else
	    {
            if (DestroyThe)
	            Destroy(gameObject);
	    }
	}

    public void OnEvent(string EventName, GameObject Sender)
    {
        if (EventName == "OnShowControls")
        {
            gameObject.SetActive(true);
        }        
        if (EventName == "OnShowMainMenu")
        {
            gameObject.SetActive(false);
        }
        if (EventName == "OnBriefHide")
        {
//            if (ChangeTime)
//            Time.timeScale = 1.0f;

            gameObject.SetActive(false);
        }
        if (EventName == "OnBriefHideTilt")
        {
            OptionsController.Instance.Tilt = true;
            PlayerPrefs.SetInt("Tilt", OptionsController.Instance.Tilt ? 1 : 0);

            if(ShowMainMenu)
            EventController.Instance.PostEvent("OnShowMainMenu", null);
            else
            {
                EventController.Instance.PostEvent("OnResume", null);
            }

        }
        if (EventName == "OnBriefHideJoy")
        {
            OptionsController.Instance.Tilt = false;
            PlayerPrefs.SetInt("Tilt", OptionsController.Instance.Tilt ? 1 : 0);

            if (ShowMainMenu)
                EventController.Instance.PostEvent("OnShowMainMenu", null);
            else
            {
                EventController.Instance.PostEvent("OnResume", null);

                
            }
        }
    }
}
