using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAR : MonoBehaviour
{
    private BossEnemy parent;

    private void Start() {
        parent = transform.parent.GetComponent<BossEnemy>(); //Small Changed, need to go up 1 parent in the tree
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) parent.isInAggroRadius = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) parent.isInAggroRadius = false;
    }
}
