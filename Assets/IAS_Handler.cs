// i6 IAS Handler [Updated 7th July 2015]
// Attach this to your IAS advert Game Objects
// Contact sean@i6.com for help and information
// Note: This is the MeshRenderer version of this script!

/* Change Log:
 * [Contact me or check our developers document if you need information on past changes!]
 *
 * 23rd April 2015
 * - Click event is now linked with the internal IAS_Manager analytics
 */

using UnityEngine;
using System.Collections;

public class IAS_Handler : MonoBehaviour
{      
	public int bannerID;                            // Screen ID
	public bool backscreen_ad = false;  // Is this a backscreen advert?
	
	private bool textureSet = false;
	private float LastUpdated = 0f;
	
	void Start()
	{
		if(!textureSet)
			LoadTexture();
	}
	
	void Update()
	{
		if(!textureSet)
			LoadTexture();
		
		if(LastUpdated <= IAS_Manager.Instance.MainLastUpdated && !backscreen_ad){
			LastUpdated = Time.time;
			textureSet = false;
		} else if(LastUpdated <= IAS_Manager.Instance.BackscreenLastUpdated && backscreen_ad){
			LastUpdated = Time.time;
			textureSet = false;
		}
		
	}
	
	private void LoadTexture()
	{
		// Don't try display an IAS advert until it is ready!
		if((backscreen_ad && !IAS_Manager.Instance.Backscreen_IASReady) || !backscreen_ad && !IAS_Manager.Instance.Main_IASReady)
			return;
		
		if(IAS_Manager.Instance.GetAdTexture(bannerID, backscreen_ad) != null)
		{
			MeshRenderer SelfMeshRenderer = GetComponent<MeshRenderer>();
			SelfMeshRenderer.material.mainTexture = IAS_Manager.Instance.GetAdTexture(bannerID, backscreen_ad);
			
			textureSet = true;
			LastUpdated = Time.time;
		}
	}
	
	void OnMouseUp()
	{
		// If an IAS advert is not ready then it can't be clicked
		if((backscreen_ad && !IAS_Manager.Instance.Backscreen_IASReady) || !backscreen_ad && !IAS_Manager.Instance.Main_IASReady)
			return;
		
		string url = IAS_Manager.Instance.GetAdURL(bannerID, backscreen_ad);
		
		IAS_Manager.Instance.IAS_Log.LogEvent("IAS Clicks", IAS_Manager.Instance.BundleID + " " + (backscreen_ad ? "(backscreen)" : "(main)"), url.Replace("https://play.google.com/store/apps/details?id=", ""));
		
		if(url != "")
		{
			Application.OpenURL(url);
		}
	}
	
	
}