// i6 IAS_Manager.cs [Updated 8th July 2015]
// Attach this script to a persistent Game Object
// Contact sean@i6.com for help and information

/* Change Log:
 * [Contact me or check our developers document if you need information on past changes!]
 *
 * 23rd April 2015
 * - Added image caching to file
 * - Old images expire after 30 days
 * - Backscreen ads are now random
 * - ads for i6 applications already installed on the device will no longer have ads shown
 * - Internal global analytics system added (for tracking IAS errors across games in 1 central Google Analytics panel)
 *
 * 28th April 2015
 * - Fixed a File:// issue with older Unity versions
 * - Fixed an issue where if a cached ias ad was corrupt the script would just throw an error instead of deleting it
 *
 * 8th May 2015
 * - Added bool to toggle local caching
 * - Added bool to toggle debug log outputs
 * - Added better error handling
 *
 * 11th May 2015
 * - Added a wait for end of frame yield before IAS screens are reported as ready
 * - Added some more debug logging outputs for debug mode
 *
 * 15th May 2015
 * - Removed JSON parsing per refresh due to heavy performance impacts
 *
 * 8th June 2015
 * - Interactions with JarLoader.cs added
 *
 * 26th June 2015
 * - Fixed a bug where the backscreen would also use ads from other slots if you had too many games installed (Lines 435 - 440)
 */

using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System.IO;
using System;

// Storage class for the main IAS advert IDs for selection
[System.Serializable]
public class IAS_Grouping
{
	public int ScreenID { get; set; }
	
	private List<string> _SlotIDs = new List<string>();
	public List<string> SlotIDs
	{
		get { return _SlotIDs; }
		set { _SlotIDs = value; }
	}
}

// Global analytics logging to a single account so we can check stats for all IAS related actions in one area
public class IAS_Analytics
{
	string SWH, DID, PID, BID, AVR, SLA;
	
	public IAS_Analytics()
	{
		SWH = Screen.width + "x" + Screen.height;
		DID = WWW.EscapeURL(SystemInfo.deviceUniqueIdentifier);
		
		PID = WWW.EscapeURL("UA-56263344-38"); // Do not change this ID! It's the global IAS logging ID
		BID = WWW.EscapeURL(IAS_Manager.Instance.BundleID);
		AVR = WWW.EscapeURL(IAS_Manager.Instance.AppVersion);
		
		SLA = Application.systemLanguage.ToString().ToLower();
	}
	
	public void LogEvent(string C, string A, string L = default(string), int V = default(int))
	{
		C = WWW.EscapeURL(C);
		A = WWW.EscapeURL(A);
		L = WWW.EscapeURL(L);
		
		string LogURL = string.Empty;
		LogURL += "http://www.google-analytics.com/collect?v=1";
		LogURL += "&ul=" + SLA;         // Language
		LogURL += "&t=event";           // Log type
		LogURL += "&sr=" + SWH;         // Screen resolution
		LogURL += "&an=" + BID;         // App name
		LogURL += "&tid=" + PID;        // Property ID
		LogURL += "&aid=" + BID;        // App identifier (bundle ID)
		LogURL += "&cid=" + DID;        // Device UID
		LogURL += "&_u=.sB";            // Unknown? Some kind of ID unique to each page??
		LogURL += "&av=" + AVR;         // App version
		LogURL += "&_v=ma1b3";          // Unknown
		LogURL += "&ec=" + C;           // Category
		LogURL += "&ea=" + A;           // Action
		LogURL += "&ev=" + V;           // Value
		LogURL += "&el=" + L;           // Label
		LogURL += "&qt=2500";           // Unknown
		LogURL += "&z=185";                     // Unknown
		
		// Process the URL
		IAS_Manager.Instance.ProcessRequest(LogURL);
	}
	
	public void LogError(string D, bool F = false)
	{
		D = WWW.EscapeURL(D);
		
		string LogURL = string.Empty;
		LogURL += "http://www.google-analytics.com/collect?v=1";
		LogURL += "&ul=" + SLA;         // Language
		LogURL += "&t=exception";       // Log type
		LogURL += "&sr=" + SWH;         // Screen resolution
		LogURL += "&an=" + BID;         // App name
		LogURL += "&tid=" + PID;        // Property ID
		LogURL += "&aid=" + BID;        // App identifier (bundle ID)
		LogURL += "&cid=" + DID;        // Device UID
		LogURL += "&_u=.sB";            // Unknown? Some kind of ID unique to each page??
		LogURL += "&av=" + AVR;         // App version
		LogURL += "&_v=ma1b3";          // Unknown
		LogURL += "&exd=" + D;          // Description
		LogURL += "&exf=" + (F?1:0);// Is the error fatal?
		LogURL += "&qt=2500";           // Unknown
		LogURL += "&z=185";                     // Unknown
		
		// Process the URL
		IAS_Manager.Instance.ProcessRequest(LogURL);
	}
}

