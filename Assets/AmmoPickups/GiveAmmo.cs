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
           
                //Get player script
                var Player = other.GetComponent<Player>();
                foreach (GunSystem gun in Player.weaponSwitching.gunSystems)
                {
                    gun.bulletsReserve += 25;
                }
            
            Destroy(this.gameObject);
        }
    }
}