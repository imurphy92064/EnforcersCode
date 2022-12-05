using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveHealth : MonoBehaviour
{
    public bool didGiveHealth = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !didGiveHealth)
        {
           
                //Get player script
                
                var PlayerHP = other.GetComponent<PlayerHP>();
                
                if(PlayerHP.health == 100)
                {
                    Debug.Log("MaxHealthy");
                }else
                {
                    didGiveHealth = true;
                    PlayerHP.health +=25;
                    Destroy(this.gameObject);
                }


                
            
            
        }
    }
}
