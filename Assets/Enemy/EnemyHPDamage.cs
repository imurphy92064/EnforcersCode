using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHPDamage : MonoBehaviour
{
    public float maxHealth = 100f;
    private GameObject explosion;
    private Text HealthBarText;
    private Image HealthBar;
    private float health;
    private bool handldedEnemyDeath = false;
    public EnemyController enemyController;
    public ScoreText toScore;

    // Use this for initialization
    private void Start()
    {
        health = maxHealth;
        enemyController= GameObject.Find("EnemyController").GetComponent<EnemyController>();
        toScore= GameObject.Find("Score").GetComponent<ScoreText>();

        
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform currTransform in transforms)
        {
            switch (currTransform.name)
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
            enemyController.RemoveEnemy();
            toScore.addScore();
            handldedEnemyDeath = true;
            //Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}