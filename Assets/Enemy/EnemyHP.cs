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
    public Animator animator;

    public UnityEngine.AI.NavMeshAgent agent;
    
    private GameObject BloodSprayEffect;
    private GameObject ShieldBreakEffect;
    public GameObject[] explosions;

    //Vars
    public int MaxHealth;
    public int MaxShield;
    public int health;
    public int shield;
    public bool didHandleDeath = false;
    public AudioClip ShieldBreakSFX;
    //public EnemyController enemyController;
    public ScoreText toScore;
    private bool isMech = false;

    // Use this for initialization
    private void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        health = MaxHealth;
        shield = MaxShield;
        //enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
        toScore= GameObject.Find("Score").GetComponent<ScoreText>();
        explosions = new GameObject[3];
        animator = GetComponentInChildren<Animator>();
        
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
                case "Explosions":
                    isMech = true;
                    for (int i = 0; i < currTransform.gameObject.transform.childCount; i++)
                    {
                        Transform tempTransform;
                        tempTransform = currTransform.gameObject.transform.GetChild(i);
                        explosions[i] = tempTransform.gameObject;
                    }
                    break;
                case "ShieldBreakEffect":
                    ShieldBreakEffect = currTransform.gameObject;
                    break;
            }
        }

        if (isMech)
        {
            foreach (GameObject explosion in explosions)
            {
                explosion.SetActive(false);
            }
        }
        BloodSprayEffect.SetActive(false);
        ShieldBreakEffect.SetActive(false);
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
        //Before
        int beforeShield = shield;
        //int beforeHealth = health;

        //Damage and clamp
        int damageThatCanBeAppliedToShield = shield < damage ? shield : damage;
        int damageToApplyToHealth = damage - damageThatCanBeAppliedToShield;
        shield -= damageThatCanBeAppliedToShield;
        health -= damageToApplyToHealth;
        health = health < 0 ? 0 : health;

        //Check if we cracked shield
        if (beforeShield > 0 && shield == 0)
        {
            ShieldBreakEffect.SetActive(true);
            StartCoroutine(hideShieldBreakEffect(0.5f));
            AudioPool.playSound(ShieldBreakSFX, transform);
        }

        if (health <= 0 && !didHandleDeath)
        {
            agent.isStopped = true;
            animator.Play("Death");
            didHandleDeath = true;
            //enemyController.RemoveEnemy();
            toScore.addScore();
            if (transform.gameObject.name == "MediumMechStriker") {
                explosions[0].SetActive(true);
                explosions[1].SetActive(true);
                explosions[2].SetActive(true);
            }
            else 
            {
                BloodSprayEffect.SetActive(true);    
            }
            
            
            Destroy(gameObject, 3.5f);
        }
    }

    IEnumerator hideShieldBreakEffect(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ShieldBreakEffect.SetActive(false);
    }
}