using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreCloseScreen : MonoBehaviour, IEventSubscriber
{
    public GameObject Items;

	public List<PreCloseScreenImageElement> Images;
    [NonSerialized]
    public PreCloseScreen Instance;

    public bool IsMainMenu = false;

    private bool CanOpen = true;
	
	private bool _placedImages;
    private float _timeScaleBefore;
	void Start()
	{
	    Instance = this;
	    _placedImages = false;
		if(PrecloseScreenIAS.Instance.preReady)
		{
			PlaceImages();
		}

		EventController.Instance.Subscribe("LoadedIAS", this);
        EventController.Instance.Subscribe("OnPrecloseAdClick", this);
        EventController.Instance.Subscribe("OnClosePreCloseScreen", this);
        EventController.Instance.Subscribe("RateGame", this);
        EventController.Instance.Subscribe("OnQuitGame", this);
        EventController.Instance.Subscribe("OnShowMainMenu", this);
        EventController.Instance.Subscribe("OnShowAirplaneSelecting", this);
	}

	void Update()
	{
	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        if (Items.activeSelf)
	        {
	            Close();
	        }
	        else
	        {
	            Show();
	        }
	    }

	    if(!_placedImages)
		{
            if (PrecloseScreenIAS.Instance != null)
			if(PrecloseScreenIAS.Instance.preReady)
			{
				PlaceImages();
			}
		}
	}

	#region IEventSubscriber implementation
	public void OnEvent (string EventName, GameObject Sender)
	{
		if(EventName == "OnQuitGame")
		{
			Debug.Log("Quit game");
			Application.Quit();
		}
		else if(EventName == "RateGame")
		{
            Application.OpenURL("market://details?id=com.i6.f18airplanesimulator3d");
		}
		else if(EventName == "OnPrecloseAdClick")
		{
            ShowAdd(Sender);
		}
        else if (EventName == "OnClosePreCloseScreen")
		{
			Close ();
		}
        else if (EventName == "OnShowMainMenu")
        {
            CanOpen = true;
        }
        else if (EventName == "OnShowAirplaneSelecting")
        {
            CanOpen = false;
        }
	}

	#endregion


    void ShowAdd(GameObject sender)
    {
        PreCloseScreenImageElement element = Images.Find(p => p.GuiObject.gameObject == sender);
        Application.OpenURL(element.Url);
    }

    private void PlaceImages()
    {
        _placedImages = true;
        for (int i = 0; i < PrecloseScreenIAS.Instance.preBannerTextures.Count; i++)
        {
            var texture = PrecloseScreenIAS.Instance.preBannerTextures[i];
            var url = PrecloseScreenIAS.Instance.preBannerURL[i];
            var imageElement = Images.Find(p => !p.Placed);
            if (imageElement == null)
            {
                break;
            }
            imageElement.Image.material.mainTexture = texture;
            imageElement.GuiObject.MainTexture = texture as Texture2D;
            imageElement.GuiObject.ActiveTexture = texture as Texture2D;
            imageElement.Url = url;
            imageElement.Placed = true;
        }
    }

    private void Show()
    {
        if (!CanOpen)
        {
            return;
        }

        _timeScaleBefore = Time.timeScale;
        Time.timeScale = 0;
        Items.SetActive(true);

        if (!IsMainMenu)
        {
            AudioListener.volume = 0;
        }

        EventController.Instance.PostEvent("OnShowPrecloseScreen", null);
    }

    void Close()
    {
        AudioListener.volume = 1;

        Time.timeScale = _timeScaleBefore;
        Items.SetActive(false);
    }
}
