using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

internal class FollowingWaypoints : State
{
    private readonly List<WayPoint> _wayPoints;
    private int _current;
	
    public FollowingWaypoints(List<WayPoint> wayPoints)
    {
        _wayPoints = wayPoints;
        if (_wayPoints.Count == 0)
        {
            Ended = true;
            return;
        }

        foreach (var point in _wayPoints)
        {
            point.gameObject.SetActive(false);
        }

        MissionStateText = string.IsNullOrEmpty(_wayPoints.First().Text)
            ? "Follow way points"
				: _wayPoints.First().Text;
	}

    public override void Start()
    {
        base.Start();
        _wayPoints.First().gameObject.SetActive(true);
    }

    public override void Update()
    {
        float distance =
            (AirplaneController.Instance.transform.position - _wayPoints[_current].transform.position).magnitude;


        if (distance < _wayPoints[_current].Radius)
        {
            _wayPoints[_current].gameObject.SetActive(false);
            _current ++;

			//MissionStateText = _wayPoints[_current].Text;
			//Debug.Log("NEXT waypoint  " + _wayPoints[_current].Text);
			Debug.Log("NEXT waypoint  " +_current);
            if (_current == _wayPoints.Count)
            {
                Ended = true;
            }
            else
            {
                _wayPoints[_current].gameObject.SetActive(true);
            }
        }
    }

    public override MissionObject GetTarget()
    {
        return _wayPoints[_current].GetComponent<MissionObject>();
    }
}