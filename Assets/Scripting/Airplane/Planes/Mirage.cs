//#define DRIVER_DEBUG

using UnityEngine;
using System.Collections;

#if DRIVER_DEBUG
[ExecuteInEditMode]
#endif
public class Mirage : AirplaneDriver 
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
        AieleronL.localRotation = Quaternion.Euler (new Vector3(0,270,93));
        AieleronL.Rotate (AieleronL.right, -20 * (Roll), Space.World);
        AieleronR.localRotation = Quaternion.Euler (new Vector3(0,90,93));
        AieleronR.Rotate (AieleronR.right, 20 * (Roll),Space.World);
        
        // ROLL
        PitchL.localRotation = Quaternion.Euler (new Vector3(0,270,90));
        PitchL.Rotate (PitchL.right, 20 * (Pitch), Space.World);
        PitchR.localRotation = Quaternion.Euler (new Vector3(0,90,90));
        PitchR.Rotate (PitchR.right, 20 * (Pitch), Space.World);
        
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
        Rudder.localRotation = Quaternion.Euler (new Vector3(0,-56,90));
        Rudder.Rotate (AieleronR.up, 20 * (Yaw),Space.Self);
        
        
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
