////////////////////////////////////////////
///                                      ///
///         RealSky Version 1.4          ///
///  Created by: Black Rain Interactive  ///
///                                      ///
//////////////////////////////////////////// 

// Super stripped down version 			  //
// Edited to be just a sky which spins 	  //

using UnityEngine;
using System.Collections;

public class RealSky : MonoBehaviour {

	public Texture dayTime;
	public float skySpeed = 0.0f;
	
    public Camera mainCamera;
    public int skyBoxLayer = 8;

    float counter = 0.0f;
    bool isPaused = false;
    GameObject skyCamera;
    private Camera _camera;
    private Camera _mainCamera;
	void Awake(){

     //   StartCoroutine("Counter");
		//StartCoroutine("SkyRotation");

        if (mainCamera == null)
            return;

        gameObject.layer = skyBoxLayer;

        skyCamera = new GameObject("SkyboxCamera");
        skyCamera.AddComponent<Camera>();
        skyCamera.GetComponent<Camera>().depth = -10;
        skyCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
        skyCamera.GetComponent<Camera>().cullingMask = 1 << skyBoxLayer;
        skyCamera.transform.position = gameObject.transform.position;
	    skyCamera.GetComponent<Camera>().farClipPlane = 20000;

        mainCamera.cullingMask = 1;
        mainCamera.clearFlags = CameraClearFlags.Nothing;

	    _camera = skyCamera.GetComponent<Camera>();

	    _mainCamera = mainCamera.GetComponent<Camera>();

	}
	
	void Start(){

		GetComponent<Renderer>().material.SetTexture("_Texture01", dayTime);

	}

    void Update(){

        if (mainCamera != null){
            skyCamera.transform.rotation = mainCamera.transform.rotation;
            if(_camera.fieldOfView != _mainCamera.fieldOfView)
            _camera.fieldOfView = _mainCamera.fieldOfView;
        }

    }

//    IEnumerator Counter(){
//
//        while (true){
//
//            if (isPaused == false){
//                counter += Time.deltaTime;
//            }
//
//            yield return null;
//
//        }
//
//    }
	
//	IEnumerator SkyRotation(){
//		
////		while (true){
////
////            transform.Rotate(Vector3.up * Time.deltaTime * skySpeed, Space.World);
////			yield return null;
////			
////		}
//	}

}