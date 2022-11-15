using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class EnemyHPDamage : MonoBehaviour
{
    public float maxHealth = 80.0f;
    private GameObject explosion;
    private Text HealthBarText;
    private Image HealthBar;
    private float health = 0f;
    private bool handldedEnemyDeath = false;

    // Use this for initialization
    private void Start()
    {
        health = maxHealth;
        HealthBarText = transform.Find("EnemyCanvas").Find("HealthBarText").GetComponent<Text>();
        HealthBar = transform.Find("EnemyCanvas").Find("MaxHealthBar").Find("HealthBar").GetComponent<Image>();
        
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform currTransform in transforms)
        {
            switch (transform.name)
            {
                case "HealthBar":
                    HealthBar = currTransform.GetComponent<Image>();
                    break;
                case "HealthBarText":
                    HealthBarText = currTransform.GetComponent<Text>();
                    break;
            }
        }
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        health = health < 0f ? 0f : health;

        //Update health
        HealthBarText.text = health.ToString();
        HealthBar.fillAmount = health / maxHealth;

        if (health <= 0f && !handldedEnemyDeath)
        {
            handldedEnemyDeath = true;
            //Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}