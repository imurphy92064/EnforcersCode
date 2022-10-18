using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Transform door;
    
    public Vector3 openDoorPos=  new Vector3(0,3f,0);
    public Vector3 closeDoorPos= new Vector3(0,0,0);
    public UnityEngine.Events.UnityEvent Trigger;
    private AudioSource audio;
    
    public float openSpeed= 10;
    private bool open = false;

   void Update()
   {
    if(open)
    {
        door.position = Vector3.Lerp(door.position, openDoorPos, Time.deltaTime * openSpeed);
    }
    else
    {
        door.position = Vector3.Lerp(door.position, closeDoorPos, Time.deltaTime * openSpeed);
    }
   }
   
    void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")
        {
        Trigger.Invoke();
        
        OpenDoor();
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
        CloseDoor();
        }
        
    }


    public void CloseDoor()
    {
        open=false;
        Debug.Log("Closed Door!");
    }
    public void OpenDoor()
    {
        open=true;
        Debug.Log("Opened Door!");
    }
}
