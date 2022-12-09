using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeEnemyLR : MonoBehaviour
{
    private LungeEnemy parent = null;

    void Start()
    {
        parent = transform.parent.parent.GetComponent<LungeEnemy>();//Small Changed, need to go up 1 parent in the tree
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parent.isInLungeRadius = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parent.isInLungeRadius = false;
        }
    }
}
