using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterCreator : MonoBehaviour 
{
    public int Iteration = 0;
    public int MaxIteration = 100;

    private GameObject left = null;
    private GameObject forward = null;
    private GameObject right = null;
    private GameObject back = null;

	float normalMapOffset;

    private GameObject CreateNewWater(Vector3 way)
    {
        Vector3 pos = transform.position - way*2000;
        RaycastHit hit;
        if (Physics.Raycast(new Ray(pos-Vector3.up*103,Vector3.up),out hit,5, 1 << LayerMask.NameToLayer("Water")))
          return hit.collider.gameObject;
        else
        {
            GameObject t = GameObject.Instantiate(gameObject) as GameObject;
            t.transform.parent = transform.parent;
            t.name = Iteration.ToString();
            t.transform.position = pos;
            t.transform.position = new Vector3(t.transform.position.x, 0f, t.transform.position.z);
            t.transform.localScale = new Vector3(200,200,1);
            t.GetComponent<WaterCreator>().Iteration = Iteration+1;
            t.GetComponent<WaterCreator>().MaxIteration = Application.isEditor ? 300 : MaxIteration;
            return t;
        }
    }
    
    void OnBecameVisible() 
    {
        if (left == null && Iteration < MaxIteration)
            left = CreateNewWater(-transform.right);

        if (forward == null && Iteration < MaxIteration)
            forward = CreateNewWater(-transform.up);

        if (right == null && Iteration < MaxIteration)
            right = CreateNewWater(transform.right);

        if (back == null && Iteration < MaxIteration)
            back = CreateNewWater(transform.up);

        if (left != null && forward != null && back != null && right != null)
            gameObject.isStatic = true;
    }

	void Update()
	{
		GetComponent<Renderer>().material.SetTextureOffset("_BumpMap",new Vector2(normalMapOffset,normalMapOffset));
		normalMapOffset += 0.00005f;
	}
}
