using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveArmor : MonoBehaviour
{
    
         public bool didGiveHealth = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !didGiveHealth)
        {
           
                //Get player script
                
                var PlayerHP = other.GetComponent<PlayerHP>();
                
                if(PlayerHP.shield == 100)
                {
                    Debug.Log("MaxShield");
                }else
                {
                    didGiveHealth = true;
                    PlayerHP.shield +=25;
                    Destroy(this.gameObject);
                }


                
            
            
        }
    }



}
