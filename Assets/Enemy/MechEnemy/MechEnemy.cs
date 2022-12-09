using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MechEnemy : MonoBehaviour
{
    public Transform PlayerTransform;
    //public GameObject explosion;
    public Animator animator;
    public GameObject destinationMarker;

    private ScoreText toScore;
    private UnityEngine.AI.NavMeshAgent agent;
    private EnemyController enemyController;
    private EnemyMechSoldier EnemyMechSoldierLOS;

    //Const
    public const float patrollingSpeed = 8.0f;
    public const float aggroSpeed = 12.0f;
    public const float manicSpeed = 8.0f;
    private const float maxHealth = 80.0f;
    private float timePerPatrolTargetChange = 12f;

    private float health = maxHealth;
    private bool handldedEnemyDeath = false;
    private MechEnemyModes currentMode = MechEnemyModes.Patrol;
    private float timeElapsed = 0.0f;
    private Vector3 currentTarget;
    public bool isInAggroRadius = false;
    public bool wasKeyCollected = false;
    private Vector3 center;
    
    //Animations
    private string ENEMY_IDLE = "Idle";
    private string ENEMY_WALK = "Walk";
    private string ENEMY_RUN = "Walk";
    private string ENEMY_IDLE_TO_WALK = "Idle-To-Walk";
    private string ENEMY_WALK_TO_RUN = "Walk-To-Run";
    
    //Animation Parameters
    private AnimatorControllerParameter[] animatorHashes;
    // Start is called before the first frame update
    void Start()
    {
        //destinationMarker = Instantiate(destinationMarker, Vector3.zero, Quaternion.Normalize(destinationMarker.transform.rotation));
        //timePerPatrolTargetChange = Random.Range(4f, 7f);
        EnemyMechSoldierLOS = GetComponent<EnemyMechSoldier>();
        animator = GetComponentInChildren<Animator>();
        animatorHashes = animator.parameters;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        toScore = GameObject.Find("Score").GetComponent<ScoreText>();
        enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
        currentTarget = transform.position;
        agent.speed = patrollingSpeed;
        isInAggroRadius = false;

        //Register callback
        EnforcersEvents.keyCollected.AddListener(keyCollected);
        animator.Play(ENEMY_IDLE);
        //destinationMarker.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        //Add the time delta
        timeElapsed += Time.deltaTime;

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

    
    private void behaviorPatrol()
    {
        if (agent.remainingDistance <= 4.5f)
        {
            animator.SetBool(animatorHashes[0].nameHash, true);
            
            // if (wasKeyCollected)
            // {
            //     animator.Play(ENEMY_IDLE_TO_SPRINT);
            //     animator.SetBool(animatorHashes[1].nameHash, false);
            //     changeMode(FastEnemyModes.Manic);
            //     return;
            // }
        }
        
        EnemyMechSoldierLOS.enabled = false;
        // Switch if we have to
        if (isInAggroRadius)
        {
            animator.Play(ENEMY_WALK_TO_RUN);
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
            //destinationMarker.SetActive(false);
            decideNewPatrolTarget();
            animator.Play(ENEMY_IDLE_TO_WALK);
            animator.SetBool(animatorHashes[0].nameHash, false);
            timeElapsed = 0.0f;
        }
    }
    
    private void decideNewPatrolTarget()
    {
        float new_x_Target = transform.position.x + ((Random.value - 0.5f) * 140.0f);
        float new_y_Target = transform.position.y;
        float new_z_Target = transform.position.z + ((Random.value - 0.5f) * 140.0f);
        currentTarget = new Vector3(new_x_Target, new_y_Target, new_z_Target);
        //destinationMarker.SetActive(true);
        //destinationMarker.transform.position = currentTarget;
    }
    
    private void behaviorAggroWalk()
    {
        EnemyMechSoldierLOS.enabled = true;
        //Switch if we have to
        if (!isInAggroRadius)
        {
            animator.SetBool(animatorHashes[1].nameHash, true);
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
        //transform.LookAt(PlayerTransform.position, Vector3.up);
        currentTarget = PlayerTransform.position;
    }
    
    private void changeMode(MechEnemyModes newMode)
    {
        //Reset time
        timeElapsed = 0.0f;

        //Handle changes
        if (newMode == MechEnemyModes.Patrol)
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
    
    public enum MechEnemyModes
    {
        Patrol,
        AggroWalk,
        Manic
    }
}
