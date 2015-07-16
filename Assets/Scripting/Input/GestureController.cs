using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureController : MonoBehaviour {

    Vector2 _pos;
    float _time;
    bool _pressed;
    Gesture _lg;
    List<Gesture> g;

    public int CountOfGestures 
    {
        get
        {
            if (g==null)
                return 1;
            else
                return g.Count;
        }
    }

    public delegate void func(Gesture g);
    public delegate void Ffunc ();
    public func OnGestureStart;
    public func OnGestureEnd;
    private Ffunc FUpdate;
    private void idle(Gesture g) { }

    void OnDestroy()
    {
        OnGestureEnd = idle;
        OnGestureStart = idle;
        foreach (Gesture gest in g)
            gest.OnAddingTouch = idle;
    }

    public static GestureController Instance { get; private set; }

    public GestureController()
    {
        Instance = this;
        _pressed = false;
        g = new List<Gesture>();
        OnGestureEnd = idle;
        OnGestureStart = idle;
    }

    private MouseTouch GetDeltaTouch()
    {
        MouseTouch touch = new MouseTouch();
        touch.position = Input.mousePosition;
        touch.deltaPosition = touch.position-_pos;
        touch.Time = Time.time;
        touch.deltaTime = touch.Time - _time;
        touch.phase = TouchPhase.Moved;
        touch.buttonID = 0;
        _time = touch.Time;
        _pos = touch.position;
        return touch;
    }

    private Vector2 v3tov2 (Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    private MouseTouch GetTouch(UnityEngine.Touch t)
    {
        MouseTouch touch = new MouseTouch();
        touch.position = t.position;
        touch.deltaPosition = t.deltaPosition;
        touch.deltaTime = t.deltaTime;
        touch.Time = Time.time;
        touch.phase = t.phase;
        touch.buttonID = t.fingerId;
        return touch;
    }

    void FixedMouseUpdate()
    {
        if (_pressed)
        if (v3tov2(Input.mousePosition) != _pos)
            _lg.AddTouch(GetDeltaTouch());

        if (Input.GetMouseButton(0) && !_pressed)
        {
            _pressed = true;
            _pos = Input.mousePosition;
            _time = Time.time;
            // on start gesture
            _lg = new Gesture();
            MouseTouch touch = new MouseTouch();
            touch.position = _pos;
            touch.deltaTime = 0;
            touch.deltaPosition = Vector2.zero;
            touch.phase = TouchPhase.Began;
            touch.buttonID = 0;
            touch.Time = Time.time;
            _lg.AddTouch(touch);
            OnGestureStart(_lg);
        }
        if (!Input.GetMouseButton(0) && _pressed)
        {
            _pressed = false;
            // on finish gesture
            MouseTouch touch = GetDeltaTouch();
            touch.phase = TouchPhase.Ended;
            _lg.AddTouch(touch);
            OnGestureEnd(_lg);
        }
    }

    public Gesture GetGByID(int id)
    {
        foreach (Gesture t in g)
            if (t.ID == id)
                return t;
        return null;
    }

    void FixedTouchUpdate()
    {
        Gesture temp;
        foreach (UnityEngine.Touch t in Input.touches)
        {
            temp = GetGByID(t.fingerId);
            if (t.phase == TouchPhase.Canceled || t.phase == TouchPhase.Ended)
            {
                if (temp != null)
                {
                    temp.AddTouch(GetTouch(t));
                    OnGestureEnd((Gesture)temp);
                    g.Remove(temp);
                }
            }
            else
            {
                if (temp != null)
                    temp.AddTouch(GetTouch(t));
                else
                {
                    temp = new Gesture();
                    temp.AddTouch(GetTouch(t));
                    g.Add(temp);
                    OnGestureStart(temp);
                }
            } 
        }
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.Android)
            FUpdate = FixedTouchUpdate;
        else
            FUpdate = FixedMouseUpdate;
        Input.multiTouchEnabled = true;
    }

    void Update()//Change it to FIXED UPDATE if you have any bugs with gesture detection!!!
    {
        FUpdate();
    }
}

public class MouseTouch
{
    public Vector2 position { get; set; }
    public Vector2 deltaPosition { get; set; }
    public TouchPhase phase { get; set; }
    public float Time { get; set; }
    public float deltaTime { get; set; }
    public int buttonID { get; set; }
}

public class Gesture
{
    public delegate void func(Gesture g);

    public func OnAddingTouch;

    private void onaddingtouch(Gesture g){}

    public List<MouseTouch> Frames { get; set; }

    public int FramesCount { get { return Frames.Count; } }

    public int ID { get { return Frames.Count > 0 ? Frames[0].buttonID : -1; } }

    public Vector2 StartPoint { get { return Frames.Count > 0 ? Frames[0].position : Vector2.zero; } }
    public Vector2 EndPoint { get { return Frames.Count > 0 ? Frames[Frames.Count-1].position : Vector2.zero; } }
    public Vector2 CenterPoint
    {
        get
        {
            Vector2 res = Frames[0].position;
            for (int i = 1; i < Frames.Count; i++)
                res += Frames[i].position;
            res /= Frames.Count;
            return res;
        }
    }

    private float Angle(Vector2 point)
    {
        /*
         *      270
         *       |
         *   0 --|-- 180
         *       |
         *       90
         */
        return Mathf.Atan2(point.y, point.x) * (180 / Mathf.PI) + 180;
    }

    public float FirsLastAngle()
    {
        return Angle(EndPoint - StartPoint);
    }

    public int TurnsCount(float minAngle,int countOfFrames)
    {
        if (Frames.Count == 0)
            return 0;
        int res = 0;

        //float delta = Angle(Frames[0].deltaPosition);
        //Debug.Log(Angle(Frames[1].deltaPosition, Frames[0].deltaPosition));

        for (int i = 1; i < Frames.Count-countOfFrames-1; i++)
        {
            for (int j = 0; j < countOfFrames; j++)
            {
                if (//Frames[i].deltaPosition != Frames[i + j].deltaPosition &&
                    Frames[i].phase == TouchPhase.Moved &&
                    Frames[i + j].phase == TouchPhase.Moved)
                {
                    if (Vector2.Angle(Frames[i].deltaPosition, Frames[i + j].deltaPosition) > minAngle)
                    {
                        res++;
                        i += countOfFrames;
                        break;
                    }
                }
            }
        }

        return res;
    }

    public string Code
    {
        get
        {
            string res = "";
            foreach (MouseTouch t in Frames)
                if (t.deltaPosition.sqrMagnitude != 0)
                {
                    float dat = Angle(t.deltaPosition) + 210;
                    if (dat > 180)
                        dat -= 180;
                    dat /= 60;
                    res += (int)dat;
                }
            return res;
        }
    }

    public Gesture()
    {
        Frames = new List<MouseTouch>();
        OnAddingTouch = onaddingtouch;
    }

    public void AddTouch(MouseTouch touch)
    {
        Frames.Add(touch);
        OnAddingTouch(this);
    }

    public float GestureTime 
    {
        get
        {
            float d = 0;
            foreach (MouseTouch t in Frames)
                d += t.deltaTime;
            return d;
        }
    }

    public float Distance
    {
        get
        {
            float l = 0;
            foreach (MouseTouch t in Frames)
                l += t.deltaPosition.magnitude;
            return l;
        }
    }

    // can throw 'OutOfRangeException'....
    public MouseTouch FirstTouch { get { return Frames[0]; } }
    public MouseTouch LastTouch { get { return Frames[Frames.Count - 1]; } }
}

