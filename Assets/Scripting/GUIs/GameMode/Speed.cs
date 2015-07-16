using UnityEngine;
using System.Collections;

public class Speed : MonoBehaviour 
{
    public Transform SmallArrow;
    public Transform BigArrow;

    void Update()
    {
        float AirSpeed = AirplaneController.Instance.CurrentSpeed;
        BigArrow.rotation = Quaternion.Euler(0, 0, -AirSpeed * 3);
        SmallArrow.rotation = Quaternion.Euler(0, 0, -AirSpeed * 3 / 10);
    }
}
