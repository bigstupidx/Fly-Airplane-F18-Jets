using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class AudioCouple
{
    public string EventName;
    public AudioClip Clip;
    public bool SFX = true;
    public bool Loop = false;
}

public class SoundController2D : MonoBehaviour, IEventSubscriber
{
    public AudioCouple[] AudioCouples;
    private AudioSource[] _sources;

    void Awake()
    {
        foreach (AudioCouple pare in AudioCouples)
            EventController.Instance.Subscribe(pare.EventName, this);
        EventController.Instance.Subscribe("OnUpdateOptions", this);
        _sources = new AudioSource[AudioCouples.Length];
        for (int i=0; i<this._sources.Length; i++)
            _sources [i] = gameObject.AddComponent<AudioSource>();
    }
    
    public void OnEvent(string EventName, GameObject Sender)
    {
        for (int i=0;i<AudioCouples.Length;i++)
        {
            if (AudioCouples[i].EventName == EventName)
            {
                AudioSource s = _sources[i];
                s.clip = AudioCouples[i].Clip;
                s.loop = AudioCouples[i].Loop;
                s.Play();
            }
        }
        if (EventName == "OnUpdateOptions")
        {
            for (int i=0;i<AudioCouples.Length;i++)
                if (AudioCouples[i].SFX)
                    _sources[i].volume = OptionsController.Instance.SFXLevel;
            else
                _sources[i].volume = OptionsController.Instance.MusicLevel;
        }
    }
}
