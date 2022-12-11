using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedKeyHitbox : MonoBehaviour
{
    RedKey parent = null;

    void Start()
    {
        parent = transform.parent.GetComponent<RedKey>();
    }

    public void OnTriggerEnter(Collider other)
    {
        parent.TriggerEntered();
    }
}
