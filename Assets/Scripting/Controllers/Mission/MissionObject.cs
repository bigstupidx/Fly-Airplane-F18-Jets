using UnityEngine;
using System.Collections;

public enum MissionObjectType
{
    Runway,
    Base,
    Transport,
    Island,
    Waypoint
}

public class MissionObject : MonoBehaviour, IDestroyable
{
    public int ID = -1;
    public string Name = "Unnamed";
    public MissionObjectType ObjectType;
    public static Vector3 LastDestroyedPosition;

    public bool Destroyed { get; private set; }
    public MissionObject()
    {
        Destroyed = false;
    }

    [ContextMenu("Destroy!")]
    void Destroy()
    {
        SetDestroyEffect(!Destroyed);
    }

    #region IDestroyable implementation

    private void SetMaterials(Transform Target, bool SetDestroyedMaterial)
    {
        if (Target.GetComponent<Renderer>() && Target.GetComponent<Renderer>().material)
        {
            Material mat;
            if (SetDestroyedMaterial)
            {
                mat = new Material(Shader.Find("Diffuse"));
                mat.SetColor("_Color",Color.black);
            }
            else
            {
                mat = new Material(Shader.Find("Mobile/Diffuse"));
            }
            mat.SetTexture("_MainTex",Target.GetComponent<Renderer>().material.GetTexture("_MainTex"));
            Target.GetComponent<Renderer>().material = mat;
        }
        for (int i=0; i<Target.childCount; i++)
            SetMaterials(Target.GetChild(i), SetDestroyedMaterial);
    }

    public void SetDestroyEffect(bool Destroy)
    {
        if (Destroy == Destroyed)
            return;

        if (Destroy)
        {
            LastDestroyedPosition = this.transform.position;


        }
        SetMaterials(transform, Destroy);
        EventController.Instance.PostEvent("MissionObjectDestroyed",gameObject);
        GameObject ps = GameObject.Instantiate(DataStorageController.Instance.BaseDestroyPSPrefab) as GameObject;
        ps.transform.position = transform.position;
        ps.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Destroyed = Destroy;
    }

    #endregion
}