public class IAS_Manager : MonoBehaviour
{      
	public static IAS_Manager Instance;
	
	// Note: You should probably replace references to BundleID with a reference to your bundle ID from another script
	public string BundleID = "com.i6.GameNameHere";
	public string AppVersion = "1.00";
	
	private IAS_Analytics _IAS_Log;
	public IAS_Analytics IAS_Log { get { return _IAS_Log; } private set { _IAS_Log = value; } }
	
	// Dictionaries used for storing cache data (See the NeedToDownload function)
	private Dictionary<string, int> CachedURLs = new Dictionary<string, int>();
	private Dictionary<string, Texture> CachedTextures = new Dictionary<string, Texture>();
	
	// Main IAS variables (Set your IAS Ad URL from the inspector of the object which this script is attached to)
	private bool _Main_IASReady = false;
	public bool Main_IASReady { get { return _Main_IASReady; } private set { _Main_IASReady = value; } }
	
	public List<int> includedScreenIDs = new List<int>(); // Ability to limit the screen IDs being downloded for this game
	
	// Main IAS JSON URL
	public string IAS_AdURL = "http://ias.i6.com/ad/6.json";
	
	// Backscreen IAS JSON URL
	public string IAS_AdURL_Backscreen = "http://ias.i6.com/ad/6.json";
	
	// List storage for main banner information
	private List<string> Main_BannerURLs = new List<string>();
	private List<string> Main_BannerImageURLs = new List<string>();
	private List<Texture> Main_BannerTextures = new List<Texture>();
	
	private bool _Backscreen_IASReady = false;
	public bool Backscreen_IASReady { get { return _Backscreen_IASReady; } private set { _Backscreen_IASReady = value; } }
	
	// How many ads are displayed on the backscreen (limits how many are loaded)
	private int Backscreen_AdLimit = 3;
	
	// List storage for backscreen banner information
	private List<string> Backscreen_BannerURLs = new List<string>();
	private List<string> Backscreen_BannerImageURLs = new List<string>();
	private List<Texture> Backscreen_BannerTextures = new List<Texture>();
	
	// List of installed i6 apps used for filtering which ads we give the player
	private List<string> _InstalledApps = new List<string>();
	public List<string> InstalledApps { get { return _InstalledApps; } set { _InstalledApps = value; } }
	
	private float _MainLastUpdated = 0f;
	public float MainLastUpdated { get { return _MainLastUpdated; } private set { _MainLastUpdated = value; } }
	
	private float _BackscreenLastUpdated = 0f;
	public float BackscreenLastUpdated { get { return _BackscreenLastUpdated; } private set { _BackscreenLastUpdated = value; } }
	
	private bool UseCaching = true;
	private bool DebugLogging = false;
	
	// This has to be processed within IAS_Manager due to limitations of Coroutines
	public void ProcessRequest(string URL) { StartCoroutine(ProcessRequest(new WWW(URL))); }
	private IEnumerator ProcessRequest(WWW www)
	{
		yield return www; // Wait for the request to complete
		www.Dispose(); // Cleanup the WWW data
	}
	
	void Awake()
	{
		DebugLog("IAS Manager is awake!");
		
		if(!Instance){
			Instance = this;
			IAS_Log = new IAS_Analytics();
		} else {
			Destroy(gameObject);
			DebugLog("Another IAS Manager tried to create an instance but one already exists!");
			return;
		}
		
		#if UNITY_EDITOR
		if(BundleID == "com.i6.GameNameHere")
			Debug.Log ("Warning! You have not yet set the BundleID for the IAS_Manager, make sure to match this with your game Bundle Identifier (Package Name)");
		
		if(!Instance)
			Debug.Log ("Warning! The IAS Manager Instance was null!");
		#else
		if(BundleID == "com.i6.GameNameHere")
			IAS_Log.LogError("IAS bundle identifier not set in an APK build!", false);
		
		if(!Instance)
			IAS_Log.LogError("IAS Manager Instance was null!");
		#endif
	}
	
