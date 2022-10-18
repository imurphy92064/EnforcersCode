using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveAmmo : MonoBehaviour
{
    // Start is called before the first frame update
    
    public FirstPersonCtrl ctrl;
    void Start()
    {
        ctrl=GameObject.Find("Robot3").GetComponent<FirstPersonCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter()
    {
        ctrl.ammoCount += 25;
        Debug.Log("Added 10 Ammo!");
        Destroy(this.gameObject);
    }


}
