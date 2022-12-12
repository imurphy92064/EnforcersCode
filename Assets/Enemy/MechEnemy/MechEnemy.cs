using UnityEngine;
using UnityEngine.AI;

public class MechEnemy : MonoBehaviour
{

    public enum MechEnemyModes
    {
        Patrol, AggroWalk, Manic
    }

    public enum MechEnemyMovement
    {
        Idle, Walking, Running, Sprinting, Dead
    }

    private MechEnemyMovement movementStatus = MechEnemyMovement.Idle;

    //Const
    public const float patrollingSpeed = 8.0f;
    public const float aggroSpeed = 12.0f;
    public const float manicSpeed = 8.0f;
    private const float maxHealth = 80.0f;

    private Transform PlayerTransform;
    private EnemyController enemyController;
    private EnemyMechSoldier EnemyMechSoldierLOS;
    private ScoreText toScore;

    //public GameObject explosion;
    public Animator animator;
    public bool isInAggroRadius;
    public bool wasKeyCollected;
    private NavMeshAgent agent;

    //Animation Parameters
    private AnimatorControllerParameter[] animatorHashes;
    private Vector3 center;
    private MechEnemyModes currentMode = MechEnemyModes.Patrol;
    private Vector3 currentTarget;

    //Animations
    private const string ENEMY_IDLE = "Idle";
    private const string ENEMY_IDLE_TO_WALK = "Idle-To-Walk";
    private const string ENEMY_RUN = "Run";
    private const string ENEMY_WALK = "Walk";
    private const string ENEMY_WALK_TO_RUN = "Walk-To-Run";
    public const string ENEMY_DEATH = "Death";

    private float timeElapsed;
    private float timePerPatrolTargetChange;

    // Start is called before the first frame update

    private void Start()
    {
        //Grab player
        if (GameObject.Find("Player") != null)
        {
            PlayerTransform = GameObject.Find("Player").transform;
        }
        else
        {
            PlayerTransform = transform;
        }

        timePerPatrolTargetChange = Random.Range(6f, 10f);
        EnemyMechSoldierLOS = GetComponent<EnemyMechSoldier>();
        animator = GetComponentInChildren<Animator>();
        animatorHashes = animator.parameters;
        agent = GetComponent<NavMeshAgent>();
        toScore = GameObject.Find("Score").GetComponent<ScoreText>();
        if (GameObject.Find("EnemyController") != null)
        {
            enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
        }
        currentTarget = transform.position;
        agent.speed = patrollingSpeed;
        isInAggroRadius = false;

        //Register callback
        //EnforcersEvents.keyCollected.AddListener(keyCollected);
        tryToPlay(ENEMY_IDLE);
    }

    // Update is called once per frame
    private void Update()
    {
        //Add the time delta
        timeElapsed += Time.deltaTime;

        if (movementStatus != MechEnemyMovement.Dead)
        {
            switch (currentMode)
            {
                case MechEnemyModes.Patrol:
                    behaviorPatrol();
                    break;
                case MechEnemyModes.AggroWalk:
                    behaviorAggroWalk();
                    break;
                case MechEnemyModes.Manic:
                    //behaviorManic();
                    break;
            }

            //Walk to target
            agent.SetDestination(currentTarget);
        }
    }

