using UnityEngine;
using System.Collections;

public class insideHUD : MonoBehaviour
{
	public GameObject[] _colors;

	void Start ()
	{
		if (PlayerPrefs.GetInt("additionalHUD") == 1)
			{
			transform.Find("Text").gameObject.SetActive(false);

			foreach (GameObject _color in _colors)
				_color.gameObject.SetActive(true);

			gameObject.SetActive(false);
			}
	}

}
