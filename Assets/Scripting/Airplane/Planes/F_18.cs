using UnityEngine;

public class F_18 : AirplaneDriver
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (PitchL != null)
		{
        PitchL.localRotation = Quaternion.Euler(new Vector3((Pitch)*5.5f, 0, 0));
        PitchR.localRotation = Quaternion.Euler(new Vector3((Pitch) * 5.5f, 0, 0));
		}

        // motors PS
        foreach (GameObject ps in MotorPSs)
        {
            Color col;
            col = ps.GetComponent<Renderer>().material.GetColor("_TintColor");
            col.a = Speed;
            ps.GetComponent<Renderer>().material.SetColor("_TintColor", col);
        }


        foreach (Transform s in Chassis)
        {
            s.localScale = new Vector3(0.0002669258f, 0.0002669258f, Mathf.Lerp(0, 0.0002669258f, 1 - ChassisLevel));
        }
        foreach (Transform cl in ChassisCoversL)
            cl.localRotation = Quaternion.Euler(0, Mathf.Lerp(-50, 0, 1 - ChassisLevel), 0);
        foreach (Transform cl in ChassisCoversR)
            cl.localRotation = Quaternion.Euler(0, Mathf.Lerp(0, 50, ChassisLevel), 0);
	}
}
