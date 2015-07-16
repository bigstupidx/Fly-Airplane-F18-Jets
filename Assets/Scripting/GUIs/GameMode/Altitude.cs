using UnityEngine;
using System.Collections;

public class Altitude : MonoBehaviour 
{
    public Transform SmallArrow;
    public Transform BigArrow;
    
    void Update()
    {
        if (AirplaneController.Instance)
        {
            float altitude = AirplaneController.Instance.Height;
            BigArrow.rotation = Quaternion.Euler(0, 0, -altitude * 3 / 10);
            SmallArrow.rotation = Quaternion.Euler(0, 0, -altitude * 3 / 100);
        }
    }
}
