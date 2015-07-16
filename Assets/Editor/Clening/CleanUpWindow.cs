using UnityEngine; 
using UnityEditor; 
using System.Collections; 
using System.Collections.Generic;
using System.IO;
using System; 

public class CleanUpWindow : EditorWindow 
{ 
    bool groupEnabled = false; 
    List<string> usedAssets = new List<string>(); 
    List<KeyValuePair<string, UnityEngine.Object>> usedObjects = new List<KeyValuePair<string, UnityEngine.Object>>();
    List<string> includedDependencies = new List<string>(); 
    private Vector2 scrollPos; 
    private bool needToBuild = false; 
    
    // Add menu named "CleanUpWindow" to the Window menu 
    [MenuItem("Window/CleanUpWindow")] 
    static void Init() 
    { 
        // Get existing open window or if none, make a new one: 
        CleanUpWindow window = (CleanUpWindow)EditorWindow.GetWindow(typeof(CleanUpWindow));
        window.Show(); 
    } 
    
    void OnGUI() 
    { 
        if (needToBuild) 
        { 
            GUI.color = Color.red; 
            GUILayout.Label("Are you sure you remembered to build project? Because you really need to...", EditorStyles.boldLabel); 
        } 
        GUI.color = Color.white; 
        
        if (GUILayout.Button("Load EditorLog")) 
        { 
            loadEditorLog(); 
        } 
        
        
        if(!needToBuild) 
        { 
            /*
            EditorGUILayout.BeginHorizontal(); 
            EditorGUILayout.BeginVertical(); 
            if (groupEnabled) 
            { 
                GUILayout.Label("DEPENDENCIES"); 
                for (int i = 0; i < includedDependencies.Count; i++) 
                { 
                    EditorGUILayout.LabelField(i.ToString()+"  "+includedDependencies[i],GUILayout.Width(this.position.width)); 
                } 
            } 
            EditorGUILayout.EndVertical(); 

            EditorGUILayout.EndHorizontal(); */
            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos); 
            EditorGUILayout.BeginVertical(); 

            if (groupEnabled)
            {
                float alltexsize = 0;
                foreach (var obj in usedObjects)
                {
                    TextureImporter ti = AssetImporter.GetAtPath(obj.Key) as TextureImporter;
                    float size = 0;
                    EditorGUILayout.BeginHorizontal();
                    if (ti != null)
                    {
                        int w=0,h=0;
                        Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath(obj.Key, typeof(Texture2D));
                        w = t.width;
                        h = t.height;
                        //Debug.Log(string.Format("Image [{0}], size ({1},{2})",obj.Key,w,h)); 
                        size = w*h;
                        /*
                         * 
                         * !!! For Android only !!!
                            Compression Memory consumption
                            RGB Compressed DXT1 0.5 bpp (bytes/pixel)
                            RGBA Compressed DXT5    1 bpp
                            RGB Compressed ETC1 0.5 bpp
                            RGB Compressed PVRTC 2 bits 0.25 bpp (bytes/pixel)
                            RGBA Compressed PVRTC 2 bits    0.25 bpp
                            RGB Compressed PVRTC 4 bits 0.5 bpp
                            RGBA Compressed PVRTC 4 bits    0.5 bpp
                            RGB 16bit   2 bpp
                            RGB 24bit   3 bpp
                            Alpha 8bit  1 bpp
                            RGBA 16bit  2 bpp
                            RGBA 32bit  4 bpp
                          */
                        switch (t.format)
                        {
                            case TextureFormat.ETC2_RGB:
                                size*= 0.5f;
                                break;
                            case TextureFormat.ETC_RGB4:
                                size*= 0.5f;
                                break;
                            case TextureFormat.DXT1:
                                size *= 0.5f;
                                break;
                            case TextureFormat.DXT5:
                                size *= 1f;
                                break;
                            case TextureFormat.RGB24:
                                size *=3f;
                                break;
                            case TextureFormat.Alpha8:
                                size *=1f;
                                break;
                            case TextureFormat.RGBA32:
                                size *=4f;
                                break;
                            case TextureFormat.ARGB32:
                                size *=4f;
                                break;
                            case TextureFormat.PVRTC_RGB2:
                                size *=0.25f;
                                break;
                            case TextureFormat.PVRTC_RGBA2:
                                size *=0.25f;
                                break;
                            case TextureFormat.PVRTC_RGB4:
                                size *=0.5f;
                                break;
                            case TextureFormat.PVRTC_RGBA4:
                                size *=0.5f;
                                break;
                            case TextureFormat.RGBA4444:
                                size *= 2;
                                break;
                            case TextureFormat.ARGB4444:
                                size *= 2;
                                break;
                        }
                        if (ti.mipmapEnabled) size*=1.33f;
                        size/=1024.0f;
                        //EditorGUILayout.LabelField(t.format.ToString(),GUILayout.Width(100));
                        if (GUILayout.Button("256",GUILayout.Width(40)))
                        {
                            ti.maxTextureSize=256;
                            AssetDatabase.ImportAsset( ti.assetPath, ImportAssetOptions.ForceUpdate ); 
                        }
                        if (GUILayout.Button("512",GUILayout.Width(40)))
                        {
                            ti.maxTextureSize=512;
                            AssetDatabase.ImportAsset( ti.assetPath, ImportAssetOptions.ForceUpdate ); 
                        }
                    }
                    EditorGUILayout.LabelField(size.ToString("#.## kb"),GUILayout.Width(100));
                    alltexsize +=size;
                    EditorGUILayout.ObjectField(obj.Value,typeof(UnityEngine.Object),true);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.LabelField(alltexsize.ToString("#.## kb"));
            }
            EditorGUILayout.EndVertical(); 
            
            EditorGUILayout.EndScrollView(); 
            EditorGUILayout.EndHorizontal(); 
        } 
        
    } 
    
