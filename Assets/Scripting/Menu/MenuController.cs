using UnityEngine;
using System.Collections;

public enum MenuState
{
	Main,
	Options,
	AirplaneSelect,
	MissionSelect,
	HelpScreen
}

public class MenuController : MonoBehaviour, IEventSubscriber
{
	public static MenuController Instance;
	public GameObject TGO;
	//public GameObject IASprefab;
	
	public Camera Cam2D;
	
	void Awake()
	{
		/*

		if(!Instance){
			Instance = this;

			if(IASprefab != null)
				Instantiate(IASprefab);
		} else {
			Destroy(gameObject);
			return;
		}
		*/
		
		GestureController.Instance.OnGestureStart += OnGestureStart;
		GestureController.Instance.OnGestureEnd = OnGestureEnd;
		Input.gyro.enabled = true;
		if (FindObjectOfType<DataStorageController>() == null)
			GameObject.Instantiate(TGO);
	}
	
	void Start()
	{
		EventController.Instance.SubscribeToAllEvents(this);
		
		EventController.Instance.PostEvent("OnHideGUI", null);
		EventController.Instance.PostEvent("OnLoadedMenuScene", null);
		EventController.Instance.PostEvent("OnShowMainMenu", null);

		OptionsController.Instance.SelfSubscribe();
	}
	
	private GameObject _activeButton;
	private Texture _firstTex;
	
	public Gesture LastStartedGesture { get; private set; }
	
