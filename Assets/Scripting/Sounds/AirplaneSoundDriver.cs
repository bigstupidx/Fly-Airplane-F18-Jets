using UnityEngine;
using System.Collections;

public class AirplaneSoundDriver : MonoBehaviour, IEventSubscriber
{
    public AudioClip Ci;
    public AudioClip Bi;
    public AudioClip Acceleration;
    public AudioClip Breaking;
    public AudioClip Motor;
    public AudioClip TouchSound;

    private AudioSource _Ci;
    private AudioSource _Bi;
    private AudioSource _Motor;

    private Transform _camTransform;

    private AudioSource PrepareSource(AudioClip Clip)
    {
        AudioSource res = gameObject.AddComponent<AudioSource>();
        res.clip = Clip;
        res.loop = true;
        res.Play();
        res.rolloffMode = AudioRolloffMode.Linear;
        res.volume = 0;
        res.dopplerLevel = 0.1f;
        return res;
    }

    void Awake()
    {
        AudioListener l = GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener;
        l.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;

        _Ci = PrepareSource(Ci);
        _Bi = PrepareSource(Bi);
        _Motor = PrepareSource(Acceleration);
        _Motor.dopplerLevel = 0.0f;
        _Motor.loop = false;

        _Motor.volume = _Bi.volume = _Ci.volume = OptionsController.Instance.SFXLevel;

        if (!Camera3DInstance.Instance)
        {
            GameObject.Destroy(this);
            return;
        }
        _camTransform = Camera3DInstance.Instance.transform;

        EventController.Instance.Subscribe("Landing",this);
        EventController.Instance.Subscribe("OnShowPauseMenu", this);
        EventController.Instance.Subscribe("OnResume", this);
        EventController.Instance.Subscribe("OnPause", this);

    }

    #region IEventSubscriber implementation

    public void OnEvent(string EventName, GameObject Sender)
    {
        switch (EventName)
        {
            case "OnShowPauseMenu":
                _Ci.Pause();
                _Bi.Pause();
                _Motor.Pause();
                _pause = true;
                break;
            
            case "OnPause":
                _Ci.Pause();
                _Bi.Pause();
                _Motor.Pause();
                _pause = true;
                break;

            case "OnResume":
                _Ci.Play();
                _Bi.Play();
                _Motor.Play();
                _pause = false;
                break;

            default:
                AudioSource s = PrepareSource(TouchSound);
                s.volume = OptionsController.Instance.SFXLevel;
                s.Play();
                Destroy(s, TouchSound.length);
                break;
        }
    }

    #endregion

    private bool _pause;

    void Update()
    {
        if (_pause)
            return;

        float angle = Vector3.Angle(_camTransform.forward, transform.forward);
        angle /= 60;
        float motorVolCoof = Mathf.Clamp01((AirplaneController.Instance.CurrentSpeed - 50) / 150);
        float coof2 = Mathf.Clamp01(angle);
        float coof3 = Mathf.Clamp01((angle - 1) / (2 - 1));
        _Bi.volume = ((1 - coof2) * (1 - coof3) * motorVolCoof) * OptionsController.Instance.SFXLevel;
        _Ci.volume = (coof2 * (1 - coof3) * motorVolCoof) * OptionsController.Instance.SFXLevel;

        _Motor.volume = AirplaneController.Instance.CurrentSpeed > 5 ? OptionsController.Instance.SFXLevel*0.7f : 0;
        float cur = AirplaneController.Instance.CurrentSpeed;
        float tar = AirplaneController.Instance.TargetSpeed;
        AudioClip newClip;
        float newTime=0;
        if (tar - cur > 10)
        {
            newClip = Acceleration;
            newTime = newClip.length * (cur / AirplaneController.Instance.MaxSpeed);
        } else if (tar - cur < -10 && cur - AirplaneController.Instance.MinFlySpead > 5)
        {
            newClip = Breaking;
            newTime = newClip.length * (1 - cur / AirplaneController.Instance.MaxSpeed);
        }
        else
            newClip = Motor;
        if (_Motor.clip != newClip)
        {
            _Motor.clip = newClip;
            _Motor.time = newTime;
            _Motor.Play();
        }
        if (!_Motor.isPlaying)
        {
            _Motor.clip = Motor;
            _Motor.Play();
        }
    }
}
