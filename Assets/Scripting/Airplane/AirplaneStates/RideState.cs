using System;
using UnityEngine;

public class RideState : IAirplaneState
{
    private AirplaneController _plane;

    public RideState(AirplaneController Controller)
    {
        _plane = Controller;
    }

    #region IAirplaneState implementation

    public void OnCollisionEnter(Collision col)
    {

    }

    public void OnCollisionExit(Collision col)
    {
        EventController.Instance.PostEvent("Takeoff", col.gameObject);
        _plane.State = AirplaneStates.Fly;
    }

    public void OnTriggerEnter(Collider other)
    {
        
    }

    public void OnTriggerExit(Collider other)
    {
    }

    public void Awake()
    {

    }

    private bool _stop = true;

    public void FixedUpdate()
    {
        if (Mathf.Abs(_plane.TargetRotation.x - _plane.CurrentRotation.x) > 0.1f)
        {
            float speed = Mathf.Abs(_plane.TargetRotation.x)<0.1f ? _plane.BreakRotation.x:
                _plane.AccelRotation.x;
            if (_plane.TargetRotation.x < _plane.CurrentRotation.x)
                _plane.CurrentRotation.x -= speed *  Time.fixedDeltaTime;
            else
                _plane.CurrentRotation.x += speed *  Time.fixedDeltaTime;
        }

        if (Mathf.Abs(_plane.CurrentRotation.x) > 0.1f)
        {
            //_plane.transform.Rotate(Vector3.up, _plane.CurrentRotation.x * Time.fixedDeltaTime, Space.World);
        }
        
        if (Mathf.Abs(_plane.TargetSpeed - _plane.CurrentSpeed) > 0.1f)
        {
            if (_plane.CurrentSpeed - _plane.TargetSpeed < 1)
                _plane.CurrentSpeed += _plane.Acceleration * Time.fixedDeltaTime;
            else if (_plane.CurrentSpeed - _plane.TargetSpeed > 1)
                _plane.CurrentSpeed -= _plane.Breaking * Time.fixedDeltaTime * MissionController.Instance.SlowdownCurve.Evaluate(_plane.CurrentSpeed);
        } else if (_plane.TargetSpeed < 1)
            _plane.TargetSpeed = Mathf.Lerp(_plane.TargetSpeed/_plane.MaxSpeed, 0, Time.fixedDeltaTime*0.1f);

        if (_plane.CurrentSpeed < 1 && _stop)
        {
            EventController.Instance.PostEvent("Stop", _plane.gameObject);
            _stop = false;
        }

        if (_plane.CurrentSpeed > 1)
        {
            _plane.GetComponent<Rigidbody>().position += _plane.transform.forward * _plane.CurrentSpeed * Time.fixedDeltaTime;
            _stop = true;
        }
        
        _plane.Driver.Yaw = _plane.TargetRotation.x / _plane.MaxRotation.x;
        _plane.Driver.OnDataChanged();
    }

    public void OnActivate()
    {
        _plane.GetComponent<Rigidbody>().isKinematic = false;
        _plane.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _plane.GetComponent<Rigidbody>().useGravity = true;
    }

    public void OnDeactivate()
    {
        _plane.CurrentRotation = Vector2.zero;
    }

    #endregion
}