using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPDamage : MonoBehaviour
{
    public float maxHealth = 100f;
    private GameObject explosion;
    private Text HealthBarText;
    private Image HealthBar;
    private float health;
    private bool handldedEnemyDeath = false;

    // Use this for initialization
    private void Start()
    {
        health = maxHealth;
        
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

    private void Update()
    {
        //Update health
        HealthBarText.text = health.ToString();
        HealthBar.fillAmount = health / maxHealth;
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        health = health < 0f ? 0f : health;

        if (health <= 0f && !handldedEnemyDeath)
        {
            handldedEnemyDeath = true;
            //Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}