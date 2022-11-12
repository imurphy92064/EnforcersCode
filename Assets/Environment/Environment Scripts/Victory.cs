using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Victory : MonoBehaviour
{
    
    public TextMeshProUGUI gameend;
    private EnemyController enem;
    // Start is called before the first frame update
    void Start()
    {
       enem= GameObject.Find("EnemyController").GetComponent<EnemyController>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {

        if(enem.Enemies.Count != 0 && other.tag == "Player" )
        {
            gameend.text= " You Must Defeat All Enemies First!";
        }
        else if(enem.Enemies.Count == 0 && other.tag == "Player")
        {
            Application.LoadLevel(1);
        }





    }


}
