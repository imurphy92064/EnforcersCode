using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class playerKill : MonoBehaviour
{
    public ReStart restart;
    
    public TextMeshProUGUI end;
    // Start is called before the first frame update
    void Start()
    {
        restart.enabled=false;
        restart= GameObject.Find("Canvas/Restart").GetComponent<ReStart>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
       if(other.tag == "Player")
       {
            
             end.text="You Lose! Click On Menu Button To Return!";
             restart.enabled= true;
       }
      
    }
}