    private void loadEditorLog() 
    { 
        UsedAssets.GetLists(ref usedAssets, ref includedDependencies); 
        
        if (usedAssets.Count == 0) 
        { 
            needToBuild = true; 
        } 
        else 
        { 
            foreach (string s in usedAssets)
            {
                //if (assets.Contains(s))
                {
                    UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(s,typeof(UnityEngine.Object));
                    if (obj != null)
                        usedObjects.Add(new KeyValuePair<string, UnityEngine.Object>(s,obj));
                }
            }

            groupEnabled = true; 
            needToBuild = false; 
        } 
    } 
    
    private string getArrangedPos(UnityEngine.Object value) 
    { 
        string path = AssetDatabase.GetAssetPath(value).ToLower(); 
        
        if (path.Contains("/plugins/")) 
        { 
            return "plugins"; 
        } 
        else if (path.Contains("/editor/")) 
        { 
            return "editor"; 
        } 
        else 
        { 
            return "some other folder"; 
        } 
    } 
}

public class UsedAssets
{
    public static string[] GetAllAssets()
    {
        string[] tmpAssets1 = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        string[] tmpAssets2 = Array.FindAll(tmpAssets1, name => !name.EndsWith(".meta"));
        string[] allAssets;
        
        allAssets = Array.FindAll(tmpAssets2, name => !name.EndsWith(".unity"));
        
        for (int i = 0; i < allAssets.Length; i++)
        {
            allAssets[i] = allAssets[i].Substring(allAssets[i].IndexOf("/Assets") + 1);
            allAssets[i] = allAssets[i].Replace(@"\", "/");
        }
        
        return allAssets;
    }
    
    public static void GetLists(ref List<string> assetResult, ref List<string> dependencyResult)
    {
        assetResult.Clear();
        dependencyResult.Clear();
        
        string LocalAppData = string.Empty;
        string UnityEditorLogfile = string.Empty;
        
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            UnityEditorLogfile = LocalAppData + "\\Unity\\Editor\\Editor.log";
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            UnityEditorLogfile = LocalAppData + "/Library/Logs/Unity/Editor.log";
        }
        
        try
        {
            // Have to use FileStream to get around sharing violations!
            FileStream FS = new FileStream(UnityEditorLogfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader SR = new StreamReader(FS);
            
            
            string line;
            while (!SR.EndOfStream && !(line = SR.ReadLine()).Contains("Mono dependencies included in the build"));
            while (!SR.EndOfStream && (line = SR.ReadLine()) != "")
            {
                dependencyResult.Add(line);
            }
            while (!SR.EndOfStream && !(line = SR.ReadLine()).Contains("Used Assets,"));
            while (!SR.EndOfStream && (line = SR.ReadLine()) != "")
            {
                
                line = line.Substring(line.IndexOf("% ") + 2);
                assetResult.Add(line);
            }
        }
        catch (Exception E)
        {
            Debug.LogError("Error: " + E);
        }
    }
}