using UnityEngine;
using System.Collections;

public class HelpTextHeight : MonoBehaviour {


    private TextMesh mText;
    private bool _startedAnimation;
    void Start()
    {
        mText = GetComponent<TextMesh>();
        mText.text = "";
    }
	
	// Update is called once per frame
    private void Update()
    {
        if (AirplaneController.Instance.State == AirplaneStates.Fly &&
            MissionController.Instance.CurrentState is FollowingWaypoints ||
            MissionController.Instance.CurrentState is DestroyTargetState)
        {
            if (BaseLevel.Instance.Height != 0)
            {
                mText.text = "Keep your height below " + BaseLevel.Instance.Height + "ft";
                StartCoroutine(Animate());
            }
            else
            {
                SetDefault();
            }
        }
        else
        {
            SetDefault();
        }
    }

    private IEnumerator Animate()
    {
        if (!_startedAnimation)
        {

            _startedAnimation = true;
            yield return new WaitForSeconds(10.0f);

            Vector3 targetPos = new Vector3(-7.4f, 4.0f, 3.7f);
            const float targetScale = 0.09f;

            for (float t = 0; t < 3.0; t+=Time.deltaTime)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime*3);

                float scale = transform.localScale.x;
                scale = Mathf.Lerp(scale, targetScale, Time.deltaTime);

                transform.localScale = new Vector3(scale, scale,1f);

                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void SetDefault()
    {
        if (mText.text != "")
            mText.text = "";
    }
}
