using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameStart : MonoBehaviour
{
    public TextMeshProUGUI startConditions;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        startConditions.text = "Make Your Way To the Factory";
    }

    void OnTriggerExit()
    {
        startConditions.text=" ";
    }

}
