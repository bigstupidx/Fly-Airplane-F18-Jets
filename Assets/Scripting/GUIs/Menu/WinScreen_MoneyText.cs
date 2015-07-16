using System.Linq;
using UnityEngine;
using System.Collections;

public class WinScreen_MoneyText : MonoBehaviour, IEventSubscriber
{
    void Start()
    {
        //gameObject.GetComponent<TextMesh>().text = TransportGOController.Instance.Missions [TransportGOController.Instance.SelectedMissionID].Payment.ToString();
        EventController.Instance.Subscribe("MissionFinished", this);
    }

    #region IEventSubscriber implementation


    public static readonly int[] missionAwards = new[]
    {
        1000,
        3000,
        4000,
        5000,
        5500,
        6000,
        7000,
        8000,
        8500,
        9000,
        10000,
        11000,
        12300,
        13000,
        14500,
        15000,
    }.Select(p => p/2).ToArray();

    public void OnEvent(string EventName, GameObject Sender)
    {
        if (EventName == "MissionFinished")
        {
            int money = GetMoneyReward(TransportGOController.Instance.SelectedMissionID);

            OptionsController.Instance.PlayerMoney += money;//TransportGOController.Instance.Missions [].Payment;
            EventController.Instance.PostEvent("OnSaveData", null);
        }
    }

    public static int GetMoneyReward(int id)
    {
        int money;
        if (id >= missionAwards.Length)
        {
            money = 15000;
        }
        else
        {
            money = missionAwards[id];
        }
        return money;
    }

    #endregion
}
