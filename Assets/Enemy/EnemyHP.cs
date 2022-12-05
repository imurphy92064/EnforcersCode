using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHP : MonoBehaviour
{
    //References
    private Text HealthBarText;
    private Image HealthBar;
    private Transform EnemyCanvas;
    private Transform CanvasPos;

    private GameObject BloodSprayEffect;

    //Vars
    public int MaxHealth;
    public int MaxShield;
    public int health;
    public int shield;
    public bool didHandleDeath = false;
    //public EnemyController enemyController;
    public ScoreText toScore;

    // Use this for initialization
    private void Start()
    {
        health = MaxHealth;
        shield = MaxShield;
        //enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
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
                case "EnemyCanvas":
                    EnemyCanvas = currTransform;
                    break;
                case "CanvasPos":
                    CanvasPos = currTransform;
                    break;
                case "BloodSprayEffect":
                    BloodSprayEffect = currTransform.gameObject;
                    break;
            }
        }

        BloodSprayEffect.SetActive(false);
    }

    private void Update()
    {
        //Update health
        HealthBarText.text = health.ToString();
        HealthBar.fillAmount = (float)health / (float)MaxHealth;
        EnemyCanvas.transform.position = CanvasPos.transform.position;
        EnemyCanvas.transform.rotation = CanvasPos.transform.rotation;
    }

    public void takeDamage(int damage)
    {
        int damageThatCanBeAppliedToShield = shield < damage ? shield : damage;
        int damageToApplyToHealth = damage - damageThatCanBeAppliedToShield;
        shield -= damageThatCanBeAppliedToShield;
        health -= damageToApplyToHealth;
        health = health < 0 ? 0 : health;

        if (health <= 0 && !didHandleDeath)
        {
            didHandleDeath = true;
            //enemyController.RemoveEnemy();
            toScore.addScore();
            BloodSprayEffect.SetActive(true);
            Destroy(gameObject, 1.5f);
        }
    }
}