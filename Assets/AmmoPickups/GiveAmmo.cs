using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveAmmo : MonoBehaviour
{
    // Start is called before the first frame update
    
    public FirstPersonCtrl ctrl;
    public bool didGiveAmmo = false;
    void Start()
    {
        ctrl=GameObject.Find("Robot3").GetComponent<FirstPersonCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !didGiveAmmo)
        {
            didGiveAmmo = true;
            ctrl.ammoCount += 25;
            Debug.Log("Added 25 Ammo!");
            Destroy(this.gameObject);
        }
    }


}
