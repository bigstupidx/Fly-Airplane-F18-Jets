using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour, IEventSubscriber
{
	public AirplaneController Plane;

    public Rect VertPosition;

    public Rect NavigateTexturePosition;
    public Texture NavigateButtonTexture;

    public Rect ChassiTexturePosition;
    public Texture ChassiButtonTexture;

    private Vector2 _navigate;
    private Vector2 _relNav = Vector2.zero;
    private bool _navigatePress=false;
    private int _navID;

    private GestureController _g;

    void Awake()
    {
        _g = gameObject.AddComponent<GestureController>();
        _g.OnGestureStart += OnGestureStart;
        _g.OnGestureEnd += OnGestureEnd;
        Input.multiTouchEnabled = true;
    }

    void Start()
    {
        Plane = AirplaneController.Instance;
        Input.gyro.enabled = true;
        EventController.Instance.SubscribeToAllEvents(this);
    }

    private Transform _lever;
    private float _leverLevel = -0.45f;
    private int _leverID = -1;

    private Transform _navButton;
    private Vector3 _navFirstPosition;

    private float _lastX;
    private float _lastY;

    void OnGestureStart(Gesture g)
    {
        Ray ray = GUICameraController.Instance.GetComponent<Camera>().ScreenPointToRay(g.StartPoint);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit))
        {
            switch(hit.collider.gameObject.name)
            {
                case "Lever":
                    _lever = hit.collider.gameObject.transform;
                    _leverID = g.ID;
                    g.OnAddingTouch += OnGestureStay;
                    break;
                case "Button_Chassi":
                    Plane.ChassisEnable = !Plane.ChassisEnable;
                    break;
                case "Shoot":
                    EventController.Instance.PostEvent("AttackButtonPressed",gameObject);
                    break;
                case "ChangeView":
                    CameraController.Instance.ChangeCamera();
                    break;
                case "NavigationButton":
                    _navigatePress = true;
                    _navID = g.ID;
                    g.OnAddingTouch += OnGestureStay;
                    _navigate = g.StartPoint;
                    _relNav = Vector2.zero;
                    _navButton = hit.collider.gameObject.transform;
                    _navFirstPosition = _navButton.localPosition;
                    break;
                default:
                    EventController.Instance.PostEvent("OnPressObject", hit.collider.gameObject);
                    break;
            }
        }
        else
            EventController.Instance.PostEvent("OnPressObject",null);
    }

    void OnGestureStay(Gesture g)
    {
        if (g.ID == _leverID)
        {
            Ray ray = GUICameraController.Instance.GetComponent<Camera>().ScreenPointToRay(g.EndPoint);
            Vector3 pos = _lever.localPosition;
            _leverLevel = Mathf.Clamp(_lever.parent.InverseTransformPoint(ray.origin).y,-0.45f,0.4f);
            pos.y = _leverLevel;
            _lever.localPosition = pos;
        } else
        {
            _relNav = g.EndPoint - _navigate;
            if (OptionsController.Instance.InvertAxisY)
                _relNav.y *= -1;

            Ray ray = GUICameraController.Instance.GetComponent<Camera>().ScreenPointToRay(g.EndPoint);
            Vector3 pos = _navButton.parent.InverseTransformPoint(ray.origin);
            pos.z = 5;
            pos = _navFirstPosition - pos;

            if (pos.x < 0.5f && pos.x > -0.5f)
                _relNav.x = 0;

            if (pos.magnitude < 1)
                _navButton.localPosition = _navFirstPosition - pos;
            else
                _navButton.localPosition = _navFirstPosition - pos.normalized;
        }
    }

    void OnGestureEnd(Gesture g)
    {
        if (g.ID == _leverID)
        {
            _leverID = -1;
            //_lever.localPosition = new Vector3(0.1f,0,-1);
        } else
        if (g.ID == _navID && _navigatePress)
        {
            _navigatePress = false;
            _relNav = Vector2.zero;
            _navButton.localPosition = _navFirstPosition;
        }

        Ray ray = GUICameraController.Instance.GetComponent<Camera>().ScreenPointToRay(g.EndPoint);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
            EventController.Instance.PostEvent("OnReleaseObject",hit.collider.gameObject);
        else
            EventController.Instance.PostEvent("OnReleaseObject",null);
    }

    void Update()
    {
        UpdatePlaneRotation();

        StartCoroutine(ChangeSpeed(_leverLevel + 0.45f));
    }

    private float rotX = 0;
    private float rotY = 0;
    private float previousX = 0;


    private float prevX = 0;
    private float prevY = 0;

    private bool firstTimeCalculationInput = true;
    private void UpdatePlaneRotation()
    {
        if (Application.isEditor)
        {
            UpdateEditorRotation();
        }
        else
        {
            if (OptionsController.Instance.Tilt)
            {
                bool leftDown = false, rightDown = false, upDown = false, downDown = false;

                Vector3 inputAccel = Input.acceleration.normalized;
                float yTiltFinal = 0f;

                // The Y rotation point which we calibrate to
                const float CenterRotation = 0.3f;

                // Amount of deadzone (Device rotation around CenterRotation which does not rotate the plane)
                const float DeadZoneAmount = 0.001f;

                // Multiplier for the rotation to be enough to count as a full plane rotation amount (e.g 2 will half the amount of rotation required to get a turn value of 1f etc)
                const float Multiplier = 2f;

                if (inputAccel.y < 0f && inputAccel.z < 0f)
                {
                    // The device is between screen flat up and screen facing player flat

                    // Device rotation is less than CenterRotation so the plane needs to point down
                    if (Mathf.Abs(inputAccel.y) <= CenterRotation)
                    {
                        // ABS((ABS(0.6) / 0.7) - 1) * 2 = 0.2
                        yTiltFinal = Mathf.Abs((Mathf.Abs(inputAccel.y)/CenterRotation) - 1f)*Multiplier;
                    }
                    else if (Mathf.Abs(inputAccel.y) >= (CenterRotation + DeadZoneAmount))
                    {
                        // ((0.7 + 0.1) + 0.9) * 2 = -0.2
                        yTiltFinal = ((CenterRotation + DeadZoneAmount) + inputAccel.y)*Multiplier;
                    }
                }
                else if (inputAccel.y < 0f && inputAccel.z > 0f)
                {
                    // The device is between screen facing player flat and screen down flat

                    // -((1 - (0.7 + 0.1)) + ABS((ABS(0.9) / 1) - 1) * 2 = -0.6
                    yTiltFinal =
                        -((1f - (CenterRotation + DeadZoneAmount)) + Mathf.Abs((Mathf.Abs(inputAccel.y)/1f) - 1f))*
                        Multiplier;
                }
                else if (inputAccel.y > 0f && inputAccel.z > 0f)
                {
                    // The device is between screen down flat and screen away player flat

                    // Obviously if we had the player rotating the device to this angle they wouldn't be able to see the screen!
                    yTiltFinal = -1f;
                }
                else
                {
                    // The device is between facing away from the player flat and screen flat up
                    yTiltFinal = ((1f - (CenterRotation + DeadZoneAmount)) +
                                  Mathf.Abs((Mathf.Abs(inputAccel.y)/1f) - 1f))*Multiplier;
                }

                yTiltFinal = Mathf.Clamp(yTiltFinal, -1f, 1f);
                // Move the Y and Z around to the new calibration value

                inputAccel = new Vector3(inputAccel.x, yTiltFinal, 0f);

                // Deadzone (Don't listen to rotations less than this value)
                float dzf = 0.05f;

                if (inputAccel.x < -dzf) leftDown = true;
                if (inputAccel.x > +dzf) rightDown = true;
                if (inputAccel.y < -dzf) downDown = true;
                if (inputAccel.y > +dzf) upDown = true;
                float x = 0;
                float y = 0;
                if (leftDown || rightDown) x = inputAccel.x;
                if (upDown || downDown) y = inputAccel.y;

                if (Input.acceleration.z < -1.8f)
                {
                    firstTimeCalculationInput = false;
                    if (!Plane.IsMakingTurn)
                    {
                        Plane.TurnDirection = 1;
                        EventController.Instance.PostEvent("MakeSharpTurn", gameObject);
                    }
                }
                prevX = x; //Mathf.Lerp(prevX, x, Time.deltaTime*100);
                prevY = y; //Mathf.Lerp(prevY, y, Time.deltaTime*100);
                Plane.Rotation = new Vector2(Mathf.Clamp(prevX*2.2f, -1, 1), Mathf.Clamp(prevY*1.1f, -1, 1));
            }
            else
            {
                Vector2 rot = _relNav / 50;
                Plane.Rotation = new Vector2(Mathf.Clamp(rot.x, -1, 1), Mathf.Clamp(rot.y, -1, 1));
            }
        }

        if (MissionController.Instance.CurrentState is LandingState &&
            AirplaneController.Instance.State == AirplaneStates.Ride)
        {
            Plane.Rotation = new Vector2(0, Plane.Rotation.y);
        }
    }




    private void UpdateEditorRotation()
    {
        float x = Input.GetKey(KeyCode.RightArrow)
            ? 1f
            : Input.GetKey(KeyCode.LeftArrow)
                ? -1f
                : 0f;
        float y = Input.GetKey(KeyCode.UpArrow)
            ? 1f
            : Input.GetKey(KeyCode.DownArrow)
                ? -1f
                : 0f;

        Plane.Rotation = new Vector2(Mathf.Clamp(x, -1, 1), Mathf.Clamp(y, -1, 1));
    }

    IEnumerator ChangeSpeed(float NewSpeed)
    {
        yield return new WaitForSeconds(0.3f);
        Plane.Speed = NewSpeed;
    }

    public Rect PercentToScreen(Rect Percent)
    {
        return new Rect(Percent.xMin * Screen.width,
                        Percent.yMin * Screen.height,
                        Percent.width * Screen.width,
                        Percent.height * Screen.height);
    }

    private Rect SwapCoords(Rect tex)
    {
        return new Rect(tex.xMin,
                        Screen.height - (tex.yMin + tex.height),
                        tex.width, tex.height);
    }

    #region IEventSubscriber implementation

    public void OnEvent(string EventName, GameObject Sender)
    {
        if (EventName == "MissionFinished")
            _relNav.y = -50;
    }

    #endregion
}
