using UnityEngine;
using System.Collections;

public class F38 : AirplaneDriver 
{
	void Awake()
	{
		OnDataChanged += Calibration;
	}

    void Update()
    {
        if (AieleronL)
            Calibration();
    }

	void Calibration()
	{
		// Ailerons position
        AieleronL.localRotation = Quaternion.Euler (new Vector3(0,0,24.375f));
        AieleronL.Rotate (AieleronL.right, -20 * (Pitch), Space.World);
        AieleronR.localRotation = Quaternion.Euler (new Vector3(0,0,-24.375f));
        AieleronR.Rotate (AieleronR.right, -20 * (Pitch),Space.World);

		// motors PS
		foreach (GameObject ps in MotorPSs) 
		{
            Color col;
            col = ps.GetComponent<Renderer>().material.GetColor ("_TintColor");
            col.a = Speed;
            ps.GetComponent<Renderer>().material.SetColor ("_TintColor", col);
		}

		// Rudder
		MotorL.localRotation = Quaternion.Euler (Pitch * 5, 0, -Yaw * 5);
		MotorR.localRotation = Quaternion.Euler (Pitch * 5, 0, -Yaw * 5);

        foreach (Transform s in Chassis)
        {
            s.localScale = new Vector3(0.0002669258f,0.0002669258f,Mathf.Lerp(0,0.0002669258f,1-ChassisLevel));
        }
        foreach (Transform cl in ChassisCoversL)
            cl.localRotation = Quaternion.Euler(0, Mathf.Lerp(-50, 0, 1-ChassisLevel), 0);
        foreach (Transform cl in ChassisCoversR)
            cl.localRotation = Quaternion.Euler(0, Mathf.Lerp(0, 50, ChassisLevel), 0);
	}
}
