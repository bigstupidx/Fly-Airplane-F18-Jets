using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MilitaryDemo;

public class WeatherControl : MonoBehaviour
{
    public GameObject AttachTo;
    public Light MainLight;

    public GameObject SunnyPrefab;
    public GameObject RainyPrefab;
    public GameObject VeryRainyPrefab;
    public GameObject SnowyPrefab;

    public List<WeatherData> Data;

    public Renderer Sky;

    public static Dictionary<int, TWeather> WeatherForLevel = new Dictionary<int, TWeather>()
    {
        {1, TWeather.Sunny},
        {2, TWeather.Sunny},
        {3, TWeather.Sunny},
        {4, TWeather.Rainy},
        {5, TWeather.Sunny},
        {6, TWeather.Rainy},
        {7, TWeather.Sunny},
        {8, TWeather.Rainy},
        {9, TWeather.VeryRainy},
        {10, TWeather.Sunny},
        {11, TWeather.VeryRainy},
        {12, TWeather.Rainy},
        {13, TWeather.Rainy},
        {14, TWeather.Sunny},
        {15, TWeather.VeryRainy},
        {16, TWeather.VeryRainy},
        {17, TWeather.Sunny},
        {18, TWeather.VeryRainy},
        {19, TWeather.VeryRainy},
        {20, TWeather.VeryRainy},
        {21, TWeather.Rainy},
        {22, TWeather.VeryRainy},
    };

    public static readonly Dictionary<TWeather, string> WheatherToName = new Dictionary<TWeather, string>()
    {
        {TWeather.Sunny, "Sunny"},
        {TWeather.Rainy, "Rain"},
        {TWeather.VeryRainy, "Heavy rain"},
        {TWeather.Snowy, "Snow"},
    };

    public static string GetWeatherNameForLevel(int level)
    {
        return WheatherToName[WeatherForLevel[level + 1]];
    }

    void Start ()
    {
        var data = Data.Find(p =>p.Type == WeatherForLevel[TransportGOController.Instance.SelectedMissionID + 1]);
	    var weatherObject = CreateWeatherEffect(data.Type);

	    if (weatherObject != null)
	    {
	        weatherObject.transform.parent = AttachTo.transform;
	        weatherObject.transform.localPosition = data.Positon;
	    }
//	    MainLight.color = data.LightColor;
//	    MainLight.intensity = data.Intensity;
//
//        Sky.sharedMaterial.SetFloat("_Blend", data.Skyblend);
	}

    private GameObject CreateWeatherEffect(TWeather weatherType)
    {
        GameObject prefab = null;
        switch (weatherType)
        {
            case TWeather.Sunny:
                prefab = SunnyPrefab;
                break;
            case TWeather.Rainy:
                prefab = RainyPrefab;

                break;
            case TWeather.VeryRainy:
                prefab = VeryRainyPrefab;

                break;
            case TWeather.Snowy:
                prefab = SnowyPrefab;
                break;
            default:
                throw new ArgumentOutOfRangeException("weatherType");
        }

        if (prefab == null)
        {
            return null;
        }

        return GameObject.Instantiate(prefab) as GameObject;
    }

    // Update is called once per frame
	void Update () {
	
	}

    public enum TWeather
    {
        Sunny,
        Rainy,
        VeryRainy,  
        Snowy
    }

    [Serializable]
    public class WeatherData
    {
        public float Intensity;
        public Color LightColor;
        public TWeather Type;

        public float Skyblend;

        public Vector3 Positon;
    }
}
