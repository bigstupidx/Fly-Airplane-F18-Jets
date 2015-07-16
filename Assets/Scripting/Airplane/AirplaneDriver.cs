using UnityEngine;
using System.Collections;

public class AirplaneDriver : MonoBehaviour 
{
	[Range(0.0F, 1.0F)]
	public float Speed = 0;

	[Range(-1.0F, 1.0F)]
	public float Roll = 0;

	[Range(-1.0F, 1.0F)]
	public float Yaw = 0;

	[Range(-1.0F, 1.0F)]
	public float Pitch = 0;

    [Range(0.0F, 1.0F)]
    public float ChassisLevel = 0;

	public delegate void func();
	public func OnDataChanged = () => {};

	public Transform MotorL = null;
	public Transform MotorR = null;
	public Transform AieleronL = null;
	public Transform AieleronR = null;
	public Transform Rudder = null;
	public Transform Rudder2 = null;
	public Transform PitchL = null;
	public Transform PitchR = null;
    public Transform RocketNubL;
    public Transform RocketNubR;


    public Transform InsideView;

    public Transform InsideViewPosition;

    public Transform[] Insides;

    public Transform[] Chassis;
    public Transform[] ChassisCoversL;
    public Transform[] ChassisCoversR;

    public GameObject[] MotorPSs;

}
