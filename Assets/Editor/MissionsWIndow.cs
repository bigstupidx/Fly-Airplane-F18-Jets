using UnityEngine;
using System.Collections;
using UnityEditor;

public class MissionsWindow : EditorWindow
{
    // Add menu named "CleanUpWindow" to the Window menu 
    [MenuItem("Window/Missions")] 
    static void Init() 
    { 
        // Get existing open window or if none, make a new one: 
        MissionsWindow window = (MissionsWindow)EditorWindow.GetWindow(typeof(MissionsWindow));
        window.Show(); 
    } 

    Vector2 _scrollPos1;
    Vector2 _scrollPos2;
    bool _showIslands = true;
    bool _showBases = true;
    bool _showRunways = true;
    bool _showMissionsData = true;

    void OnGUI()
    {
        // DataStorage
        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical(GUILayout.Width(this.position.width));
        _scrollPos1 = EditorGUILayout.BeginScrollView(_scrollPos1);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("MissionObjects",GUILayout.Width(100));
        if (GUILayout.Button("Collect data",GUILayout.Width(100)))
            if (EditorUtility.DisplayDialog("Confirm...","Are you sure, baby? This will rewrite all data...","Yep","Nope"))
                DataStorageController.Instance.GetData();
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        _showIslands = EditorGUILayout.ToggleLeft("Islands", _showIslands,GUILayout.Width(200));
        _showBases = EditorGUILayout.ToggleLeft("Bases", _showBases,GUILayout.Width(200));
        _showRunways = EditorGUILayout.ToggleLeft("Runways", _showRunways,GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        if (_showIslands)
            DataStorageController.Instance.MissionIslandsID = 
                MODBlock("Islands",DataStorageController.Instance.MissionIslandsID);

        if (_showBases)
            DataStorageController.Instance.MissionBasesID = 
                MODBlock("Bases",DataStorageController.Instance.MissionBasesID);

        if (_showRunways)
            DataStorageController.Instance.MissionRunwaysID = 
                MODBlock("Runways",DataStorageController.Instance.MissionRunwaysID);

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Missions - Count ["+TransportGOController.Instance.Missions.Length+"]",GUILayout.Width(100));
        if (GUILayout.Button("Set defs.",GUILayout.Width(100)))
        {
            for (int i =0; i<TransportGOController.Instance.Missions.Length;i++)
            {
                TransportGOController.Instance.Missions[i].Targets = new MissionObjectData[5];
                TransportGOController.Instance.Missions[i].MissionTitle = "Mission "+(i+1).ToString()+" title";
                TransportGOController.Instance.Missions[i].MissionText = "Mission "+(i+1).ToString()+" text";
                TransportGOController.Instance.Missions[i].Blocked = i!=0;
                for (int j=0;j<5;j++)
                    TransportGOController.Instance.Missions[i].Targets[j] = new MissionObjectData();
            }
        }
        if (GUILayout.Button("Upd. Dist/Pay",GUILayout.Width(100)))
        {
            for (int i=0;i<TransportGOController.Instance.Missions.Length;i++)
            {
                MissionInfo info = TransportGOController.Instance.Missions[i];
                Vector3 rpos = DataStorageController.Instance.MissionRunwaysID[0].Position;
                MissionObjectData obj = info.Targets[0];
                float dist = 0;
                if (obj != null && obj.ID != -1)
                    dist = Vector3.Distance(rpos,DataStorageController.GetMissionObjectByID(obj.ID).transform.position);
                for (int j=0;j<4;j++)
                {
                    MissionObject mo = DataStorageController.GetMissionObjectByID(info.Targets[j].ID);
                    MissionObject mo2 = DataStorageController.GetMissionObjectByID(info.Targets[j+1].ID);
                    if (mo != null && mo2!= null)
                        dist+=Vector3.Distance(mo.transform.position,mo2.transform.position);
                    else
                    {
                        if (mo != null)
                            dist+= Vector3.Distance(mo.transform.position,rpos);
                        break;
                    }
                }
                info.Distance = (int)dist;
                info.Payment = (int)(dist/25);
            }
        }
        if (GUILayout.Button("Set rand targets",GUILayout.Width(100)))
        {
            for (int i=0;i<TransportGOController.Instance.Missions.Length;i++)
            {
                TransportGOController.Instance.Missions[i].Targets[0] = GetRandTarget();
                if (i>7) TransportGOController.Instance.Missions[i].Targets[1] = GetRandTarget();
                if (i>12) TransportGOController.Instance.Missions[i].Targets[2] = GetRandTarget();
                if (i>15) TransportGOController.Instance.Missions[i].Targets[3] = GetRandTarget();
            }
        }

        EditorGUILayout.EndHorizontal();
        _showMissionsData = EditorGUILayout.ToggleLeft("Show Missions", _showMissionsData,GUILayout.Width(200));

        if (_showMissionsData)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Num", GUILayout.Width(30));
            GUILayout.Label("Mission Title", GUILayout.Width(300));
            GUILayout.Label("Mission Text", GUILayout.Width(300));
            GUILayout.Label("Distance", GUILayout.Width(80));
            GUILayout.Label("Playment", GUILayout.Width(80));
            GUILayout.Label("Blocked", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            for (int i =0; i<TransportGOController.Instance.Missions.Length;i++)
            {
                EditorGUILayout.BeginHorizontal();
                MissionInfo info = TransportGOController.Instance.Missions[i];
                EditorGUILayout.LabelField((i+1).ToString(),GUILayout.Width(30));
                info.MissionTitle = EditorGUILayout.TextField(info.MissionTitle,GUILayout.Width(300));
                info.MissionText = EditorGUILayout.TextField(info.MissionText,GUILayout.Width(300));
                info.Distance = EditorGUILayout.IntField(info.Distance,GUILayout.Width(80));
                info.Payment = EditorGUILayout.IntField(info.Payment,GUILayout.Width(80));
                info.Blocked = EditorGUILayout.Toggle(info.Blocked,GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();

                if (info.Targets.Length==5)
                {
                    for (int j=0;j<5;j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Target "+(j+1).ToString(),GUILayout.Width(80));
                        info.Targets[j].Objective = EditorGUILayout.TextField(info.Targets[j].Objective,GUILayout.Width(300));
                        MissionObject mo = DataStorageController.GetMissionObjectByID(info.Targets[j].ID);
                        if (mo)
                            mo = EditorGUILayout.ObjectField(mo,typeof(MissionObject),true,GUILayout.Width(200)) as MissionObject;
                        else
                            mo = EditorGUILayout.ObjectField(null,typeof(MissionObject),true,GUILayout.Width(200)) as MissionObject;
                        if (mo != null)
                            info.Targets[j].ID = mo.ID;
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }
    
    MissionObjectData[] MODBlock(string label, MissionObjectData[] data)
    {
        EditorGUILayout.LabelField(label+" - Count ["+data.Length+"]");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("ID", GUILayout.Width(30));
        EditorGUILayout.Space();
        GUILayout.Label("Position", GUILayout.Width(200));
        EditorGUILayout.Space();
        GUILayout.Label("Name", GUILayout.Width(100));
        EditorGUILayout.Space();
        GUILayout.Label("Object", GUILayout.Width(80));
        EditorGUILayout.EndHorizontal();

        MissionObjectData[] res = data;
        for (int i = 0;i<res.Length;i++)
        {
            res[i] = MODLabel(res[i]);
        }

        return res;
    }

    MissionObjectData MODLabel(MissionObjectData mod)
    {
        MissionObjectData res = mod;
        EditorGUILayout.BeginHorizontal();
        int id = 0;
        if (int.TryParse(GUILayout.TextField(mod.ID.ToString(), GUILayout.Width(30)), out id))
            res.ID = id;
        EditorGUILayout.Space();
        Vector3 pos;
        res.Position = EditorGUILayout.Vector3Field("", mod.Position, GUILayout.Width(200));
        EditorGUILayout.Space();
        res.Name = EditorGUILayout.TextField(mod.Name, GUILayout.Width(100));
        EditorGUILayout.Space();
        MissionObject mo2 = DataStorageController.GetMissionObjectByID(mod.ID);
        if (mo2 != null)
            EditorGUILayout.ObjectField(mo2.gameObject, typeof(GameObject), true, GUILayout.Width(100));
        else
            EditorGUILayout.LabelField("None", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();
        return res;
    }

    MissionObjectData GetRandTarget()
    {
        MissionObjectData dat = DataStorageController.Instance.MissionBasesID [UnityEngine.Random.Range(0, DataStorageController.Instance.MissionBasesID.Length - 1)];
        return new MissionObjectData(dat);
    }
}