	void OnGestureStart(Gesture g)
	{
		LastStartedGesture = g;
		Ray ray = Cam2D.ScreenPointToRay(g.StartPoint);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			EventController.Instance.PostEvent("OnPressObject", hit.collider.gameObject);
			g.OnAddingTouch+=OnGestureMoved;
		}
		else
			EventController.Instance.PostEvent("OnPressObject",null);
	}
	
	void OnGestureMoved(Gesture g)
	{
		if (Vector2.Distance(g.StartPoint,g.EndPoint)>5)
		{
			EventController.Instance.PostEvent("OnTouchMoved", null);
		}
	}
	
	void OnGestureEnd(Gesture g)
	{
		Ray ray = Cam2D.ScreenPointToRay(g.EndPoint);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
			EventController.Instance.PostEvent("OnReleaseObject",hit.collider.gameObject);
		else
			EventController.Instance.PostEvent("OnReleaseObject",null);
	}
	
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
			EventController.Instance.PostEvent("OnGoBack",null);
	}
	
	private MenuState _menuState;
	public MenuState MenuMode { get { return _menuState; } }
	
	#region IEventSubscriber implementation

	public void OnEvent(string EventName, GameObject Sender)
	{
		switch (EventName)
		{
		case "OnShowGameMode":
			PlayerPrefs.SetInt("HUDcolor",0);
			if (GameObject.Find("GreenColor") != null)
				GameObject.Find("GreenColor").transform.localScale = new Vector3(1.3f, 1.3f, 1);

			if (PlayerPrefs.GetInt("restart") == 0)
			{
				AdMob_Manager.Instance.LoadInterstitial(true);
				Debug.Log("IAS loaded");
			}
			else
				PlayerPrefs.SetInt("restart" ,0);

				Application.LoadLevel("ads");
			break;
			
		case "OnRestoreDefaults":
			OptionsController.Instance.MusicLevel = 1;
			OptionsController.Instance.SFXLevel = 1;
			OptionsController.Instance.InvertAxisY = false;
			EventController.Instance.PostEvent("OnUpdateGUI",null);
			EventController.Instance.PostEvent("OnUpdateOptions",null);
			break;
			
		case "OnShowMainMenu":
			Debug.Log ("Main Menu loaded");
			_menuState = MenuState.Main;
			if (GoogleAnalytics.Instance)
				GoogleAnalytics.Instance.LogScreen("Main Menu");
			break;

		case "OnShowIAS":
			//AdMob_Manager.Instance.LoadInterstitial(true);
			break;

		case "OnShowAirplaneSelecting":
			_menuState = MenuState.AirplaneSelect;
			IAS_Manager.Instance.ResetMainBanners();
			PlayerPrefs.SetInt("restart" ,0);
			
			if (GoogleAnalytics.Instance)
				GoogleAnalytics.Instance.LogScreen("Airplane Scelecting");
			break;
			
		case "OnShowMissionSelecting":
			_menuState = MenuState.MissionSelect;
			if (GoogleAnalytics.Instance)
				GoogleAnalytics.Instance.LogScreen("Mission Selecting");
			break;
			
		case "OnShowOptions":
			_menuState = MenuState.Options;
			if (GoogleAnalytics.Instance)
				GoogleAnalytics.Instance.LogScreen("Setting menu");
			break;
			
		case "OnShowHelp":
			_menuState = MenuState.HelpScreen;
			if (GoogleAnalytics.Instance)
				GoogleAnalytics.Instance.LogScreen("Help Screen");
			break;
			
		case "OnGoBack":
			if (_menuState != MenuState.Main)
				EventController.Instance.PostEvent("OnHideGUI",null);
			switch (_menuState)
			{
			case MenuState.AirplaneSelect:
				EventController.Instance.PostEvent("OnShowMainMenu",null);
				break;
				
			case MenuState.MissionSelect:
				EventController.Instance.PostEvent("OnShowAirplaneSelecting",null);
				break;
				
			case MenuState.HelpScreen:
				EventController.Instance.PostEvent("OnShowMainMenu",null);
				break;
				
			case MenuState.Options:
				EventController.Instance.PostEvent("OnSaveData",null);
				EventController.Instance.PostEvent("OnShowMainMenu",null);
				break;
				
			case MenuState.Main:
				//Application.Quit();
				break;
			}
			break;
			
		case "OnResetProgress":
			for (int i=1;i<TransportGOController.Instance.Missions.Length;i++)
				TransportGOController.Instance.Missions[i].Blocked = true;
			if (GoogleAnalytics.Instance)
				GoogleAnalytics.Instance.LogScreen("Settings - Reset progress");
			break;
			
		case "OnResetPurchases":
			var skus = new string[] { "airplane_f22", "airplane_fa38" };
			// GoogleIAB.queryInventory( skus );
			if (GoogleAnalytics.Instance)
				GoogleAnalytics.Instance.LogScreen("Settings - Restore purchases");
			/*
                AirplaneInfo p = TransportGOController.GetPlaneInfo(Airplanes.Mirage);
                p.Locked = true;
                p.Buyout = false;
                p = TransportGOController.GetPlaneInfo(Airplanes.F_16);
                p.Locked = true;
                p.Buyout = true;
                p = TransportGOController.GetPlaneInfo(Airplanes.FA_22);
                p.Locked = true;
                p.Buyout = true;
                p = TransportGOController.GetPlaneInfo(Airplanes.FA_38);
                p.Locked = true;
                p.Buyout = true;
                p = TransportGOController.GetPlaneInfo(Airplanes.SAAB);
                p.Locked = false;
                p.Buyout = false;
                */
			break;
			
		case "OnShowMoreGames":
			Application.OpenURL("https://play.google.com/store/apps/developer?id=i6+Games");
			if (GoogleAnalytics.Instance)
				GoogleAnalytics.Instance.LogScreen("More Games");
			break;
			
		case "OnDebugAddGold":
			OptionsController.Instance.PlayerMoney += 1000;
			EventController.Instance.PostEvent("OnUpdateOptions", null);
			EventController.Instance.PostEvent("OnSaveData", null);
			break;
			
		case "OnLoadedMenuScene":
			//                AdMobAndroid.destroyBanner();
			break;
			
			// buy HUD
		case "buyHUD":
			if (PlayerPrefs.GetInt("PlayerMoney") > 20000)
			{
				PlayerPrefs.SetInt("PlayerMoney", PlayerPrefs.GetInt("PlayerMoney") - 20000);
				EventController.Instance.PostEvent("buyHUDsucceed", null);
				PlayerPrefs.SetInt("additionalHUD",1);
				Application.LoadLevel(Application.loadedLevel);
			}
			else
				EventController.Instance.PostEvent("buyHUDfail", null);
			break;
			
		case "SelectGreenColor":
			GameObject.Find("GreenColor").transform.localScale = new Vector3(1.3f, 1.3f, 1);
			GameObject.Find("BlueColor").transform.localScale = new Vector3(1, 1, 1);
			GameObject.Find("YellowColor").transform.localScale = new Vector3(1, 1, 1);
			
			PlayerPrefs.SetInt("HUDcolor",0);
			break;
			
		case "SelectBlueColor":
			GameObject.Find("BlueColor").transform.localScale = new Vector3(1.3f, 1.3f, 1);
			GameObject.Find("GreenColor").transform.localScale = new Vector3(1, 1, 1);
			GameObject.Find("YellowColor").transform.localScale = new Vector3(1, 1, 1);
			
			PlayerPrefs.SetInt("HUDcolor",1);
			break;
			
		case "SelectYellowColor":
			GameObject.Find("YellowColor").transform.localScale = new Vector3(1.3f, 1.3f, 1);
			GameObject.Find("GreenColor").transform.localScale = new Vector3(1, 1, 1);
			GameObject.Find("BlueColor").transform.localScale = new Vector3(1, 1, 1);
			
			PlayerPrefs.SetInt("HUDcolor",2);
			break;
		}
	}
	
	#endregion
}