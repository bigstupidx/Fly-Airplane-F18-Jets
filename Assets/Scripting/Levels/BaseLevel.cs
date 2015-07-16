using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BaseLevel : MonoBehaviour
{

    public static BaseLevel Instance { get; private set; }

    public Transform TakeOffPos;

    public MissionObject TakeOff;
    public MissionObject Landing;

    public List<WayPoint> WayPoints;
    public List<WayPoint> ReturnWayPoints;

    public List<MissionObject> Targets;

    public bool DeathOutside;

    public float Height;

    public float ReachHeight;

    private List<State> _states;
    private int _currentState;


    public void NextState()
    {
        _currentState += 1;

        if (CurrentState != null)
        {
            if (CurrentState.Ended)
            {
                NextState();
            }
            else
            {
                CurrentState.Start();
                EventController.Instance.PostEvent("MissionChangeTarget", gameObject);
            }
        }
        else
        {
            EventController.Instance.PostEvent("MissionFinished", gameObject);
        }

    }

	void Start ()
	{
	    Instance = this;

	    _states = new List<State>
	    {
	        new TakeoffState(TakeOff),
	        new FollowingWaypoints(WayPoints),
            new ReachHeightState(ReachHeight),
	        new DestroyTargetState(Targets),
	        new FollowingWaypoints(ReturnWayPoints),
	        new LandingState(Landing)
	    };

	    CurrentState.Start();

	    _Start();

        EventController.Instance.PostEvent("MissionChangeTarget", gameObject);

	}

    private bool _failed;
	void Update ()
	{

	    bool death = true;

	    if (CurrentState is FollowingWaypoints && _currentState <= 2)
	    {
	        {
	            if (DeathOutside)
	            {
	                foreach (var wayPoint in WayPoints)
	                {
	                    if (
	                        Vector3.Distance(wayPoint.transform.position,
	                            AirplaneController.Instance.transform.position) <
	                        wayPoint.SafeZoneAround)
	                    {
	                        death = false;
	                        break;
	                    }
	                }
	            }
	            else
	            {
	                death = false;
	            }
	        }
	    }
	    else
	    {
	        death = false;
	    }

	    if (_currentState <= 3)
	    {
	        if (Height != 0)
	        {
	            if (AirplaneController.Instance.Height > Height)
	            {
	                BaseBeenDestroyedText.BaseDestroyed = true;
	                BaseBeenDestroyedText.ReachedHeight = true;


	                death = true;
	            }
	        }
	    }


	    if (death)
	    {
	        if (!_failed)
	        {
	            EventController.Instance.PostEvent("MissionFailed", null);
	            Debug.Log("DEATH");
	            _failed = true;
	        }
	    }

	    _Update();
	}


    protected virtual void _Start()
    {
    }

    protected virtual void _Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();

            if (CurrentState.Ended)
            {
                NextState();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                NextState();
            }
        }
    }

    public MissionObject Target
    {
        get
        {
            if (CurrentState == null)
                return null;
            
            return CurrentState.GetTarget();
        }
    }

    public State CurrentState 
    {
        get
        {
            if (_currentState > _states.Count - 1) return null;
            return _states[_currentState];
        }
    }


}
