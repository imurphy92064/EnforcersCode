using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvoke : MonoBehaviour

{
    public UnityEngine.Events.UnityEvent Trigger;
    

void OnTriggerEnter(Collider col)
    {
        if(col.tag== "Enemy")
        {
            Trigger.Invoke();
        }

    }


}
