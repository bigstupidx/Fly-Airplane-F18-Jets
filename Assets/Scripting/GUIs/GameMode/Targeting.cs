using UnityEngine;
using System.Collections;

public class Targeting : MonoBehaviour, IEventSubscriber
{
    public GameObject OutGUI;
    public GameObject TargetGUI;

    private GameObject _target = null;

    #region IEventSubscriber implementation

    public void OnEvent(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "TargetingActive":
                StartCoroutine(ActivateTargeting());
                break;
            case "TargetingDeactive":
                StopAllCoroutines();
                SetAlpha(TargetGUI, 0);
                SetAlpha(OutGUI, 0);
                SetAlpha(gameObject, 0);
                break;
            case "MissionObjectDestroyed":
                EventController.Instance.PostEvent("TargetingDeactive",null);
                break;
            default:
                {
                    //_target = AirplaneController.Instance.GetMissionObject().gameObject;
                    if (MissionController.Instance.CurrentState is DestroyTargetState)
                    {
                        _target = MissionController.Instance.CurrentTarget.gameObject;
                    }
                    else
                    {
                        _target = null;
                        SetAlpha(TargetGUI, 0);
                    }
                }
                break;
        }
    }

    void Awake()
    {
        EventController.Instance.Subscribe("MissionStarted", this);
        EventController.Instance.Subscribe("MissionChangeTarget", this);
        EventController.Instance.Subscribe("TargetingActive", this);
        EventController.Instance.Subscribe("TargetingDeactive", this);
        EventController.Instance.Subscribe("MissionObjectDestroyed", this);
        SetAlpha(TargetGUI,0);
        SetAlpha(OutGUI,0);
        SetAlpha(gameObject,0);
    }

    #endregion

    IEnumerator ActivateTargeting()
    {
        float time = Time.time;
        while (Time.time<time+1)
        {
            float d = 1 - (Time.time - time);
            OutGUI.transform.localScale = new Vector3(d,d,1);
            SetAlpha(OutGUI,d);
            OutGUI.transform.localPosition= Vector3.Lerp(TargetGUI.transform.localPosition,Vector3.zero,d);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(ActivateTargeting());
    }

    void SetAlpha(GameObject Something, float Alpha)
    {
        if (Something.GetComponent<Renderer>())
        {
            Something.GetComponent<Renderer>().enabled = Alpha!=0;
            Color col = Something.GetComponent<Renderer>().material.GetColor("_Color");
            col.a = Alpha;
            Something.GetComponent<Renderer>().material.SetColor("_Color",col);
        }
    }

    private bool _inTarget = false;
    private float _oldValue;
    private bool inProgress = false;
    void Update()
    {
        var missionObject = AirplaneController.Instance.GetMissionObject();
        if(missionObject !=null)
        {
            var newGameObject = AirplaneController.Instance.GetMissionObject().gameObject;

            if (_target != newGameObject)
            {
                _inTarget = false;
                inProgress = false;
                _oldValue = 0.0f;
                if(_inTarget)
                {
                    EventController.Instance.PostEvent("TargetingDeactive", null);
                }
                if(inProgress)
                {
                    EventController.Instance.PostEvent("TargetingInProgressEnd", null);
                }

            }
            _target = newGameObject;
        }
        else
        {
            _target = null;
        }
        if (_target && Vector3.Angle(AirplaneController.Instance.transform.forward,
                                     _target.transform.position - AirplaneController.Instance.transform.position)<90)
        {
            Vector3 pos = Camera3DInstance.Instance.GetComponent<Camera>().WorldToScreenPoint(_target.transform.position);
            pos.z = 0;
            pos = GUICameraController.Instance.GetComponent<Camera>().ScreenToWorldPoint(pos);
            pos = transform.InverseTransformPoint(pos);
            pos.z = 0;
            TargetGUI.transform.localPosition = pos;
            float m = pos.magnitude;
            SetAlpha(TargetGUI, 1-m);
            SetAlpha(gameObject, 1-m);

            //Debug.Log(m);

            if (m < 1f) 
            {
                if (!inProgress)
                {
                    EventController.Instance.PostEvent("TargetingInProgress", null);
                    inProgress = true;
                }
            }
            else if (_oldValue >= 1f)
            {
                if (inProgress)
                {
                    EventController.Instance.PostEvent("TargetingInProgressEnd", null);
                    inProgress = false;
                }
            }

            _oldValue = m;

            if (m<0.5f)
            {
                if (!_inTarget)
                {
                    EventController.Instance.PostEvent("TargetingActive",null);
                    _inTarget = true;
                }
            } else
            {
                if (_inTarget)
                {
                    EventController.Instance.PostEvent("TargetingDeactive",null);
                    _inTarget = false;
                }
                SetAlpha(OutGUI,0);
            }
        }
    }
}
