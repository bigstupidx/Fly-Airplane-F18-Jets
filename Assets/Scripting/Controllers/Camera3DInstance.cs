using UnityEngine;
using System.Collections;

public class Camera3DInstance : MonoBehaviour 
{
    public static Camera3DInstance Instance { get; private set; }

    public Camera3DInstance()
    {
        Instance = this;
    }
}
