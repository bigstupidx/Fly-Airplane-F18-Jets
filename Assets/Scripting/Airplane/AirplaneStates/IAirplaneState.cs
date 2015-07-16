using UnityEngine;
using System.Collections;

public interface IAirplaneState 
{
    void Awake();
    void FixedUpdate();
    void OnActivate();
    void OnDeactivate();

    void OnCollisionEnter(Collision col);
    void OnCollisionExit(Collision col);

    void OnTriggerEnter(Collider other);
    void OnTriggerExit(Collider other);
}
