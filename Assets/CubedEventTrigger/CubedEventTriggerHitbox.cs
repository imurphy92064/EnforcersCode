using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubedEventTriggerHitbox : MonoBehaviour
{
    private CubedEventTriggerMain main;

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        main = transform.parent.GetComponent<CubedEventTriggerMain>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            main.SpawnThings();
        }
    }
}
