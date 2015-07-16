using UnityEngine;
using System.Collections;

public class EUFI : AirplaneDriver {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var rotation = Quaternion.Euler(new Vector3((this.Pitch) * 12.5f, 0, 0));

        if (AieleronL != null)
	    AieleronL.localRotation = rotation;
        if (AieleronR != null)
	    AieleronR.localRotation = rotation;



        Rudder.localRotation = Quaternion.Euler(new Vector3(32.91029f, Rudder.localRotation.y, -(this.Yaw) * 14.5f));

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
