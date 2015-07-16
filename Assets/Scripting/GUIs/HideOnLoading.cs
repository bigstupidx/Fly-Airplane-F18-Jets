using UnityEngine;
using System.Collections;

public class HideOnLoading : MonoBehaviour,IEventSubscriber {

	// Use this for initialization
	void Start () {
        EventController.Instance.Subscribe("OnShowLoading", this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnEvent(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "OnShowLoading":
            {
                gameObject.SetActive(false);
                break;
            }
        }
    }
}
