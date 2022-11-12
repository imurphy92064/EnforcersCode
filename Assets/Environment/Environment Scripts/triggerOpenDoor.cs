using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class triggerOpenDoor : MonoBehaviour
{
    public DoorTriggerKey trigger;
    public TextMeshProUGUI endText;
    public GameObject key;

    
    // Start is called before the first frame update
    
    
    void Start()
    {
        key.SetActive(false);
        trigger= GameObject.Find("DoorTrigger2").GetComponent<DoorTriggerKey>();
    }

   
   void OnTriggerEnter(Collider other)
   {
    if(other.tag == "Player")
    {
        EnforcersEvents.keyCollected.Invoke();
        trigger.OpenDoor();
        Destroy(this.gameObject);
        endText.text="Key Switch Found! A Door Has Opened!";
    }
    void OnTriggerExit()
    {
        endText.text=" ";
    }

   }

   public void show()
   {
    key.SetActive(true);
   }




}
