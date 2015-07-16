using UnityEngine;
using System.Collections;

public class Compas : MonoBehaviour 
{
    public Transform SecondsArrow;
    public Transform CompasLayer;

    void Update()
    {
        SecondsArrow.rotation = Quaternion.Euler(0, 0, Time.time*6);
        CompasLayer.rotation = Quaternion.Euler(0, 0, AirplaneController.Instance.transform.rotation.eulerAngles.y);
    }
}
