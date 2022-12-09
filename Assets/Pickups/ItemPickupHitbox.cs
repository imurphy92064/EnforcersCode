using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupHitbox : MonoBehaviour
{
    ItemPickup parent = null;

    void Start()
    {
        parent = transform.parent.GetComponent<ItemPickup>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            parent.TriggerEntered(other);
        }
    }
}