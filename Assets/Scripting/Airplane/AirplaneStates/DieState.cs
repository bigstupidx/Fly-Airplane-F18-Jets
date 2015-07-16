using System;
using UnityEngine;

public class DieState : IAirplaneState
{
    AirplaneController _plane;

    public DieState(AirplaneController Controller)
    {
        _plane = Controller;
    }

    #region IAirplaneState implementation

    public void OnCollisionEnter(Collision col)
    {

    }

    public void OnCollisionExit(Collision col)
    {

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

    public void FixedUpdate()
    {

    }

    private void PreapareToDie(Transform target)
    {
        target.parent = null;
        target.gameObject.AddComponent<Rigidbody>();
        Vector3 way = _plane.transform.forward;
        way.y *= -1;
        way.Normalize();
        way += UnityEngine.Random.insideUnitSphere;
        target.GetComponent<Rigidbody>().velocity = way * _plane.CurrentSpeed/5;

        GameObject.Destroy(target.gameObject, 20);
    }

    public void OnActivate()
    {
        EventController.Instance.PostEvent("Crash", _plane.gameObject);
        int i, j;
        for (i=0;i<_plane.transform.childCount;i++)
        {
            for (j=0;j<_plane.transform.GetChild(i).childCount;j++)
                PreapareToDie(_plane.transform.GetChild(i).GetChild(j));
            PreapareToDie(_plane.transform.GetChild(i));
        }

        GameObject ps = GameObject.Instantiate(DataStorageController.Instance.PlaneDeathPS) as GameObject;
        ps.transform.position = _plane.transform.position;

    }

    public void OnDeactivate()
    {

    }

    #endregion
}