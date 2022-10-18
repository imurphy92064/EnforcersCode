using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class LungeEnemy : MonoBehaviour
{
    public Transform PlayerTransform;
    public GameObject explostion;

    private ScoreText toScore;
    private Text healText;
    private Image healBar;
    private UnityEngine.AI.NavMeshAgent agent;
    private EnemyController enemyController;

    //Const
    public const float patrollingSpeed = 3.0f;
    public const float aggroSpeed = 5.0f;
    public const float lungingSpeed = 40.0f;
    private const float maxHealth = 100.0f;
    private const float timePerPatrolTargetChange = 1.8f;
    private const float timeLungeCharge = 0.5f;
    private const float timeLungeDuration = 2.0f;

    private float health = maxHealth;
    private bool handldedEnemyDeath = false;
    private LungeEnemyModes currentMode = LungeEnemyModes.Patrol;
    private float timeElapsed = 0.0f;
    private Vector3 currentTarget;
    public bool isInAggroRadius = false;
    public bool isInLungeRadius = false;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        healText = transform.Find("EnemyCanvas").Find("HealthBarText").GetComponent<Text>();
        healBar = transform.Find("EnemyCanvas").Find("MaxHealthBar").Find("HealthBar").GetComponent<Image>();
        toScore = GameObject.Find("Score").GetComponent<ScoreText>();
        enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
        currentTarget = transform.position;
        agent.speed = patrollingSpeed;
        isInAggroRadius = false;
        isInLungeRadius = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Add the time delta
        timeElapsed += Time.deltaTime;

        switch (currentMode)
        {
            case LungeEnemyModes.Patrol:
                behaviorPatrol();
                break;
            case LungeEnemyModes.AggroWalk:
                behaviorAggroWalk();
                break;
            case LungeEnemyModes.LungeCharge:
                behaviorLungeCharge();
                break;
            case LungeEnemyModes.Lunging:
                behaviorLunging();
                break;
        }

        //Walk to target
        agent.SetDestination(currentTarget);

        //Update health
        healText.text = health.ToString();
        healBar.fillAmount = health / maxHealth;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            health -= 10.0f;
            if (health < 1.0f && !handldedEnemyDeath)
            {
                handldedEnemyDeath = true;
                enemyController.RemoveEnemy();
                Destroy(this);
                Instantiate(explostion, transform.position, transform.rotation);
                Destroy(gameObject);
                toScore.addScore();
            }
        }
    }

    private void behaviorPatrol()
    {
        //Switch if we have to
        if (isInLungeRadius)
        {
            changeMode(LungeEnemyModes.LungeCharge);
            return;
        }
        if (isInAggroRadius)
        {
            changeMode(LungeEnemyModes.AggroWalk);
            return;
        }

        //Update speed
        agent.speed = patrollingSpeed;

        //See if the time has gone past
        if (timeElapsed > timePerPatrolTargetChange)
        {
            decideNewPatrolTarget();
            timeElapsed = 0.0f;
        }
    }

    private void behaviorAggroWalk()
    {
        //Switch if we have to
        if (isInLungeRadius)
        {
            changeMode(LungeEnemyModes.LungeCharge);
            return;
        }
        if (!isInAggroRadius)
        {
            changeMode(LungeEnemyModes.Patrol);
            return;
        }

        //Update speed
        //Point at player
        //Update to player position
        agent.speed = aggroSpeed;
        transform.LookAt(PlayerTransform.position, Vector3.up);
        currentTarget = PlayerTransform.position;
    }

    private void behaviorLungeCharge()
    {
        //Update speed
        //Point at player
        //Update to player position
        agent.speed = 0.0f;
        transform.LookAt(PlayerTransform.position, Vector3.up);
        currentTarget = PlayerTransform.position;

        //See if the time has gone past
        if (timeElapsed > timeLungeCharge)
        {
            changeMode(LungeEnemyModes.Lunging);
            return;
        }

    }

    private void behaviorLunging()
    {
        //Update speed
        agent.speed = 20.0f;

        //See if the time has gone past
        if (timeElapsed > timeLungeDuration)
        {
            changeMode(LungeEnemyModes.AggroWalk);
            return;
        }
    }

    private void decideNewPatrolTarget()
    {
        float new_x_Target = transform.position.x + ((Random.value - 0.5f) * 20.0f);
        float new_y_Target = transform.position.y;
        float new_z_Target = transform.position.z + ((Random.value - 0.5f) * 20.0f);
        currentTarget = new Vector3(new_x_Target, new_y_Target, new_z_Target);
    }

    private void changeMode(LungeEnemyModes newMode)
    {
        //Reset time
        timeElapsed = 0.0f;

        //Handle changes
        if (newMode == LungeEnemyModes.Patrol)
        {
            decideNewPatrolTarget();
        }

        //Update
        currentMode = newMode;
    }
}

public enum LungeEnemyModes
{
    Patrol,
    AggroWalk,
    LungeCharge,
    Lunging
}