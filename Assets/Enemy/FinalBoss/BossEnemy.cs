using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemy : MonoBehaviour
{
	public enum BossEnemyModes { Patrol, AggroWalk, Manic }
	
	public enum BossEnemyMovement {
		Idle, Walking, Running, Sprinting, Dead
	}
	
	private BossEnemyMovement movementStatus = BossEnemyMovement.Idle;
	
    //Const
	public const float patrollingSpeed = 7.0f;
	public const float aggroSpeed = 20.0f;
	public const float manicSpeed = 8.0f;
	private const float maxHealth = 80.0f;

	public Transform PlayerTransform;
	private EnemyController enemyController;
	private EnemyBossSoldier EnemyBossSoldierLOS;
	private ScoreText toScore;

	//public GameObject explosion;
	public Animator animator;
	public bool isInAggroRadius;
	public bool wasKeyCollected;
	private NavMeshAgent agent;

	//Animation Parameters
	private AnimatorControllerParameter[] animatorHashes;
	private Vector3 center;
	private BossEnemyModes currentMode = BossEnemyModes.Patrol;
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

	private void Start() {
		timePerPatrolTargetChange = Random.Range(11f, 15f);
		EnemyBossSoldierLOS = GetComponent<EnemyBossSoldier>();
		animator = GetComponentInChildren<Animator>();
		animatorHashes = animator.parameters;
		agent = GetComponentInChildren<NavMeshAgent>();
		toScore = GameObject.Find("Score").GetComponent<ScoreText>();
		enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
		currentTarget = transform.position;
		agent.speed = patrollingSpeed;
		isInAggroRadius = false;
		
		
		//Register callback
		//EnforcersEvents.keyCollected.AddListener(keyCollected);
		tryToPlay(ENEMY_IDLE);
	}

	// Update is called once per frame
	private void Update() {
		//Add the time delta
		timeElapsed += Time.deltaTime;

		if (movementStatus != BossEnemyMovement.Dead) {
			switch (currentMode) {
				case BossEnemyModes.Patrol:
					behaviorPatrol();
					break;
				case BossEnemyModes.AggroWalk:
					behaviorAggroWalk();
					break;
				case BossEnemyModes.Manic:
					//behaviorManic();
					break;
			}

			//Walk to target
			agent.SetDestination(currentTarget);	
		}
	}

	private void behaviorPatrol() {
		if (agent.remainingDistance <= 6.5f){
			animator.SetBool(animatorHashes[0].nameHash, true);
			changeMovementStatus(BossEnemyMovement.Idle);
		}

		// if (wasKeyCollected)
		// {
		//     animator.Play(ENEMY_IDLE_TO_SPRINT);
		//     animator.SetBool(animatorHashes[1].nameHash, false);
		//     changeMode(FastEnemyModes.Manic);
		//     return;
		// }
		EnemyBossSoldierLOS.enabled = false;

		// Switch if we have to
		if (isInAggroRadius) {
			animator.SetBool(animatorHashes[1].nameHash, false);
			animator.SetBool(animatorHashes[0].nameHash, false);
			tryToPlay(ENEMY_WALK_TO_RUN);
			changeMode(BossEnemyModes.AggroWalk);
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
		if (timeElapsed > timePerPatrolTargetChange) {
			decideNewPatrolTarget();
			animator.SetBool(animatorHashes[0].nameHash, false);
			tryToPlay(ENEMY_IDLE_TO_WALK);
			timeElapsed = 0.0f;
		}
	}

	private void decideNewPatrolTarget() {
		var new_x_Target = transform.position.x + (Random.value - 0.5f) * 200.0f;
		var new_y_Target = transform.position.y;
		var new_z_Target = transform.position.z + (Random.value - 0.5f) * 200.0f;
		currentTarget = new Vector3(new_x_Target, new_y_Target, new_z_Target);
	}

	private void behaviorAggroWalk() {
		EnemyBossSoldierLOS.enabled = true;
	
		//Switch if we have to
		if (!isInAggroRadius) {
			animator.SetBool(animatorHashes[1].nameHash, true);
			changeMovementStatus(BossEnemyMovement.Walking);
			changeMode(BossEnemyModes.Patrol);
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

	private void changeMode(BossEnemyModes newMode) {
		//Reset time
		timeElapsed = 0.0f;

		//Handle changes
		if (newMode == BossEnemyModes.Patrol) decideNewPatrolTarget();

		//Update
		currentMode = newMode;
	}

	public void tryToPlay(string animation) {
		switch (animation) {
			case ENEMY_IDLE:	
				if (movementStatus != BossEnemyMovement.Idle && movementStatus != BossEnemyMovement.Dead) {
					animator.Play(animation);
					movementStatus = BossEnemyMovement.Idle;
				}

				break;
			case ENEMY_WALK:
			case ENEMY_IDLE_TO_WALK:	
				if (movementStatus != BossEnemyMovement.Walking && movementStatus != BossEnemyMovement.Dead) {
					animator.Play(animation);
					movementStatus = BossEnemyMovement.Walking;
				}

				break;
			case ENEMY_RUN:
			case ENEMY_WALK_TO_RUN:	
				if (movementStatus != BossEnemyMovement.Running && movementStatus != BossEnemyMovement.Dead) {
					animator.Play(animation);
					movementStatus = BossEnemyMovement.Running;
				}

				break;
			case ENEMY_DEATH:
				if (movementStatus != BossEnemyMovement.Dead) {
					animator.Play(animation);
					movementStatus = BossEnemyMovement.Dead;
				}

				break;
		}
	}

	public void changeMovementStatus(BossEnemyMovement newStatus) {
		switch (newStatus) {
			case BossEnemyMovement.Idle:
				movementStatus = newStatus;
				break;
			case BossEnemyMovement.Walking:
				movementStatus = newStatus;
				break;
			case BossEnemyMovement.Running:
				movementStatus = newStatus;
				break;
		}
	}
	
	public void keyCollected() {
		wasKeyCollected = true;
	}
}
