using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    public int MaxHealth;
    public int MaxShield;
    public AudioClip ShieldBreakSFX;
    //public EnemyController enemyController;

    private bool didHandleDeath;
    private GameObject ShieldBreakEffect;
    private GameObject BloodSprayEffect;
    private GameObject[] explosions = new GameObject[3];
    private Transform CanvasPos;
    private Transform EnemyCanvas;
    private FastEnemy FastEnemyObj;
    private LungeEnemy LungeEnemyObj;
    private MechEnemy MechEnemyObj;
    private BossEnemy FinalBossEnemyObj;
    private Image HealthBar;
    private Text HealthBarText;
    private EnemyType enemyType;
    private Animator animator;
    private NavMeshAgent agent;
    private ScoreText toScore;

    [Header("Ignore these")]
    public int health;
    public int shield;

    private void Start()
    {
        //Set HP
        health = MaxHealth;
        shield = MaxShield;

        //Grab Agent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = GetComponentInChildren<NavMeshAgent>();
        }

        //Grab some other crap
        toScore = GameObject.Find("Score").GetComponent<ScoreText>();
        animator = GetComponentInChildren<Animator>();

        var transforms = GetComponentsInChildren<Transform>();
        foreach (var currTransform in transforms)
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
                case "ShieldBreakEffect":
                    ShieldBreakEffect = currTransform.gameObject;
                    break;
                case "Explosions":
                    for (var i = 0; i < currTransform.gameObject.transform.childCount; i++)
                    {
                        Transform tempTransform;
                        tempTransform = currTransform.gameObject.transform.GetChild(i);
                        explosions[i] = tempTransform.gameObject;
                    }
                    break;
            }
        }

        //Disable effects
        if (BloodSprayEffect != null) BloodSprayEffect.SetActive(false);
        if (ShieldBreakEffect != null) ShieldBreakEffect.SetActive(false);
        if (explosions[0] != null) explosions[0].SetActive(false);
        if (explosions[1] != null) explosions[1].SetActive(false);
        if (explosions[2] != null) explosions[2].SetActive(false);

        //Get enemy scripts
        if (transform.name.Contains("Fast"))
        {
            enemyType = EnemyType.Fast;
            FastEnemyObj = GetComponent<FastEnemy>();
        }
        if (transform.name.Contains("Lunge"))
        {
            enemyType = EnemyType.Lunge;
            LungeEnemyObj = GetComponent<LungeEnemy>();

        }
        if (transform.name.Contains("Mech"))
        {
            enemyType = EnemyType.Mech;
            MechEnemyObj = GetComponent<MechEnemy>();
        }
        if (transform.name.Contains("FinalBoss"))
        {
            enemyType = EnemyType.FinalBoss;
            FinalBossEnemyObj = GetComponent<BossEnemy>();
        }
    }

    private void Update()
    {
        //Update canvas
        EnemyCanvas.transform.position = CanvasPos.transform.position;
        EnemyCanvas.transform.rotation = CanvasPos.transform.rotation;

        //Update health
        HealthBar.fillAmount = health / (float)MaxHealth;
        HealthBarText.text = health.ToString();
        float HPPercent = (float)health / (float)MaxHealth;
        float HPTextR = ((255f / 255f) * (0 + HPPercent)) + ((255f / 255f) * (1 - HPPercent));
        float HPTextG = ((000f / 255f) * (0 + HPPercent)) + ((255f / 255f) * (1 - HPPercent));
        float HPTextB = ((000f / 255f) * (0 + HPPercent)) + ((255f / 255f) * (1 - HPPercent));
        float HPTextA = ((200f / 255f) * (0 + HPPercent)) + ((040f / 255f) * (1 - HPPercent));
        HealthBarText.color = new Color(HPTextR, HPTextG, HPTextB, HPTextA);
    }

    public void takeDamage(int damage)
    {
        //Before
        int beforeShield = shield;

        //Damage and clamp
        int damageThatCanBeAppliedToShield = shield < damage ? shield : damage;
        int damageToApplyToHealth = damage - damageThatCanBeAppliedToShield;
        shield -= damageThatCanBeAppliedToShield;
        health -= damageToApplyToHealth;
        health = health < 0 ? 0 : health;

        //Check if we cracked shield
        if (beforeShield > 0 && shield == 0 && ShieldBreakEffect != null)
        {
            ShieldBreakEffect.SetActive(true);
            StartCoroutine(disableInXSeconds(ShieldBreakEffect, 0.5f));
            AudioPool.playSound(ShieldBreakSFX, transform, 0.5f);
        }

        if (health <= 0 && !didHandleDeath)
        {
            agent.isStopped = true;

            switch (enemyType)
            {
                case EnemyType.Fast:
                    BloodSprayEffect.SetActive(true);
                    StartCoroutine(disableInXSeconds(BloodSprayEffect, 1.5f));
                    FastEnemyObj.tryToPlay(FastEnemy.ENEMY_DEATH);
                    break;
                case EnemyType.Lunge:
                    BloodSprayEffect.SetActive(true);
                    StartCoroutine(disableInXSeconds(BloodSprayEffect, 1.5f));
                    LungeEnemyObj.tryToPlay(LungeEnemy.ENEMY_DEATH);
                    break;
                case EnemyType.Mech:
                    explosions[0].SetActive(true);
                    StartCoroutine(disableInXSeconds(explosions[0], 4f));
                    explosions[1].SetActive(true);
                    StartCoroutine(disableInXSeconds(explosions[1], 4f));
                    explosions[2].SetActive(true);
                    StartCoroutine(disableInXSeconds(explosions[2], 4f));
                    MechEnemyObj.tryToPlay(MechEnemy.ENEMY_DEATH);
                    break;
                case EnemyType.FinalBoss:
                    FinalBossEnemyObj.tryToPlay(BossEnemy.ENEMY_DEATH);
                    break;
            }
            didHandleDeath = true;

            //enemyController.RemoveEnemy();
            toScore.addScore();

            Destroy(gameObject, 5f);
        }
    }

    IEnumerator disableInXSeconds(GameObject pGameObject, float pTime)
    {
        yield return new WaitForSeconds(pTime);
        pGameObject.SetActive(false);
    }
}

public enum EnemyType
{
    Lunge,
    Fast,
    Mech,
    FinalBoss,
}