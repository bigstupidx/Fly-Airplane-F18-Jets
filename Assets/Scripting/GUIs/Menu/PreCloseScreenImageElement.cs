using System;
using UnityEngine;
using System.Collections;

public class PreCloseScreenImageElement : MonoBehaviour 
{
	public MeshRenderer Image;
    public Button GuiObject;

    [NonSerialized]
    public bool Placed;

    [NonSerialized]
    public string Url;
}
