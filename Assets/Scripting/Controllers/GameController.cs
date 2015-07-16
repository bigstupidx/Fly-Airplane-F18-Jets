using System;
using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour, IEventSubscriber
{
    #region IEventSubscriber implementation

	void Awake()
    {
        EventController.Instance.Subscribe("OnRestart", this);
        EventController.Instance.Subscribe("OnMainMenu", this);
        EventController.Instance.Subscribe("MissionFailed", this);
        EventController.Instance.Subscribe("MissionFinished", this);
        EventController.Instance.Subscribe("OnLoadNextLevel", this);
        EventController.Instance.Subscribe("Stop", this);
        EventController.Instance.Subscribe("OnShowPause", this);
        EventController.Instance.Subscribe("OnShowPauseMenu", this);
        EventController.Instance.Subscribe("OnResume", this);

        EventController.Instance.PostEvent("OnLoadedMenuScene", null);

        OptionsController.Instance.SelfSubscribe();
//        AdMobAndroid.destroyBanner();
		//DontDestroyOnLoad(this);
    }

    private bool _finished = false;

    public void OnEvent(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "OnShowPause":
				if(GoogleAnalytics.Instance)
					GoogleAnalytics.Instance.LogScreen("Level " + TransportGOController.Instance.SelectedMissionID + 1 + " - Pause Screen");
                break;

            case "OnRestart":
                Time.timeScale = 1; 
				PlayerPrefs.SetInt("restart" ,1);

			// adverts
			PlayerPrefs.SetInt("RestartsSinceLastInt", PlayerPrefs.GetInt("RestartsSinceLastInt")+1);
			
			if(PlayerPrefs.GetInt("RestartsSinceLastInt") >= 3)
			{
				AdMob_Manager.Instance.LoadInterstitial(true);
				PlayerPrefs.SetInt("RestartsSinceLastInt", 0);
			}

				ShowLoading();
				break;

            case "OnMainMenu":
                Time.timeScale = 1;
                ShowLoading();
				PlayerPrefs.SetInt("restart" ,1);
				Application.LoadLevel(0);
                break;

            case "MissionFailed":
                if (MissionController.Instance.Finished)
                    break;
                EventController.Instance.PostEvent("OnHideGUI",null);
                EventController.Instance.PostEvent("OnPause",null);
                StartCoroutine(EventWithSleep("OnShowLoseScreen",1));
                StartCoroutine(EventWithSleep("OnShowAirplaneSelecting",1));
                GameObject.FindObjectOfType<PlaneSelecting_Buy>().NextButton = 
                    GameObject.Find("Button_LoseRestart");
				
				if(GoogleAnalytics.Instance)
					GoogleAnalytics.Instance.LogScreen("Level " + (TransportGOController.Instance.SelectedMissionID + 1) + " - Death");

                //AdMobAndroid.createBanner("ca-app-pub-9255742339770963/5809451896",
                  //                        AdMobAndroidAd.phone320x50, AdMobAdPlacement.BottomLeft );
                //AdMobAndroid.requestInterstital(OptionsController.AdInterstialID);
				IAS_Manager.Instance.ResetMainBanners();
				break;

            case "MissionFinished":
                EventController.Instance.PostEvent("OnPause",null);
                EventController.Instance.PostEvent("OnHideGUI",null);
                if (TransportGOController.Instance.SelectedMissionID >= 19)
                {
                    StartCoroutine(EventWithSleep("OnShowFinalWinScreen", 3));

                }
                else
                {
                    StartCoroutine(EventWithSleep("OnShowWinScreen", 3));
                    StartCoroutine(EventWithSleep("OnShowAirplaneSelecting", 3));
                }
                GameObject.FindObjectOfType<PlaneSelecting_Buy>().NextButton = 
                    GameObject.Find("Button_WinNext");

				if(GoogleAnalytics.Instance)
					GoogleAnalytics.Instance.LogScreen("Level " + (TransportGOController.Instance.SelectedMissionID + 1) + " - Win");
                //AdMobAndroid.createBanner("ca-app-pub-9255742339770963/5809451896",
                  //                        AdMobAndroidAd.phone320x50, AdMobAdPlacement.BottomLeft );
//                AdMobAndroid.requestInterstital(OptionsController.AdInterstialID);
                MissionController.Instance.Finished = true;

				IAS_Manager.Instance.ResetMainBanners();

				//AdMob_Manager.Instance.LoadInterstitial(true);

                break;

            case "OnLoadNextLevel":
				
                int id = TransportGOController.Instance.SelectedMissionID+1;
                if (id<TransportGOController.Instance.Missions.Length)
                    TransportGOController.Instance.SelectedMissionID = id;
                GameObject.DontDestroyOnLoad(TransportGOController.Instance.gameObject);

                ShowLoading();

                break;
            case "OnShowPauseMenu":
                Time.timeScale = 0;
                break;
            case "OnResume":
                Time.timeScale = 1;
                break;

        }
    }

    private static void ShowLoading()
    {
		EventController.Instance.PostEvent("OnHideGUI", null);
        EventController.Instance.PostEvent("OnShowLoading", null);
		//adverts
		if (PlayerPrefs.GetInt("restart") == 0)
		{
			AdMob_Manager.Instance.LoadInterstitial(true);
			Debug.Log("IAS loaded");
		}
		else
			PlayerPrefs.SetInt("restart" ,0);

		Application.LoadLevel("ads");
	}

    #endregion

    private IEnumerator EventWithSleep(string EventName,float Sleep)
    {
        yield return new WaitForSeconds(Sleep);
        EventController.Instance.PostEvent(EventName, null);
    }


}
