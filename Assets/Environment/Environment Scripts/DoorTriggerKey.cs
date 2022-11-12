using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DoorTriggerKey : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform door;
    
    public Vector3 openDoorPos=  new Vector3(0,3f,0);
    public Vector3 closeDoorPos= new Vector3(0,0,0);
    public TextMeshProUGUI endText;
    
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
        endText.text= "You must find the key to open this door!";
        }
        
    }

    void OnTriggerExit()
    {
        endText.text = " ";
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
