using System;
using System.Text;
using DN;
using UnityEngine;

public class FlyState : IAirplaneState, IEventSubscriber
{
    AirplaneController _plane;
    private Timer mUndeadTimer;

    private bool _makingSharpTurn;

    private float _sharpTurnTarget;
    private Timer _sharpTurnTimer;

    private Vector2 _planeMaxRotation;
    private Vector2 _planeAccelRotation;
    private Vector2 _planeBreakRotation;

    public FlyState(AirplaneController Controller)
    {
        _plane = Controller;
        mUndeadTimer = new Timer();
        mUndeadTimer.Duration = 0.25f;
        mUndeadTimer.OnTick += OnTick;
        mUndeadTimer.Run();

        _sharpTurnTimer = new Timer();
        _sharpTurnTimer.Duration = 1.3f;
        _sharpTurnTimer.OnTick += OnSharpTurnEnd;

        _planeMaxRotation = _plane.MaxRotation;
        _planeAccelRotation = _plane.AccelRotation;
        _planeBreakRotation = _plane.BreakRotation;

        _prevBreaking = _plane.Breaking;

        EventController.Instance.Subscribe("MakeSharpTurn", this);
    }

    private void OnSharpTurnEnd(object sender, EventArgs eventArgs)
    {
        _makingSharpTurn = false;
    }

    private void OnTick(object sender, EventArgs eventArgs)
    {
        if(_collided)
        {
            _plane.State = AirplaneStates.Die;
        }

    }

    #region IAirplaneState implementation

