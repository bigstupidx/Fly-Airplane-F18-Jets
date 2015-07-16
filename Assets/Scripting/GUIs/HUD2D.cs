using UnityEngine;
using System.Collections;

public class HUD2D : MonoBehaviour
{
    public Transform NumberHeightRoot;
    public GameObject NumberPointPrefab;

    private const float NumberOffset = 100f;
    private const float NumberOffsetHeight = 0.9f;

    public Transform NumberSpeedRoot;
    public GameObject NumberSpeedPrefab;

    public GameObject Root1;
    public GameObject Root2;

    private const float NumberSpeedOffset = 50f;
    private const float NumberSpeedOffsetHeight = 0.9f;

	void Start ()
	{
	    GenerateHeightIndicator();
	    GenerateSpeedIndicator();
	}
	
	void Update ()
	{
	    UpdateHeightIndicator();
	    UpdateSpeedIndicator();

        Root1.SetActive(CameraController.Instance.IsSuperInside);
        Root2.SetActive(CameraController.Instance.IsSuperInside);
        
	}

    private void GenerateHeightIndicator()
    {
        for (int i = -10; i < 100; i++)
        {
            GameObject numberPoint = GameObject.Instantiate(NumberPointPrefab) as GameObject;
            numberPoint.transform.parent = NumberHeightRoot;
            numberPoint.transform.localPosition = new Vector3(0, i * NumberOffsetHeight, 0);
            numberPoint.transform.localEulerAngles = new Vector3(0, 0, 0);
            numberPoint.transform.localScale = new Vector3(1, 1, 1);
            numberPoint.SetActive(true);

            numberPoint.GetComponentInChildren<TextMesh>().text = (i * NumberOffset).ToString();
        }
    }

    private void UpdateHeightIndicator()
    {
        if (NumberHeightRoot.gameObject.activeInHierarchy)
        {

            float height = Mathf.Clamp(AirplaneController.Instance.Height, 0, 1006660f);
            NumberHeightRoot.transform.localPosition = new Vector3(NumberHeightRoot.transform.localPosition.x,
                -(height/NumberOffset)*NumberOffsetHeight, NumberHeightRoot.transform.localPosition.z);
        }
    }






    private void GenerateSpeedIndicator()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject numberPoint = GameObject.Instantiate(NumberSpeedPrefab) as GameObject;
            numberPoint.transform.parent = NumberSpeedRoot;
            numberPoint.transform.localPosition = new Vector3(0, i * NumberSpeedOffsetHeight, 0);
            numberPoint.transform.localEulerAngles = new Vector3(0, 0, 0);
            numberPoint.transform.localScale = new Vector3(1, 1, 1);
            numberPoint.SetActive(true);

            numberPoint.GetComponentInChildren<TextMesh>().text = (i * NumberSpeedOffset).ToString();
        }
    }

    private void UpdateSpeedIndicator()
    {
        float height = AirplaneController.Instance.CurrentSpeed;
        NumberSpeedRoot.transform.localPosition = new Vector3(NumberHeightRoot.transform.localPosition.x, -(height / NumberSpeedOffset) * NumberSpeedOffsetHeight, NumberHeightRoot.transform.localPosition.z);
    }
}
