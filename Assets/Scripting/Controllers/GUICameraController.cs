using UnityEngine;
using System.Collections;

public class GUICameraController : MonoBehaviour 
{
    public static GUICameraController Instance { get; private set; }

    public GUICameraController()
    {
        Instance = this;
    }
}