	void Start()
	{
		// Get a list of installed packages on the device and store package names of all i6 apps
		UpdateInstalledPackages();
		
		// Fetch the main banners
		StartCoroutine(FetchMainBanners());
		
		// Fetch the Backscreen banners
		StartCoroutine(FetchBackscreenBanners());
		
		IAS_Log.LogEvent("Total i6 games installed", InstalledApps.Count.ToString());
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.G))
			ResetMainBanners();
	}
	
	private void DebugLog(string Message)
	{
		if(!DebugLogging)
			return;
		
		// Prepend IASLOG to the debug output to make it easier to filter in logcat
		Debug.Log ("IASLOG " + Message);
	}
	
	// This is called after the IAS requests have completed (Main and Backscreen are separate so this is called twice on startup)
	private void OnIASComplete()
	{
		DebugLog("A set of IAS ads has completed loading!");
		
		if(Main_IASReady && Backscreen_IASReady && !CacheCleanedThisSession){
			// Cleanup the data folder, this will delete all old IAS images no longer being used
			// it deletes files older than 30 day old (only if there's an active internet connection and IAS downloads succeeded)
			// If an IAS ad hasn't been displayed within the past 30 days we can safely assume it's not being used
			CleanOldCacheData();
		}
	}
	
	private void UpdateInstalledPackages()
	{
		DebugLog("Updating Installed packages!");
		
		InstalledApps.Clear();
		
		string PackageList = JarLoader.GetPackageList("com.i6.");
		
		// Cleanup the package list mistakes (ends with a comma, remove the spaces)
		if(!string.IsNullOrEmpty(PackageList)){
			PackageList = PackageList.Trim(); // Trim whitespaces
			PackageList = PackageList.Remove(PackageList.Length-1); // Remove the final comma
			
			// Split the comma separated list into a string array
			string[] PackageArray = PackageList.Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
			
			if(PackageArray.Length > 0){
				// Extract all packages and store them in the InstalledApps list
				foreach(string PackageName in PackageArray)
					InstalledApps.Add(PackageName.Trim());
			} else {
				DebugLog("Installed package array length was 0");
				IAS_Log.LogError("Installed package array length was 0");
			}
		} else {
			DebugLog("Installed package list was null or empty!");
			IAS_Log.LogError("Installed package list was null or empty!");
		}
	}
	
	private bool CacheCleanedThisSession = false;
	
	private void CleanOldCacheData()
	{
		DebugLog("Cleaning old cache data!");
		
		// Set this true now so if any errors occur here they won't be ran into again this session
		CacheCleanedThisSession = true;
		
		if(string.IsNullOrEmpty(Application.persistentDataPath)){
			DebugLog("Application data path was null or empty!");
			IAS_Log.LogError("Application data path was null or empty!");
			return;
		}
		
		string DataPath = Application.persistentDataPath + "/";
		DirectoryInfo DataDir = new DirectoryInfo(DataPath);
		FileInfo[] FileInfo = DataDir.GetFiles();
		long NowTicks = System.DateTime.Now.Ticks;
		
		if(FileInfo.Length > 0){
			foreach(FileInfo CurFile in FileInfo)
			{
				if(NowTicks >= CurFile.LastWriteTime.Ticks + 864000000000L * 30L){
					CurFile.Delete();
				}
			}
		} else {
			DebugLog("No files found in data directory! (Probably a clean install)");
		}
	}
	
	public void ResetBackscreenBanners()
	{
		if(!Backscreen_IASReady)
			return;
		
		DebugLog("Resetting backscreen banners");
		
		// Mark the Backscreen IAS ads as not ready whilst re-downloading the data
		Backscreen_IASReady = false;
		
		// Clear the Backscreen IAS lists
		Backscreen_BannerURLs.Clear();
		Backscreen_BannerImageURLs.Clear();
		Backscreen_BannerTextures.Clear();
		
		// Fetch the Backscreen banners
		StartCoroutine(FetchBackscreenBanners());
	}
	
	public void ResetMainBanners()
	{
		if(!Main_IASReady)
			return;
		
		DebugLog("Resetting main banners");
		
		// Mark the main IAS ads as not ready whilst re-downloading the data
		Main_IASReady = false;
		
		// Clear the main IAS lists
		Main_BannerURLs.Clear ();
		Main_BannerImageURLs.Clear ();
		Main_BannerTextures.Clear ();
		
		// Fetch the main banners
		StartCoroutine(FetchMainBanners());
	}
	
	public string GetAdURL(int bannerIndex, bool isBackscreen = false)
	{
		DebugLog("GetAdURL requested! bannerIndex:" + bannerIndex + ", isBackscreen:" + isBackscreen);
		DebugLog("GetAdURL return URL: " + (!isBackscreen ? Main_BannerURLs[bannerIndex - 1] : Backscreen_BannerURLs[bannerIndex - 1]));
		
		return (!isBackscreen ? Main_BannerURLs[bannerIndex - 1] : Backscreen_BannerURLs[bannerIndex - 1]);
	}
	
	public Texture GetAdTexture(int bannerIndex, bool isBackscreen = false)
	{
		DebugLog("GetAdTexture requested! bannerIndex:" + bannerIndex + ", isBackscreen:" + isBackscreen);
		
		return (!isBackscreen ? Main_BannerTextures[bannerIndex - 1] : Backscreen_BannerTextures[bannerIndex - 1]);
	}
	
	private void SetCachedTexture(string URL, int TimeStamp, Texture DLTexture)
	{
		// Update the CachedTextures dictionary with the newly downloaded texture
		CachedTextures[URL] = DLTexture;
		
		// Update the CachedURLs dictionary for the new texture
		CachedURLs[URL] = TimeStamp;
	}
	
	private bool NeedToDownload(string URL, int TimeStamp)
	{
		if(CachedURLs.ContainsKey(URL)){
			if(CachedURLs.ContainsValue(TimeStamp)){
				DebugLog("Don't need to re-download " + URL + " because we have it stored in a list and the timestamp is up to date!");
				// We have the URL cached and it's upto date
				return false;
			} else {
				DebugLog("We need to re-download " + URL + " because it's out of date!");
				// We have the URL cached, however it's outdated
				return true;
			}
		} else {
			DebugLog("We need to download " + URL + " because we haven't cached it yet!");
			// We don't have the URL cached at all
			return true;
		}
	}
	
	private JSONNode BackscreenRootNode;
	
	// SkipInstalledCheck is true when the player has all the advertised games already installed
	private IEnumerator FetchBackscreenBanners(bool SkipInstalledCheck = false)
	{
		DebugLog("Fetching backscreen banners.." + (SkipInstalledCheck ? "(This is a re-run to include apps already installed)" : ""));
		
		// If we haven't parsed the JSON data then do it now
		if(BackscreenRootNode == null){
			// Request JSON data from the URL (We read this URL everytime)
			WWW wwwJSON = new WWW(IAS_AdURL_Backscreen);
			
			// Wait for the JSON data to be collected from the URL
			yield return wwwJSON;
			
			if (!string.IsNullOrEmpty(wwwJSON.error)){
				IAS_Log.LogEvent("IAS JSON Download error (Backscreen)", wwwJSON.error);
				return false;
			}
			
			// Parse the JSON data we just read into usable data
			BackscreenRootNode = JSON.Parse (wwwJSON.text);
			
			// Dispose of the JSON data
			wwwJSON.Dispose();
		}
		
		// Loop for each "slot" in the JSON data
		foreach(JSONNode node in BackscreenRootNode["slots"].AsArray)
		{
			// We need to work with the nodes as strings so just set them to local variables
			string sloturl = node["adurl"];
			string slotimg = node["imgurl"];
			string slotID = node["slotid"];
			int slotVal = int.Parse(slotID[slotID.Length-2].ToString());
			
			// Only use ads from slot 1 on the backscreen!
			if(slotVal != 1)
				continue;
			
			// If the current ad is an advert for this game then skip it
			// Note: You should probably replace BundleID with a reference to your bundle ID from another script
			if(sloturl.Contains (BundleID))
				continue;
			
			bool SkipCurIteration = false;
			
			if(!SkipInstalledCheck){
				foreach(string InstalledApp in InstalledApps)
					if(sloturl.Contains (InstalledApp)) SkipCurIteration = true;
			}
			
			// Make sure we don't add duplicates to the backscreen list..
			foreach(string PendingURLs in Backscreen_BannerURLs)
				if(sloturl.Contains(PendingURLs.Split("=".ToCharArray())[1])) SkipCurIteration = true;
			
			if(SkipCurIteration)
				continue;
			
			// Add the advert URL as an item to the list
			Backscreen_BannerURLs.Add(sloturl);
			
			// Set the ad image URL to a new item in the list
			Backscreen_BannerImageURLs.Add(slotimg);
		}
		
		// Oh no, the player has all of our games installed! Try again and this time include installed games
		if(!SkipInstalledCheck && Backscreen_BannerURLs.Count < Backscreen_AdLimit){
			DebugLog("Less than " + Backscreen_AdLimit + " games in slot installed (Backscreen)");
			IAS_Log.LogEvent("Less than " + Backscreen_AdLimit + " games in slot installed (Backscreen)", IAS_AdURL);
			StartCoroutine(FetchBackscreenBanners(true));
			return false;
		}
		
		// Shuffle the order of the backscreen lists
		for(int i=0;i < Backscreen_BannerURLs.Count;i++)
		{
			string TempStoreURLs = Backscreen_BannerURLs[i];
			string TempStoreIMGs = Backscreen_BannerImageURLs[i];
			
			int RandIndex = UnityEngine.Random.Range(i, Backscreen_BannerURLs.Count);
			
			Backscreen_BannerURLs[i] = Backscreen_BannerURLs[RandIndex];
			Backscreen_BannerURLs[RandIndex] = TempStoreURLs;
			
			Backscreen_BannerImageURLs[i] = Backscreen_BannerImageURLs[RandIndex];
			Backscreen_BannerImageURLs[RandIndex] = TempStoreIMGs;
		}
		
		// Download the images
		for(int i=0; i < Backscreen_AdLimit; i++)
		{
			// Split the URL into parts using ? as a delimiter
			// We need to set this to an array variable so we can check the length before doing anything with it or it'll break the whole script
			string[] URLParts = Backscreen_BannerImageURLs[i].Split("?".ToCharArray(), System.StringSplitOptions.None);
			
			// Create an int for the timestamp (Separate due to use of TryParse)
			int IASTimeStamp = 0;
			
			// Hopefully if we're in the future and the IAS was changed then not too much was changed about it
			// If there is atleast 2 parts in the URL then take the second part and hope it's the timestamp
			// Incase it's not and Don changed everything then TryParse will always default to 0 and not throw any errors)
			// Plus if in the future the timestamp was removed from the URL completely then we'll just use 0 as the actual value which effectively caches everything this session
			int.TryParse(URLParts.Length >= 2 ? URLParts[1] : "0", out IASTimeStamp);
			
			// Store the downloaded texture in this variable once downloaded or loaded from the cache
			Texture ReadyIASTexture;
			
			// Check if we need to download this IAS banner or if we already have an upto date version stored in the CachedTextures dictionary
			if(NeedToDownload(URLParts[0], IASTimeStamp)){
				
				DebugLog("Re-downloading (backscreen) " + URLParts[0]);
				
				string Filename = Path.GetFileNameWithoutExtension(Backscreen_BannerImageURLs[i].Split("?".ToCharArray())[0]);
				string Filetype = Path.GetExtension(Backscreen_BannerImageURLs[i].Split("?".ToCharArray())[0]);
				string FullFilename = Filename + "_" + IASTimeStamp + Filetype;
				string LocalName = (Application.persistentDataPath + "/" + FullFilename);
				bool NeedToRewrite = false;
				bool FileExists = (File.Exists(LocalName));
				
				DebugLog("Filename: " + Filename);
				DebugLog("Filetype: " + Filetype);
				DebugLog("FullFilename: " + FullFilename);
				DebugLog ("LocalName: " + LocalName);
				
				WWW wwwImage = default(WWW);
				
				if(FileExists && UseCaching){
					DebugLog("A local file was found and caching is enabled!");
					
					wwwImage = new WWW("file:///" + LocalName);
				} else {
					DebugLog("Re-downloading banner from URL: " + Backscreen_BannerImageURLs[i]);
					
					// Request the banner texture from the full URL including timestamp
					wwwImage = new WWW(Backscreen_BannerImageURLs[i]);
					NeedToRewrite = true;
				}
				
				// Wait for the image data to be downloaded
				yield return wwwImage;
				
				if (!string.IsNullOrEmpty(wwwImage.error)){
					DebugLog("IAS Image (Backscreen) Error: " + wwwImage.error);
					IAS_Log.LogEvent("IAS Image Download error (Backscreen)", wwwImage.error, (NeedToRewrite ? "Downloaded" : "Local"));
					
					// This was a locally grabbed file, it must be corrupt!
					if(!NeedToRewrite){
						DebugLog("IAS Image (Backscreen) is corrupt and will be deleted!");
						File.Delete(LocalName);
						
						// Lets retry this iteration..
						i--; continue;
					}
				}
				
				if(NeedToRewrite){
					DebugLog("IAS Image (Backscreen) WritingAllBytes!");
					
					// Rewrite the file (This also updates the write time to now
					File.WriteAllBytes(LocalName, wwwImage.bytes);
				} else {
					DebugLog("IAS Image (Backscreen) SettingLastWriteTime!");
					
					// Don't bother rewriting the file, just update the write time to now
					if(FileExists){
						try
						{
							File.SetLastWriteTime(LocalName, System.DateTime.Now);
						}
						catch (IOException e)
						{
							DebugLog("IAS Image (Backscreen) File IO Exception with set last write time: " + e.Message);
						}
					} else {
						DebugLog("IAS Image (Backscreen) File did not exist, could not set last write time!");
					}
				}
				
				// Set ReadyIASTexture to the newly downloaded texture
				ReadyIASTexture = wwwImage.texture;
				
				// Update the cache dictionaries
				SetCachedTexture(URLParts[0], IASTimeStamp, ReadyIASTexture);
				
				// Dispose of the downloaded data, we don't need it anymore
				wwwImage.Dispose();
				
			} else {
				
				// Set the ReadyIASTexture to the texture in the CachedTexture dictionary
				ReadyIASTexture = CachedTextures[URLParts[0]];
				
			}
			
			// Set the texture
			Backscreen_BannerTextures.Add(ReadyIASTexture);
			
		}
		
		yield return new WaitForEndOfFrame();
		
		DebugLog("Backscreen IAS marked as ready!");
		
		Backscreen_IASReady = true;
		BackscreenLastUpdated = Time.time;
		
		OnIASComplete();
	}
	
	private JSONNode MainRootNode;
	
	// SkipInstalledCheck contains bools on whether each slot should skip the installed check
	private IEnumerator FetchMainBanners(List<bool> SkipInstalledCheck = default(List<bool>))
	{
		if(SkipInstalledCheck == null)
			SkipInstalledCheck = new List<bool>();
		
		DebugLog("Fetching main banners.." + (SkipInstalledCheck.Count > 0 ? "(This is a re-run to include apps already installed)" : ""));
		
		if(MainRootNode == null){
			// Request JSON data from the URL
			WWW wwwJSON = new WWW(IAS_AdURL);
			
			// Wait for the JSON data to be collected from the URL
			yield return wwwJSON;
			
			if (!string.IsNullOrEmpty(wwwJSON.error)){
				IAS_Log.LogEvent("IAS JSON Download error (Main)", wwwJSON.error);
				return false;
			}
			
			// Parse the JSON data we just read into usable data
			MainRootNode = JSON.Parse (wwwJSON.text);
			
			// This method of sorting the JSON data isn't the cleanest but it works
			// We need to order the JSON data starting from screen 1 and counting up
			// Let us know if you can improve this code block
			// Note: We would prefer not to use and heavy sorting methods as performance is important
			string sortedJSON = "";
			
			// Wrap the sorted JSON within the slots array item
			sortedJSON += "{\"slots\":[";
			
			for(int i = MainRootNode["slots"].Count;i >= 0;i--)
			{
				if(MainRootNode["slots"].AsArray[i] != null)
				{
					sortedJSON += (MainRootNode["slots"].AsArray[i].ToString());
				}
			}
			
			// Close the slots wrapper
			sortedJSON += "]}";
			
			// Replace the rootNode with the new sortedJSON data
			MainRootNode = JSON.Parse (sortedJSON);
			
			// Dipose of the JSON data
			wwwJSON.Dispose ();
		}
		
		// IAS_Grouping is a custom class defined at the top of this script
		// The class returns the values ScreenID (int) and SlotIDs (List<string>)
		List<IAS_Grouping> IAS_SlotGroups = new List<IAS_Grouping>();
		
		// Local int to store the previous slot value so we can compare if it has changed each iteration
		int prevSlotVal = 0;
		
		// List used to store the generated random slotIDs based from the available slots
		List<int> curSlotID = new List<int>();
		
		// Iterate through all slots from the JSON data
		foreach(JSONNode node in MainRootNode["slots"].AsArray)
		{
			string slotID = node["slotid"];
			string slotURL = node["adurl"];
			string slotChar = slotID[slotID.Length-1].ToString();
			int slotVal = int.Parse(slotID[slotID.Length-2].ToString());
			
			// Check if we have iterated onto a new screen ID
			if(slotVal != prevSlotVal)
			{
				// Add the IAS Grouping class item to the list as a new item
				IAS_SlotGroups.Add (new IAS_Grouping());
				
				// Set the stored screen ID for this list item and set the previous slot value too
				IAS_SlotGroups[slotVal-1].ScreenID = slotVal;
				
				// Set the previous slot value so we don't add this screen ID again
				prevSlotVal = slotVal;
			}
			
			// Skip any adverts which advertise its self and display the next ad instead
			if(slotURL.Contains (BundleID))
				continue;
			
			bool SkipCurIteration = false;
			
			if(SkipInstalledCheck.Count <= 0){
				foreach(string InstalledApp in InstalledApps)
					if(slotURL.Contains (InstalledApp))
						SkipCurIteration = true;
			} else {
				if(!SkipInstalledCheck[slotVal-1]){
					foreach(string InstalledApp in InstalledApps)
						if(slotURL.Contains (InstalledApp))
							SkipCurIteration = true;
				}
			}
			
			if(SkipCurIteration){
				continue;
			}
			
			// Set the current slot ID inside the current screen ID list
			IAS_SlotGroups[slotVal-1].SlotIDs.Add(slotChar);
		}
		
		bool NeedToReRun = false;
		
		// Ensure all the screen IDs have atleast 1 advert each
		foreach(IAS_Grouping screenSlotIDs in IAS_SlotGroups)
		{
			if(screenSlotIDs.SlotIDs.Count <= 0){
				SkipInstalledCheck.Add(true);
				NeedToReRun = true;
				DebugLog("All games in slot installed (Main) Screen ID: " + screenSlotIDs.ScreenID);
				IAS_Log.LogEvent("All games in slot installed (Main)", IAS_AdURL, "Screen ID " + screenSlotIDs.ScreenID);
			} else {
				SkipInstalledCheck.Add(false);
			}
		}
		
		if(NeedToReRun){
			StartCoroutine(FetchMainBanners(SkipInstalledCheck));
			return false;
		}
		
		int curSlotCount = 0;
		prevSlotVal = 0;
		
		// Loop for each "slot" in the JSON data
		foreach(JSONNode node in MainRootNode["slots"].AsArray)
		{
			// We need to work with the nodes as strings so assign them to local variables
			string slotURL = node["adurl"];
			string slotIMG = node["imgurl"];
			string slotID = node["slotid"];
			int screenSlot = int.Parse(slotID[slotID.Length-2].ToString());
			
			// Skip any adverts which advertise its self and display the next ad instead
			if(slotURL.Contains (BundleID))
				continue;
			
			bool SkipCurIteration = false;
			
			if(SkipInstalledCheck.Count <= 0){
				foreach(string InstalledApp in InstalledApps)
					if(slotURL.Contains (InstalledApp))
						SkipCurIteration = true;
			} else {
				if(!SkipInstalledCheck[screenSlot-1]){
					foreach(string InstalledApp in InstalledApps)
						if(slotURL.Contains (InstalledApp))
							SkipCurIteration = true;
				}
			}
			
			if(SkipCurIteration)
				continue;
			
			if(screenSlot != prevSlotVal){
				// Reset the slot count because we've moved to the next screen ID
				curSlotCount = 0;
				
				// Load information about the current slot id
				curSlotID.Add(PlayerPrefs.GetInt("IAS_ADSlot_" + (screenSlot - 1), 0));
				
				// Increase the curSlotID if it's lower than the max, else reset it to advert slot 1
				if(curSlotID[screenSlot - 1] + 1 < IAS_SlotGroups[screenSlot-1].SlotIDs.Count){
					// Increase the slot ID for the current screen
					curSlotID[screenSlot - 1]++;
				} else {
					// Set the slot ID for the current screen back to 1
					curSlotID[screenSlot - 1] = 0;
				}
				
				PlayerPrefs.SetInt("IAS_ADSlot_" + (screenSlot - 1), curSlotID[screenSlot - 1]);
				
				prevSlotVal = screenSlot;
			} else {
				// Increase the cur slot count
				curSlotCount++;
			}
			
			// Does the current slotID iteration match the randomly generated slotID for this screenID?
			if(curSlotCount == curSlotID[screenSlot - 1])
			{
				// Add the advert URL as an item to the list
				Main_BannerURLs.Add (slotURL);
				
				// Set the ad image URL to a new item in the list
				Main_BannerImageURLs.Add (slotIMG);                            
			}
		}
		
		// Download the images
		for(int i = 0; i < Main_BannerURLs.Count; i++)
		{
			Main_BannerTextures.Add (new Texture());
			
			// Limit the screen IDs being used for this game
			if(!includedScreenIDs.Contains(i+1) && includedScreenIDs.Count > 0)
				continue;
			
			// Split the URL into parts using ? as a delimiter
			// We need to set this to an array variable so we can check the length before doing anything with it or it'll break the whole script
			string[] URLParts = Main_BannerImageURLs[i].Split("?".ToCharArray(), System.StringSplitOptions.None);
			
			// Create an int for the timestamp (Separate due to use of TryParse)
			int IASTimeStamp = 0;
			
			// Hopefully if we're in the future and the IAS was changed then not too much was changed about it
			// If there is atleast 2 parts in the URL then take the second part and hope it's the timestamp
			// Incase it's not and Don changed everything then TryParse will always default to 0 and not throw any errors)
			// Plus if in the future the timestamp was removed from the URL completely then we'll just use 0 as the actual value which effectively caches everything this session
			int.TryParse(URLParts.Length >= 2 ? URLParts[1] : "0", out IASTimeStamp);
			
			// Store the downloaded texture in this variable once downloaded or loaded from the cache
			Texture ReadyIASTexture;
			
			// Check if we need to download this IAS banner or if we already have an upto date version stored in the CachedTextures dictionary
			if(NeedToDownload(URLParts[0], IASTimeStamp)){
				
				DebugLog("Re-downloading (main) " + URLParts[0]);
				
				string Filename = Path.GetFileNameWithoutExtension(Main_BannerImageURLs[i].Split("?".ToCharArray())[0]);
				string Filetype = Path.GetExtension(Main_BannerImageURLs[i].Split("?".ToCharArray())[0]);
				string FullFilename = Filename + "_" + IASTimeStamp + Filetype;
				string LocalName = (Application.persistentDataPath + "/" + FullFilename);
				bool NeedToRewrite = false;
				bool FileExists = (File.Exists(LocalName));
				
				DebugLog("Filename: " + Filename);
				DebugLog("Filetype: " + Filetype);
				DebugLog("FullFilename: " + FullFilename);
				DebugLog ("LocalName: " + LocalName);
				
				WWW wwwImage = default(WWW);
				
				if(FileExists && UseCaching){
					DebugLog("A local file was found and caching is enabled! (Main)");
					
					wwwImage = new WWW("file:///" + LocalName);
				} else {
					DebugLog("Re-downloading (main) banner from URL: " + Main_BannerImageURLs[i]);
					
					// Request the banner texture from the full URL including timestamp
					wwwImage = new WWW(Main_BannerImageURLs[i]);
					NeedToRewrite = true;
				}
				
				// Wait for the image data to be downloaded
				yield return wwwImage;
				
				if (!string.IsNullOrEmpty(wwwImage.error)){
					DebugLog("IAS Image (Main) Error: " + wwwImage.error);
					IAS_Log.LogEvent("IAS Image Download error (Main)", wwwImage.error, (NeedToRewrite ? "Downloaded" : "Local"));
					
					// This was a locally grabbed file, it must be corrupt!
					if(!NeedToRewrite){
						DebugLog("IAS Image (Main) is corrupt and will be deleted!");
						File.Delete(LocalName);
						
						// Lets retry this iteration..
						i--; continue;
					}
				}
				
				// Store the image to cache
				if(NeedToRewrite){
					DebugLog("IAS Image (Main) WritingAllBytes!");
					
					// Rewrite the file (This also updates the write time to now
					File.WriteAllBytes(LocalName, wwwImage.bytes);
				} else {
					DebugLog("IAS Image (Main) SettingLastWriteTime!");
					
					// Don't bother rewriting the file, just update the write time to now
					if(FileExists){
						try
						{
							File.SetLastWriteTime(LocalName, System.DateTime.Now);
						}
						catch (IOException e)
						{
							DebugLog("IAS Image (Main) File IO Exception with set last write time: " + e.Message);
						}
					} else {
						DebugLog("IAS Image (Main) File did not exist, could not set last write time!");
					}
				}
				
				// Set the ReadyIASTexture to the newly downloaded texture
				ReadyIASTexture = wwwImage.texture;
				
				// Update the cache dictionaries
				SetCachedTexture(URLParts[0], IASTimeStamp, ReadyIASTexture);
				
				// Dispose of the downloaded data, we don't need it anymore
				wwwImage.Dispose();
				
			} else {
				
				// Set the ReadyIASTexture to the texture in the CachedTexture dictionary
				ReadyIASTexture = CachedTextures[URLParts[0]];
				
			}
			
			// Add the texture to the list
			Main_BannerTextures[i] = (ReadyIASTexture);
			
		}
		
		yield return new WaitForEndOfFrame();
		
		DebugLog("Main IAS marked as ready!");
		
		// Set the IASReady variable to true once all the ads are ready
		Main_IASReady = true;
		MainLastUpdated = Time.time;
		
		OnIASComplete();
	}
}