using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeEnemyAR : MonoBehaviour
{
    private LungeEnemy parent = null;

    void Start()
    {
        parent = transform.parent.GetComponent<LungeEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parent.isInAggroRadius = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parent.isInAggroRadius = false;
        }
    }
}
