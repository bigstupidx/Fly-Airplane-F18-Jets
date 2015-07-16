using UnityEngine;
using System.Collections;

public class FundsText : MonoBehaviour, IEventSubscriber
{
    public GameObject BuyButton;
	// Use this for initialization
	void Awake () {
        EventController.Instance.Subscribe("OnShowAirplaneSelecting", this);
        EventController.Instance.Subscribe("OnShowBuyPlanePopup", this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnEvent(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "OnShowAirplaneSelecting":
            {
                this.GetComponent<Renderer>().enabled = false;
                break;
            }
            case "OnShowBuyPlanePopup":
            {
                CheckFunds(Sender);

                BuyButton.GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;

                break;
            }
        }
        
    }

    private void CheckFunds(GameObject Sender)
    {

        PlaneSelectButton planeSelectButton = Sender.GetComponent<PlaneSelectButton>();
        AirplaneInfo info = TransportGOController.GetPlaneInfo(planeSelectButton.SelectAirplane);

        if (!info.Locked)
        {
            GetComponent<Renderer>().enabled = false;
            return;
        }

        int playerMoney = OptionsController.Instance.PlayerMoney;
        int planeCost = PlaneSelecting_Buy._airplaneToCost[planeSelectButton.SelectAirplane];
        if (playerMoney < planeCost)
        {
            GetComponent<Renderer>().enabled = true;
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
        }
    }
}
