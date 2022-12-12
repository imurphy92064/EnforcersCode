using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHP : MonoBehaviour
{
    //Vars
    public int MaxHealth;
    public int MaxShield;
    public bool didHandleDeath = false;

    //Player Kill
    private ReStart restart;
    private TextMeshProUGUI end;

    [Header("Ignore these")]
    public int health;
    public int shield;

    private void Start()
    {
        health = MaxHealth;
        shield = MaxShield;

        //References
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform currTransform in transforms)
        {
            switch (currTransform.name)
            {
                case "Restart":
                    restart = currTransform.GetComponent<ReStart>();
                    break;
                case "Game End":
                    end = currTransform.GetComponent<TextMeshProUGUI>();
                    break;
            }
        }
        
        restart.enabled = false;
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
            end.text = "You Lose! Click On Menu Button To Return!";
            restart.enabled = true;
            Player.UnlockCursor();
        }
    }
}