using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class FastEnemy : MonoBehaviour
{
    public Transform PlayerTransform;
    public GameObject explostion;

    private ScoreText toScore;
    private UnityEngine.AI.NavMeshAgent agent;
    private EnemyController enemyController;

    //Const
    public const float patrollingSpeed = 3.0f;
    public const float aggroSpeed = 7.0f;
    public const float manicSpeed = 14.0f;
    private const float maxHealth = 80.0f;
    private const float timePerPatrolTargetChange = 2.0f;

    private float health = maxHealth;
    private bool handldedEnemyDeath = false;
    private FastEnemyModes currentMode = FastEnemyModes.Patrol;
    private float timeElapsed = 0.0f;
    private Vector3 currentTarget;
    public bool isInAggroRadius = false;
    public bool wasKeyCollected = false;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        toScore = GameObject.Find("Score").GetComponent<ScoreText>();
        enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
        currentTarget = transform.position;
        agent.speed = patrollingSpeed;
        isInAggroRadius = false;

        //Register callback
        EnforcersEvents.keyCollected.AddListener(keyCollected);
    }

    // Update is called once per frame
    void Update()
    {
        //Add the time delta
        timeElapsed += Time.deltaTime;

        switch (currentMode)
        {
            case FastEnemyModes.Patrol:
                behaviorPatrol();
                break;
            case FastEnemyModes.AggroWalk:
                behaviorAggroWalk();
                break;
            case FastEnemyModes.Manic:
                behaviorManic();
                break;
        }

        //Walk to target
        agent.SetDestination(currentTarget);
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
        if (isInAggroRadius)
        {
            changeMode(FastEnemyModes.AggroWalk);
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
        if (!isInAggroRadius)
        {
            changeMode(FastEnemyModes.Patrol);
            return;
        }
        if (wasKeyCollected)
        {
            changeMode(FastEnemyModes.Manic);
            return;
        }

        //Update speed
        //Point at player
        //Update to player position
        agent.speed = aggroSpeed;
        transform.LookAt(PlayerTransform.position, Vector3.up);
        currentTarget = PlayerTransform.position;
    }

    private void behaviorManic()
    {
        //Switch if we have to
        if (!isInAggroRadius)
        {
            changeMode(FastEnemyModes.Patrol);
            return;
        }

        //Update speed
        //Point at player
        //Update to player position
        agent.speed = manicSpeed;
        transform.LookAt(PlayerTransform.position, Vector3.up);
        currentTarget = PlayerTransform.position;
    }

    private void decideNewPatrolTarget()
    {
        float new_x_Target = transform.position.x + ((Random.value - 0.5f) * 20.0f);
        float new_y_Target = transform.position.y;
        float new_z_Target = transform.position.z + ((Random.value - 0.5f) * 20.0f);
        currentTarget = new Vector3(new_x_Target, new_y_Target, new_z_Target);
    }

    private void changeMode(FastEnemyModes newMode)
    {
        //Reset time
        timeElapsed = 0.0f;

        //Handle changes
        if (newMode == FastEnemyModes.Patrol)
        {
            decideNewPatrolTarget();
        }

        //Update
        currentMode = newMode;
    }

    public void keyCollected()
    {
        wasKeyCollected = true;
    }
}

public enum FastEnemyModes
{
    Patrol,
    AggroWalk,
    Manic
}