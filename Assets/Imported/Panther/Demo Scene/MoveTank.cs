using MilitaryDemo;
using UnityEngine;
using System.Collections;

public class MoveTank : MonoBehaviour
{
    public GameObject Target;

    [SerializeField]
    private float acceleration = 5;

    [SerializeField]
    private float currentVelocity = 0;

    [SerializeField]
    private float maxSpeed = 25;

    [SerializeField]
    private float rotationSpeed = 30;

    [SerializeField]
    private Transform spawnPoint;
    
    [SerializeField]
    private GameObject bulletObject;

    [SerializeField]
    private GameObject fireEffect;


    private MissionObject missionObject;
    private void Start()
    {
        missionObject = GetComponent<MissionObject>();
    }

    private void Update()
    {
        if (Target == null)
        {
            return;
        }
        if (missionObject.Destroyed)
        {
            return;
        }

        //if (Input.GetKey(KeyCode.UpArrow))
        {
            float maxSpd = maxSpeed;
            if (TransportGOController.Instance.SelectedMissionID == 0)
            {
                maxSpd *= 0.25f;
            }

            if (currentVelocity <= maxSpd)
            {
                currentVelocity += acceleration*Time.deltaTime;
            }
            else
            {
                currentVelocity = maxSpd;
            }

        }
//        else if (Input.GetKey(KeyCode.DownArrow))
//        {
//            // minus speed
//            if (currentVelocity >= -maxSpeed)
//                currentVelocity -= acceleration*Time.deltaTime;
//
//        }
//        else
//        {
//            // No key input. 
//            if (currentVelocity > 0)
//                currentVelocity -= acceleration*Time.deltaTime;
//            else if (currentVelocity < 0)
//                currentVelocity += acceleration*Time.deltaTime;
//
//        }


        // Turn off engine if currentVelocity is too small. 
        if (Mathf.Abs(currentVelocity) <= 0.05f)
            currentVelocity = 0;

        if (!enteredBase)
        {
            // Move Tank by currentVelocity
            transform.Translate(new Vector3(0, 0, currentVelocity*Time.deltaTime));
        }



        var to = Quaternion.LookRotation(Target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, to, rotationSpeed*Time.deltaTime);
        transform.localEulerAngles = new Vector3(0,transform.localEulerAngles.y, 0);

        if (enteredBase)
        {
            if (friendlyBase.CurrentHealth > 0f)
            {
                friendlyBase.CurrentHealth -= 2.5f * Time.deltaTime;
                if (friendlyBase.CurrentHealth <= 0f)
                {
                    GameObject ps = GameObject.Instantiate(DataStorageController.Instance.BaseDestroyPSPrefab) as GameObject;
                    ps.transform.position = friendlyBase.transform.position;
                    BaseBeenDestroyedText.BaseDestroyed = true;
                    EventController.Instance.PostEvent("MissionFailed", null);
                }

                // Fire!
                if (RandomTool.NextBool(0.01f))//Input.GetButtonDown("Fire1"))
                {
                    // make fire effect.
                    Instantiate(fireEffect, spawnPoint.position, spawnPoint.rotation);

                    // make ball
                    Instantiate(bulletObject, spawnPoint.position, spawnPoint.rotation);
                }
            }
        }
    }

    private bool enteredBase = false;

    public bool EnteredBase 
    {
        get { return enteredBase; }
    }

    private FriendlyBase friendlyBase;
    private void OnTriggerEnter(Collider collider)
    {
        if (enteredBase) return;
        if (collider.gameObject.CompareTag("FriendlyBase"))
        {
            friendlyBase = collider.gameObject.GetComponent<FriendlyBase>();
            enteredBase = true;
        }
    }
}