    private void behaviorPatrol()
    {
        if (agent.remainingDistance <= 4.5f)
        {
            animator.SetBool(animatorHashes[0].nameHash, true);
            changeMovementStatus(MechEnemyMovement.Idle);
        }

        // if (wasKeyCollected)
        // {
        //     animator.Play(ENEMY_IDLE_TO_SPRINT);
        //     animator.SetBool(animatorHashes[1].nameHash, false);
        //     changeMode(FastEnemyModes.Manic);
        //     return;
        // }
        EnemyMechSoldierLOS.enabled = false;

        // Switch if we have to
        if (isInAggroRadius)
        {
            tryToPlay(ENEMY_WALK_TO_RUN);
            animator.SetBool(animatorHashes[1].nameHash, false);
            animator.SetBool(animatorHashes[0].nameHash, false);
            changeMode(MechEnemyModes.AggroWalk);
            return;
        }

        // if (wasKeyCollected)
        // {
        //     animator.Play(ENEMY_WALK_TO_SPRINT);
        //     animator.SetBool(animatorHashes[1].nameHash, false);
        //     changeMode(FastEnemyModes.Manic);
        //     return;
        // }

        //Update speed
        agent.speed = patrollingSpeed;

        //See if the time has gone past
        if (timeElapsed > timePerPatrolTargetChange)
        {
            decideNewPatrolTarget();
            animator.SetBool(animatorHashes[0].nameHash, false);
            tryToPlay(ENEMY_IDLE_TO_WALK);
            timeElapsed = 0.0f;
        }
    }

    private void decideNewPatrolTarget()
    {
        var new_x_Target = transform.position.x + (Random.value - 0.5f) * 100.0f;
        var new_y_Target = transform.position.y;
        var new_z_Target = transform.position.z + (Random.value - 0.5f) * 100.0f;
        currentTarget = new Vector3(new_x_Target, new_y_Target, new_z_Target);
    }

    private void behaviorAggroWalk()
    {
        EnemyMechSoldierLOS.enabled = true;

        //Switch if we have to
        if (!isInAggroRadius)
        {
            animator.SetBool(animatorHashes[1].nameHash, true);
            changeMovementStatus(MechEnemyMovement.Walking);
            changeMode(MechEnemyModes.Patrol);
            animator.SetBool(animatorHashes[0].nameHash, false);
            return;
        }

        // if (wasKeyCollected)
        // {
        //     animator.Play("");
        //     animator.SetBool(animatorHashes[1].nameHash, false);
        //     changeMode(MechEnemyModes.Manic);
        //     return;
        // }

        //Update speed
        //Point at player
        //Update to player position
        agent.speed = aggroSpeed;
        currentTarget = PlayerTransform.position;
    }

    private void changeMode(MechEnemyModes newMode)
    {
        //Reset time
        timeElapsed = 0.0f;

        //Handle changes
        if (newMode == MechEnemyModes.Patrol) decideNewPatrolTarget();

        //Update
        currentMode = newMode;
    }

    public void tryToPlay(string animation)
    {
        switch (animation)
        {
            case ENEMY_IDLE:
                if (movementStatus != MechEnemyMovement.Idle && movementStatus != MechEnemyMovement.Dead)
                {
                    animator.Play(animation);
                    movementStatus = MechEnemyMovement.Idle;
                }

                break;
            case ENEMY_WALK:
            case ENEMY_IDLE_TO_WALK:
                if (movementStatus != MechEnemyMovement.Walking && movementStatus != MechEnemyMovement.Dead)
                {
                    animator.Play(animation);
                    movementStatus = MechEnemyMovement.Walking;
                }

                break;
            case ENEMY_RUN:
            case ENEMY_WALK_TO_RUN:
                if (movementStatus != MechEnemyMovement.Running && movementStatus != MechEnemyMovement.Dead)
                {
                    animator.Play(animation);
                    movementStatus = MechEnemyMovement.Running;
                }

                break;
            case ENEMY_DEATH:
                if (movementStatus != MechEnemyMovement.Dead)
                {
                    animator.Play(animation);
                    movementStatus = MechEnemyMovement.Dead;
                }

                break;
        }
    }

    public void changeMovementStatus(MechEnemyMovement newStatus)
    {
        switch (newStatus)
        {
            case MechEnemyMovement.Idle:
                movementStatus = newStatus;
                break;
            case MechEnemyMovement.Walking:
                movementStatus = newStatus;
                break;
            case MechEnemyMovement.Running:
                movementStatus = newStatus;
                break;
        }
    }

    public void keyCollected()
    {
        wasKeyCollected = true;
    }
}