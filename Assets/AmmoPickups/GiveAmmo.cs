using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveAmmo : MonoBehaviour
{
    public bool didGiveAmmo = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !didGiveAmmo)
        {
            didGiveAmmo = true;
            for (int i = 0; i < 3; i++)
            {
                Globals.ReserveAmmoCount[i] += 25;
            }
            //Debug.Log("Added 25 Ammo!");
            Destroy(this.gameObject);
        }
    }
}