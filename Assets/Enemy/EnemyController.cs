using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyController : MonoBehaviour
{
    [SerializeField]//this tag makes your private variable also viewable in the editor for easier debugging
    public List<GameObject> Enemies = new List<GameObject>();
    public TextMeshProUGUI enemyCounter;

    //Script just needs to keep track of enemies and display how many are left

    // Start is called before the first frame update
    void Start()
    {
        enemyCounter.text = "Enemies In The Area: " + Enemies.Count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Enemies.Count == 0)
        {
            enemyCounter.text = "All Enemies Defeated!";
        }
    }

    public void RemoveEnemy()
    {

        Enemies.RemoveAt(0);
        enemyCounter.text = "Enemies Remaining: " + Enemies.Count.ToString();
    }
}
