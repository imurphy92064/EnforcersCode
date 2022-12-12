using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Victory02 : MonoBehaviour
{
    //public TextMeshProUGUI gameend;
    //private EnemyController enem;

    /*void Start()
    {
        enem = GameObject.Find("EnemyController").GetComponent<EnemyController>();
    }*/

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SceneLoading.loadScene("Level3");
        }
    }
}