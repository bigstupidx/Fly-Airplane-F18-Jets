//#define DRIVER_DEBUG

using UnityEngine;
using System.Collections;

#if DRIVER_DEBUG
[ExecuteInEditMode]
#endif
public class F_16 : AirplaneDriver 
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
    
    void Validate()
    {
        Calibration();
    }
    
    void Calibration()
    {
        // Ailerons position
        AieleronL.localRotation = Quaternion.Euler (new Vector3(-10,270,90));
        AieleronL.Rotate (AieleronL.right, 20 * (Roll), Space.World);
        AieleronR.localRotation = Quaternion.Euler (new Vector3(-10,90,90));
        AieleronR.Rotate (AieleronR.right, -20 * (Roll),Space.World);
        
        // ROLL
        if (PitchL)
        {
            PitchL.localRotation = Quaternion.Euler(new Vector3(10, 180, 0));
            PitchL.Rotate(PitchL.right, -20*(Pitch), Space.World);
        }
        if (PitchR)
        {
            PitchR.localRotation = Quaternion.Euler(new Vector3(10, 0, 0));
            PitchR.Rotate(PitchR.right, -20*(Pitch), Space.World);
        }

#if !DRIVER_DEBUG
        // motors PS
        foreach (GameObject ps in MotorPSs) 
        {
            Color col;
            col = ps.GetComponent<Renderer>().material.GetColor ("_TintColor");
            col.a = Speed;
            ps.GetComponent<Renderer>().material.SetColor ("_TintColor", col);
        }
        #endif
        
        // Rudder
        Rudder.localRotation = Quaternion.Euler (new Vector3(0,33.3f,0));
        Rudder.Rotate (Rudder.forward, -20 * (Yaw),Space.World);
        
        
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
