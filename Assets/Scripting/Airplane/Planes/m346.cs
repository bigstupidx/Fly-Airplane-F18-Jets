using UnityEngine;
using System.Collections;

public class m346 : AirplaneDriver
{

    private void Update()
    {
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
    }
}
    