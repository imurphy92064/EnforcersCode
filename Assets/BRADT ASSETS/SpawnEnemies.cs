using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent EnemiesToSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            EnemiesToSpawn.Invoke();
            //this.gameObject.SetActive(false);
        }
    }
}