    private bool _collided = false;
    private bool _landing = false;
    private float _prevFlySpeed;
    private float _prevBreaking;
    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Death"))
        {
            Debug.Log("Death " + col.gameObject.name);


            _collided = true;
            if (!mUndeadTimer.IsRunning)
            {
                _plane.State = AirplaneStates.Die;
            }
        }
        if (col.gameObject.CompareTag("MissionObject"))
        {
            _plane.State = AirplaneStates.Die;
        }
        if (col.gameObject.CompareTag("Runway"))
        {
            CheckLandingDeath();
        }
    }

    private void CheckLandingDeath()
    {
        float angle = Vector3.Angle(Vector3.up, _plane.transform.up);
        if (angle < 15 && _plane.CurrentSpeed < _plane.MaxSpeed/2 &&
            _plane.ChassisEnable)
        {
        }
        else
        {
//            Debug.Log(angle);
//            Debug.Log("Current " + _plane.CurrentSpeed);
//            Debug.Log("Max " + _plane.MaxSpeed);

            _plane.State = AirplaneStates.Die;
        }
    }

    public void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Death"))
        {
            _collided = false;
        }

        if (col.gameObject.CompareTag("Runway"))
        {

        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Runway"))
        {
            _landing = true;
            _prevBreaking = _plane.Breaking;
            _plane.Breaking = 23;

            _prevFlySpeed = _plane.MinFlySpead;
            _plane.MinFlySpead = 0;

            _plane.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Runway"))
        {
            _landing = false;
            _plane.MinFlySpead = _prevFlySpeed;
            _plane.Breaking = _prevBreaking;

            _plane.GetComponent<Rigidbody>().useGravity = false;

        }
    }


    public void Awake()
    {

    }

    public void Update()
    {

    }



    public void FixedUpdate()
    {
        if (_plane.CurrentSpeed < 30)
        {
            _plane.GetComponent<Rigidbody>().useGravity = true;
        }
        else
        {
            _plane.GetComponent<Rigidbody>().useGravity = false;
        }
        if (_landing)
        {
            //_plane.CurrentSpeed = Mathf.Lerp(_plane.CurrentSpeed, 0, Time.deltaTime);
            if (_plane.CurrentSpeed < 2.5f)
            {
//                Vector3 target = _plane.transform.position + _plane.transform.forward;
//                target.y = _plane.transform.position.y;
//                _plane.transform.LookAt(target);
                EventController.Instance.PostEvent("Landing", _plane.gameObject);
                //_plane.CurrentRotation = _plane.TargetRotation;
                _plane.State = AirplaneStates.Ride;
            }
            CheckLandingDeath();
        }
        else
        {
            if (_makingSharpTurn)
            {
                _plane.Rotation = new Vector2(_plane.TurnDirection, 0);
                _plane.IsMakingTurn = true;
            }
            else
            {
                _plane.MaxRotation.x = Mathf.Lerp(_plane.MaxRotation.x, _planeMaxRotation.x, Time.fixedDeltaTime*3);
                _plane.AccelRotation.x = Mathf.Lerp(_plane.AccelRotation.x, _planeAccelRotation.x, Time.fixedDeltaTime*3);
                _plane.BreakRotation.x = Mathf.Lerp(_plane.BreakRotation.x, _planeBreakRotation.x, Time.fixedDeltaTime*3);
                _plane.IsMakingTurn = false;
            }
        }

        Lift();
        UpdateHorizontalRotation();
        UpdateVerticalRotation();
        UpdatePositionRotation();
        UpdateSpeed();
        mUndeadTimer.Update(Time.fixedDeltaTime);


        Showcase();

        _sharpTurnTimer.Update(Time.fixedDeltaTime);
    }

    private void UpdateSpeed()
    {
        float target = Mathf.Max(_plane.TargetSpeed, _plane.MinFlySpead);
        if (Mathf.Abs(target - _plane.CurrentSpeed) > 1f)
        {
            if (_plane.CurrentSpeed - target < 1)
                _plane.CurrentSpeed += _plane.Acceleration*Time.fixedDeltaTime;
            else if (_plane.CurrentSpeed - target > 1)
                _plane.CurrentSpeed -= _plane.Breaking*Time.fixedDeltaTime*(_plane.ChassisEnable ? 2 : 1);
        }
    }

    private void Lift()
    {
        Vector3 forward = _plane.transform.forward;
        if (forward.y > 0)
            forward.y *= Mathf.Clamp(_plane.CurrentSpeed - _plane.MinFlySpead, 0, 100)/100.0f;
        if (_plane.transform.position.y > 19900)
            forward.y *= Mathf.Clamp(100 - (2000 - _plane.transform.position.y), 0, 100)/100.0f;

        if (_plane.CurrentSpeed < _plane.MinFlySpead)
        {
            _plane.CurrentSpeed = Mathf.Lerp(_plane.CurrentSpeed, _plane.MinFlySpead, Time.deltaTime);
        }
        else
        {
            _plane.CurrentSpeed = Mathf.Clamp(_plane.CurrentSpeed, _plane.MinFlySpead, _plane.MaxSpeed * 3);
        }
        _plane.GetComponent<Rigidbody>().position += forward*_plane.CurrentSpeed*Time.fixedDeltaTime;
    }

    private void Showcase()
    {
// SHOWCASE
        _plane.Driver.Yaw = _plane.TargetRotation.x / _plane.MaxRotation.x;
        

        _plane.Driver.Pitch = _plane.TargetRotation.y/_plane.MaxRotation.y;
        _plane.Driver.Roll = _plane.TargetRotation.x/_plane.MaxRotation.x;
        _plane.Driver.OnDataChanged();
    }

    private void UpdatePositionRotation()
    {
        // POSITION / ROTATION
        _plane.transform.Rotate(Vector3.up, _plane.CurrentRotation.x*Time.fixedDeltaTime, Space.World);
        Vector3 rot = _plane.transform.rotation.eulerAngles;
        rot.z = (-_plane.CurrentRotation.x/_plane.MaxRotation.x)*30.0f;
        _plane.transform.rotation = Quaternion.Euler(rot);
    }

    private void UpdateVerticalRotation()
    {

        float target = _landing ? 0 : _plane.TargetRotation.y;

        _plane.CurrentRotation.y = Mathf.Lerp(_plane.CurrentRotation.y, target,
                Time.fixedDeltaTime*_plane.AccelRotation.y);
        

        Vector3 rot = _plane.transform.rotation.eulerAngles;
        rot.x = _plane.CurrentRotation.y;
        _plane.transform.rotation = Quaternion.Euler(rot);
    }

    private void UpdateHorizontalRotation()
    {

            float speed = Mathf.Abs(_plane.TargetRotation.x) < 0.1f
                ? _plane.BreakRotation.x
                : _plane.AccelRotation.x;
        float target = _plane.TargetRotation.x;
            _plane.CurrentRotation.x = Mathf.Lerp(_plane.CurrentRotation.x, target,
                Time.fixedDeltaTime*speed);

    }

    public void OnActivate()
    {
        _plane.GetComponent<Rigidbody>().useGravity = false;
    }

    public void OnDeactivate()
    {

    }

    #endregion

    public void OnEvent(string EventName, GameObject Sender)
    {
        if (EventName == "MakeSharpTurn")
        {
            if (!_makingSharpTurn)
            {
                _sharpTurnTimer.Run(true);
                _makingSharpTurn = true;

                _plane.MaxRotation = new Vector2(130, _plane.MaxRotation.y);
                _plane.AccelRotation = new Vector2(15f, _plane.AccelRotation.y);
                _plane.BreakRotation = new Vector2(15f, _plane.BreakRotation.y);

            }
        }
    }
}