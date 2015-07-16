using System.Linq;
using UnityEngine;
using System.Collections;

public enum AirplaneStates
{
    Ride,
    Fly,
    Die
}

public class AirplaneController : MonoBehaviour, IEventSubscriber
{
    public static AirplaneController Instance { get; private set; }

    public float Height {
        get { return _lastHeight; }
    }

    public AirplaneController()
    {
        Instance = this;
    }

    public float MaxSpeed = 100;
    public float MinFlySpead = 100;
    public float Acceleration = 50;
    public float Breaking = 50;

    public Vector2 AccelRotation = Vector2.zero;
    public Vector2 MaxRotation = Vector2.zero;
    public Vector2 BreakRotation = Vector2.zero;

    public AirplaneDriver Driver;

    public bool IsMakingTurn;

    private float _targetSpeed = 0;
    private float _lastHeight;

    public float TargetSpeed
    {
        get
        {
            if (ChassisEnable)
            {
                if (State == AirplaneStates.Ride &&
                    MissionController.Instance.CurrentState is LandingState)
                    return 0;
                else
                    return Mathf.Min(_targetSpeed, MaxSpeed*0.45f);
            }
            else
                return _targetSpeed;
        }
        set { _targetSpeed = value; }
    }

    public float TurnDirection;

    public float CurrentSpeed = 0;

    public float Speed
    {
        get { return TargetSpeed; }
        set
        {
            TargetSpeed = value*MaxSpeed;
            Driver.Speed = value;
        }
    }

    public Vector2 TargetRotation = Vector2.zero;
    public Vector2 CurrentRotation = Vector2.zero;

    public Vector2 Rotation
    {
        get { return TargetRotation; }
        set
        {
            TargetRotation = new Vector2(value.x*MaxRotation.x,
                value.y*MaxRotation.y);
        }
    }

    private AirplaneStates _state;

    public AirplaneStates State
    {
        get { return _state; }
        set
        {
            if (value == AirplaneStates.Fly && MissionController.Instance.CurrentState is LandingState)
                return;
            if (_currentState != null)
                _currentState.OnDeactivate();
            _state = value;
            switch (value)
            {
                case AirplaneStates.Ride:
                    _currentState = _rideState;
                    break;
                case AirplaneStates.Fly:
                    _currentState = _flyState;
                    break;
                case AirplaneStates.Die:
                    _currentState = _dieState;
                    break;
            }
            _currentState.OnActivate();
        }
    }

    private IAirplaneState _rideState;
    private IAirplaneState _flyState;
    private IAirplaneState _dieState;
    private IAirplaneState _currentState;

    private bool _chassisEnable = true;
    private bool _chassisBusy = false;
    private bool _pause = false;

    public bool ChassisEnable
    {
        get { return _chassisEnable; }
        set
        {
            if (!_chassisBusy && State == AirplaneStates.Fly)
            {
                if (_chassisEnable && !value)
                {
                    if (State != AirplaneStates.Ride)
                        StartCoroutine(HideChassis());
                }
                if (!_chassisEnable && value && !_chassisBusy)
                    StartCoroutine(ShowChassis());
                _chassisEnable = value;
            }
        }
    }


    private IEnumerator ShowChassis()
    {
        _chassisBusy = true;
        Driver.ChassisLevel = 1;
        EventController.Instance.PostEvent("WheelsDown", gameObject);
		while (Driver.ChassisLevel > 0)
        {
            Driver.ChassisLevel -= Time.deltaTime*2;
            Driver.OnDataChanged();
            yield return new WaitForEndOfFrame();
        }
        Driver.ChassisLevel = 0;
        Driver.OnDataChanged();
//        EventController.Instance.PostEvent("WheelsDown", gameObject);
        _chassisBusy = false;
    }

    private IEnumerator HideChassis()
    {
        _chassisBusy = true;
        Driver.ChassisLevel = 0;
        EventController.Instance.PostEvent("WheelsUp", gameObject);
		while (Driver.ChassisLevel < 1)
        {
            Driver.ChassisLevel += Time.deltaTime*2;
            Driver.OnDataChanged();
            yield return new WaitForEndOfFrame();
        }
        Driver.ChassisLevel = 1;
        Driver.OnDataChanged();
//        EventController.Instance.PostEvent("WheelsUp", gameObject);
        _chassisBusy = false;
    }

    private void Awake()
    {
        _rideState = new RideState(this);
        _flyState = new FlyState(this);
        _dieState = new DieState(this);

        if (!Driver)
            Driver = GetComponent<AirplaneDriver>();

        _rideState.Awake();
        _flyState.Awake();
        _dieState.Awake();

        State = AirplaneStates.Ride;

        EventController.Instance.Subscribe("StartRocket", this);
        EventController.Instance.Subscribe("TargetingActive", this);
        EventController.Instance.Subscribe("TargetingDeactive", this);
        EventController.Instance.Subscribe("OnShowPauseMenu", this);
        EventController.Instance.Subscribe("OnResume", this);
        EventController.Instance.Subscribe("OnShowWinScreen", this);
        EventController.Instance.Subscribe("OnShowLoseScreen", this);
        EventController.Instance.Subscribe("MissionFinished", this);
    }

    private void FixedUpdate()
    {
        if (_pause)
            return;
        _currentState.FixedUpdate();

        RaycastHit hit;
        var result = Physics.Raycast(new Ray(transform.position + new Vector3(0,-0.25f,0), -Vector3.up), out hit);
            
        if (result)
        {
            _lastHeight = hit.distance * 1.28084f;
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        _currentState.OnCollisionEnter(col);
    }

    private void OnCollisionExit(Collision col)
    {
        _currentState.OnCollisionExit(col);
    }

    private void OnTriggerEnter(Collider other)
    {
        _currentState.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _currentState.OnTriggerExit(other);
    }



    #region IEventSubscriber implementation

    private bool _NavigateRocket = false;

    public void OnEvent(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "StartRocket":
                GameObject r = GameObject.Instantiate(DataStorageController.Instance.RocketPrefab) as GameObject;
                r.transform.position = Driver.RocketNubL.position;
                r.transform.rotation = Driver.RocketNubL.rotation;
                r.GetComponent<Rocket>().Speed = CurrentSpeed;
                if (_NavigateRocket) r.GetComponent<Rocket>().Target = GetMissionObject();

                r = GameObject.Instantiate(DataStorageController.Instance.RocketPrefab) as GameObject;
                r.transform.position = Driver.RocketNubR.position;
                r.transform.rotation = Driver.RocketNubR.rotation;
                r.GetComponent<Rocket>().Speed = CurrentSpeed;
                if (_NavigateRocket) r.GetComponent<Rocket>().Target = GetMissionObject();
                break;
            case "TargetingActive":
                _NavigateRocket = true;
                break;
            case "TargetingDeactive":
                _NavigateRocket = false;
                break;

            case "OnShowPauseMenu":
                GetComponent<Rigidbody>().Sleep();
                _pause = true;
                break;

            case "OnResume":
                GetComponent<Rigidbody>().WakeUp();
                _pause = false;
                break;

            case "OnShowWinScreen":
                GetComponent<Rigidbody>().isKinematic = true;
                _pause = true;
                break;

            case "OnShowLoseScreen":
                GetComponent<Rigidbody>().isKinematic = true;
                _pause = true;
                break;

            case "MissionFinished":
                GetComponent<Rigidbody>().Sleep();
                break;
        }
    }

    private MissionObject _lastMissionObject;
    public MissionObject GetMissionObject()
    {
        return BaseLevel.Instance.Target;
    }

    #endregion
}
