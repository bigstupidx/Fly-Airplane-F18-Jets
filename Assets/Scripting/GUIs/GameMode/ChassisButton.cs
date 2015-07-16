using UnityEngine;
using System.Collections;

public class ChassisButton : MonoBehaviour, IEventSubscriber
{
    public Texture ActiveButtonTexture;
    public Texture NoActiveButtonTexture;

    #region IEventSubscriber implementation

    public void OnEvent(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "WheelsDown":
                GetComponent<Renderer>().material.SetTexture("_MainTex",ActiveButtonTexture);
                break;
            case "WheelsUp":
                GetComponent<Renderer>().material.SetTexture("_MainTex",NoActiveButtonTexture);
                break;
        }
    }

    void Awake()
    {
        EventController.Instance.Subscribe("WheelsDown", this);
        EventController.Instance.Subscribe("WheelsUp", this);
    }

    #endregion
}
