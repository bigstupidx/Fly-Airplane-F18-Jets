using UnityEngine;
using System.Collections;

public class Radar : MonoBehaviour 
{
    public static Radar Instance { get; private set; }
    public bool Main;

    public GameObject Point;
    public Transform CompasLayer;
    public float DistanceToTarget { get; private set; }

    public bool Local;

    private bool _inZone = false;

    private void Start()
    {
        if (Main)
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (AirplaneController.Instance)
        {
            if (Local)
            {
                CompasLayer.localRotation = Quaternion.Euler(0, 0, AirplaneController.Instance.transform.rotation.eulerAngles.y);
            }
            else
            {
                CompasLayer.rotation = Quaternion.Euler(0, 0, AirplaneController.Instance.transform.rotation.eulerAngles.y);
            }
            var missionObject = AirplaneController.Instance.GetMissionObject();
            if (missionObject == null)
            {
                return;
            }
            Vector3 pos = AirplaneController.Instance.transform.position;
            Vector3 tpos = missionObject.transform.position;//  MissionController.Instance.CurrentTarget.transform.position;
            tpos -= pos;

            DistanceToTarget = tpos.magnitude;
            if (DistanceToTarget < DataStorageController.Instance.ViewZoneDistance && !_inZone)
            {
                _inZone = true;
                EventController.Instance.PostEvent("ViewZoneEnter", missionObject.gameObject);
            } else
                if (DistanceToTarget > DataStorageController.Instance.ViewZoneDistance && _inZone)
            {
                _inZone = false;
                EventController.Instance.PostEvent("ViewZoneExit", missionObject.gameObject);
            }

            Vector3 r = AirplaneController.Instance.transform.right;
            Vector3 fw = AirplaneController.Instance.transform.forward;
            tpos.y = 0;
            r.y = 0;
            r.Normalize();
            fw.y = 0;
            fw.Normalize();
            pos.x = Vector3.Dot(r, tpos);
            pos.y = Vector3.Dot(fw, tpos);
            pos /= 250f;
            if (pos.magnitude > 0.4f)
                pos = pos.normalized * 0.4f;
            pos = new Vector3(pos.x, pos.y, 0);

            Point.transform.localPosition = pos + (Local ? new Vector3(0, 0, 7f) : new Vector3(0, 0, 5));
        }
    }

    IEnumerator Blink(GameObject P)
    {
        Color col = P.GetComponent<Renderer>().material.GetColor("_Color");

        float time = Time.time;
        while (Time.time < time + 0.1f)
        {
            col.a = (Time.time - time) * 10;
            P.GetComponent<Renderer>().material.SetColor("_Color",col);
            yield return new WaitForEndOfFrame();
        }

        col.a = 1;
        P.GetComponent<Renderer>().material.SetColor("_Color",col);

        while (Time.time < time + 3f)
        {
            col.a = 1 - (Time.time - time)/3;
            P.GetComponent<Renderer>().material.SetColor("_Color",col);
            yield return new WaitForEndOfFrame();
        }

        col.a = 0;
        P.GetComponent<Renderer>().material.SetColor("_Color",col);
    }
}
