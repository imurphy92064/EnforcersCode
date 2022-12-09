using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechEnemyAR : MonoBehaviour
{
    private MechEnemy parent = null;

    void Start()
    {
        parent = transform.parent.parent.GetComponent<MechEnemy>();//Small Changed, need to go up 1 parent in the tree
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
