//#define DRIVER_DEBUG

using UnityEngine;
using System.Collections;

#if DRIVER_DEBUG
[ExecuteInEditMode]
#endif
public class FA_22 : AirplaneDriver 
{
    public Transform RudderL;
    public Transform RudderR;

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
        AieleronL.localRotation = Quaternion.Euler (new Vector3(3,180,-15));
        AieleronL.Rotate (AieleronL.up, 20 * (Roll), Space.World);
        AieleronR.localRotation = Quaternion.Euler (new Vector3(3,0,-15));
        AieleronR.Rotate (AieleronR.up, -20 * (Roll),Space.World);
        
        // ROLL
        PitchL.localRotation = Quaternion.Euler (new Vector3(10,0,0));
        PitchL.Rotate (PitchL.up, 10 * (Pitch), Space.World);
        PitchR.localRotation = Quaternion.Euler (new Vector3(10,0,0));
        PitchR.Rotate (PitchR.up, 10 * (Pitch), Space.World);
        
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
        RudderL.localRotation = Quaternion.Euler (new Vector3(-25,-27,10));
        RudderL.Rotate (RudderL.forward, -20 * (Yaw),Space.World);
        RudderR.localRotation = Quaternion.Euler (new Vector3(-25,-207,10));
        RudderR.Rotate (RudderR.forward, 20 * (Yaw),Space.World);
        
        
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
