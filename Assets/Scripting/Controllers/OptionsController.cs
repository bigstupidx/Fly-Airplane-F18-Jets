using UnityEngine;
using System.Collections;

public class OptionsController : MonoBehaviour, IEventSubscriber
{
//    public static readonly string AnalyticsID = "UA-56263344-16";
//    public static readonly string AdBannerID = "ca-app-pub-9255742339770963/8564079494";
    public static readonly string AdInterstialID = "ca-app-pub-9255742339770963/2137268291";
    public static readonly string IASURL = "http://ias.i6.com/ad/30.json";
    //public static readonly string PublisherID = "pub-9255742339770963";

    private static OptionsController s_Instance = null;
    
    public static OptionsController Instance 
    {
        get 
        {
            if (s_Instance == null)
                s_Instance = FindObjectOfType(typeof (OptionsController)) as OptionsController;
            if (s_Instance == null) 
            {
                GameObject obj = new GameObject("OptionsGO");
                s_Instance = obj.AddComponent(typeof (OptionsController)) as OptionsController;
                GameObject.DontDestroyOnLoad(s_Instance);
            }
            return s_Instance;
        }
    }
    
    void OnApplicationQuit() 
    {
        Save();
        s_Instance = null;
    }

    public void SelfSubscribe()
    {
        EventController.Instance.Subscribe("OnShowWinScreen", this);
        EventController.Instance.Subscribe("OnSaveData", this);
        EventController.Instance.Subscribe("OnLoadData", this);
        Load();
    }

    [Range(0,1)]
    public float MusicLevel = 1;
    [Range(0,1)]
    public float SFXLevel = 1;
    public bool InvertAxisY = false;
    public int PlayerMoney = 0;
    public bool Tilt = true;

    #region IEventSubscriber implementation

    private void Save()
    {
        PlayerPrefs.SetFloat("MusicLevel",this.MusicLevel);
        PlayerPrefs.SetFloat("SFXLevel",this.SFXLevel);
        PlayerPrefs.SetInt("InvertAxisY",this.InvertAxisY ? 1:0);
        PlayerPrefs.SetInt("PlayerMoney",this.PlayerMoney);
        PlayerPrefs.SetInt("Tilt",this.Tilt ? 1 : 0);
        for (int i=0;i<TransportGOController.Instance.PlanesInfo.Length;i++)
        {
            bool a = TransportGOController.Instance.PlanesInfo[i].Locked;
            bool b = TransportGOController.Instance.PlanesInfo[i].Buyout;
            PlayerPrefs.SetString(TransportGOController.Instance.PlanesInfo[i].Name,
                                  (a?"+":"-")+(b?"+":"-"));
        }
        string m = "";
        for (int i=0;i<TransportGOController.Instance.Missions.Length;i++)
            m+=(TransportGOController.Instance.Missions[i].Blocked?"+":"-");
        PlayerPrefs.SetString("MissionEnabled",m);
        PlayerPrefs.Save();
        EventController.Instance.PostEvent("OnUpdateOptions", null);
    }

    private void Load()
    {
        if (PlayerPrefs.HasKey("MusicLevel"))
            this.MusicLevel = PlayerPrefs.GetFloat("MusicLevel");
        
        if (PlayerPrefs.HasKey("SFXLevel"))
            this.SFXLevel = PlayerPrefs.GetFloat("SFXLevel");
        
        if (PlayerPrefs.HasKey("InvertAxisY"))
            this.InvertAxisY = PlayerPrefs.GetInt("InvertAxisY") == 1;
        
        if (PlayerPrefs.HasKey("PlayerMoney"))
            this.PlayerMoney = PlayerPrefs.GetInt("PlayerMoney");

        if (PlayerPrefs.HasKey("Tilt"))
            this.Tilt = PlayerPrefs.GetInt("Tilt") == 1;
        
        for (int i=0; i<TransportGOController.Instance.PlanesInfo.Length; i++)
        {
            if (PlayerPrefs.HasKey(TransportGOController.Instance.PlanesInfo [i].Name))
            {
                string data = PlayerPrefs.GetString(TransportGOController.Instance.PlanesInfo [i].Name);
                bool a = data [0] == '+';
                bool b = data [1] == '+';
                TransportGOController.Instance.PlanesInfo [i].Locked = a;
                TransportGOController.Instance.PlanesInfo [i].Buyout = b;
            }
        }
        
        if (PlayerPrefs.HasKey("MissionEnabled"))
        {
            string me = PlayerPrefs.GetString("MissionEnabled");
            for (int i=0; i<TransportGOController.Instance.Missions.Length; i++)
                TransportGOController.Instance.Missions [i].Blocked = me [i] == '+';
        }
        
        EventController.Instance.PostEvent("OnUpdateOptions", null);
    }

    public void OnEvent(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "OnSaveData":
                Save();
                break;

            case "OnLoadData":
                Load();
                break;

            case "OnShowWinScreen":
                Win();
                break;
        }
    }

    private static void Win()
    {
        int nextLevelID = TransportGOController.Instance.SelectedMissionID + 1;
        if (nextLevelID < TransportGOController.Instance.Missions.Length)
        {
            TransportGOController.Instance.Missions[nextLevelID].Blocked = false;
            EventController.Instance.PostEvent("OnSaveData", null);
        }
    }

    #endregion
}
