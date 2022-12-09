using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class LungeEnemy : MonoBehaviour
{
    public Transform PlayerTransform;
    //public GameObject explostion;
    public Animator animator;

    private ScoreText toScore;
    private UnityEngine.AI.NavMeshAgent agent;
    private EnemyController enemyController;
    public GunSystem heldGun;

    //Const
    public const float patrollingSpeed = 3.0f;
    public const float aggroSpeed = 5.0f;
    public const float lungingSpeed = 20.0f;
    private const float maxHealth = 100.0f;
    private float timePerPatrolTargetChange;
    private const float timeLungeCharge = .5f;
    private const float timeLungeDuration = 1.2f;

    private float health = maxHealth;
    private bool handldedEnemyDeath = false;
    private LungeEnemyModes currentMode = LungeEnemyModes.Patrol;
    private float timeElapsed = 0.0f;
    private Vector3 currentTarget;
    public bool isInAggroRadius = false;
    public bool isInLungeRadius = false;
    private LungeEnemyMovement movementStatus = LungeEnemyMovement.Idle;
    

    //Animations
    private const string ENEMY_IDLE = "BasicMotions@Idle01";
    private const string ENEMY_WALK = "BasicMotions@Walk01 - Forwards";
    private const string ENEMY_RUN = "BasicMotions@Run01 - Forwards";
    private const string ENEMY_SPRINT = "BasicMotions@Sprint01 - Forwards";
    private const string ENEMY_WALK_TO_RUN = "Walk-To-Run";
    private const string ENEMY_IDLE_TO_WALK = "Idle-To-Walk";
    private const string ENEMY_IDLE_TO_RUN = "Idle-To-Run";
    private const string ENEMY_IDLE_TO_SPRINT = "Idle-To-Sprint";
    private const string ENEMY_RUN_TO_IDLE = "Run-To-Idle";
    private const string ENEMY_SPRINT_TO_IDLE = "Sprint-To-Idle";
    //Animation Parameters
    private AnimatorControllerParameter[] animatorHashes;
    /*
     * 12/6/22 Changes:
     *  1. Implemented the animation controls and triggers to play when the FastEnemy is moving in specific states.
     * 12/7/22 Changes:
     *  1. Implemented the transitions from one animation to another for smoother looking movement.
     *  2. Added the animator parameter hashing for faster parameter setting.
     *  3. Implemented shooting of the shotgun from this class not EnemySoldiers.
     * 12/8/22 Changes:
     *  1. Changed the way animations are played for the LungeEnemy using Enum Movement states. This is to make sure same
     *      type of animations dont play over each other. To play animations you must use tryToPlay(string)
     */
    
    // Use this for initialization
    void Start()
    {
        timePerPatrolTargetChange = Random.Range(4f, 7f);
        heldGun = GetComponentInChildren<GunSystem>();
        animator = GetComponentInChildren<Animator>();
        animatorHashes = animator.parameters;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        toScore = GameObject.Find("Score").GetComponent<ScoreText>();
        enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
        currentTarget = transform.position;
        agent.speed = patrollingSpeed;
        isInAggroRadius = false;
        isInLungeRadius = false;
        animator.Play(ENEMY_IDLE);
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
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            if (health < 1.0f && !handldedEnemyDeath)
            {
                handldedEnemyDeath = true;
                enemyController.RemoveEnemy();
                toScore.addScore();
            }
        }
    }

    private void behaviorPatrol() {
        if (agent.remainingDistance <= .5f)
        {
            animator.SetBool(animatorHashes[0].nameHash, true);
            changeMovementStatus(LungeEnemyMovement.Idle);
        }
        
       
        //Switch if we have to
        if (isInLungeRadius)
        {
            animator.SetBool(animatorHashes[0].nameHash, true);
            changeMovementStatus(LungeEnemyMovement.Idle);
            changeMode(LungeEnemyModes.LungeCharge);
            return;
        }
        if (isInAggroRadius)
        {
            tryToPlay(ENEMY_WALK_TO_RUN);
            animator.SetBool(animatorHashes[1].nameHash, false);
            animator.SetBool(animatorHashes[0].nameHash, false);
            changeMode(LungeEnemyModes.AggroWalk);
            return;
        }

        //Update speed
        agent.speed = patrollingSpeed;

        //See if the time has gone past
        if (timeElapsed > timePerPatrolTargetChange)
        {
            decideNewPatrolTarget();
            tryToPlay(ENEMY_IDLE_TO_WALK);
            animator.SetBool(animatorHashes[0].nameHash, false);
            timeElapsed = 0.0f;
        }
    }

    private void behaviorAggroWalk()
    {
        //Switch if we have to
        if (isInLungeRadius)
        { 
            tryToPlay(ENEMY_RUN_TO_IDLE);
            changeMode(LungeEnemyModes.LungeCharge);
            return;
        }
        
        if (!isInAggroRadius)
        {
            animator.SetBool(animatorHashes[1].nameHash, true);
            changeMode(LungeEnemyModes.Patrol);
            animator.SetBool(animatorHashes[0].nameHash, false);
            changeMovementStatus(LungeEnemyMovement.Walking);
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
        //enemyNormalSoldierLOS.enabled = true;
        //Update speed
        //Point at player
        //Update to player position
        agent.speed = 0.0f;
        transform.LookAt(PlayerTransform.position, Vector3.up);
        currentTarget = PlayerTransform.position;

        //See if the time has gone past
        if (timeElapsed > timeLungeCharge)
        { 
            tryToPlay(ENEMY_IDLE_TO_SPRINT);
            animator.SetBool(animatorHashes[1].nameHash, false);
            changeMode(LungeEnemyModes.Lunging);
            return;
        }

    }

    private void behaviorLunging()
    {
        if (agent.remainingDistance <= 1f)
        {
             tryToPlay(ENEMY_SPRINT_TO_IDLE);
             agent.isStopped = true;
            transform.LookAt(PlayerTransform.position, Vector3.up);
            heldGun.TryShoot();
            agent.isStopped = false;
        }
        //Update speed
        agent.speed = 20.0f;

        //See if the time has gone past
        if (timeElapsed > timeLungeDuration)
        {
            if (isInLungeRadius)
            {
                changeMode(LungeEnemyModes.LungeCharge);
                return;
            }
            changeMode(LungeEnemyModes.AggroWalk);
            tryToPlay(ENEMY_IDLE_TO_RUN);
            animator.SetBool(animatorHashes[1].nameHash, false);
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

    private void tryToPlay(string animation)
    {
        switch (animation)
        {
            case ENEMY_IDLE:
            case ENEMY_RUN_TO_IDLE:
            case ENEMY_SPRINT_TO_IDLE:
                if (movementStatus != LungeEnemyMovement.Idle)
                {
                    animator.Play(animation);
                    movementStatus = LungeEnemyMovement.Idle;
                }
                break;
            case ENEMY_WALK:
            case ENEMY_IDLE_TO_WALK:
                if (movementStatus != LungeEnemyMovement.Walking)
                {
                    animator.Play(animation);
                    movementStatus = LungeEnemyMovement.Walking;
                }
                break;
            case ENEMY_RUN:
            case ENEMY_IDLE_TO_RUN:
            case ENEMY_WALK_TO_RUN:
                if (movementStatus != LungeEnemyMovement.Running)
                {
                    animator.Play(animation);
                    movementStatus = LungeEnemyMovement.Running;
                }
                break;
            case ENEMY_SPRINT:
            case ENEMY_IDLE_TO_SPRINT:
                if (movementStatus != LungeEnemyMovement.Sprinting)
                {
                    animator.Play(animation);
                    movementStatus = LungeEnemyMovement.Sprinting;
                }
                break;
                
        }
    }
    
    public void changeMovementStatus(LungeEnemyMovement newStatus) {
        switch (newStatus)
        {
            case LungeEnemyMovement.Idle:
                movementStatus = newStatus;
                break;
            case LungeEnemyMovement.Walking:
                movementStatus = newStatus;
                break;            
            case LungeEnemyMovement.Running:
                movementStatus = newStatus;
                break;
            case LungeEnemyMovement.Sprinting:
                movementStatus = newStatus;
                break;
        }
    }
}

public enum LungeEnemyModes
{
    Patrol,
    AggroWalk,
    LungeCharge,
    Lunging
}

public enum LungeEnemyMovement
{
    Idle,
    Walking,
    Running,
    Sprinting
}