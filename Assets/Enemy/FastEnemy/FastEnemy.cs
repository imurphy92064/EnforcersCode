using UnityEngine;
using UnityEngine.AI;



public class FastEnemy : MonoBehaviour {

	public enum FastEnemyModes { Patrol, AggroWalk, Manic }

	public enum FastEnemyMovement {
		Idle, Walking, Running, Sprinting, Dead
	}
	//Const
	public const float patrollingSpeed = 3.0f;
	public const float aggroSpeed = 7.0f;
	public const float manicSpeed = 14.0f;
	private const float maxHealth = 80.0f;

	//Animations
	private const string ENEMY_IDLE = "BasicMotions@Idle01";
	private const string ENEMY_WALK = "BasicMotions@Walk01 - Forwards";
	private const string ENEMY_RUN = "BasicMotions@Run01 - Forwards";
	private const string ENEMY_SPRINT = "BasicMotions@Sprint01 - Forwards";
	private const string ENEMY_WALK_TO_RUN = "Walk-To-Run";
	private const string ENEMY_IDLE_TO_WALK = "Idle-To-Walk";
	private const string ENEMY_IDLE_TO_SPRINT = "Idle-To-Sprint";
	private const string ENEMY_WALK_TO_SPRINT = "Walk-To-Sprint";
	public const string ENEMY_DEATH = "Death";

	private Transform PlayerTransform;

	//public GameObject explostion;
	private Animator animator;
	public bool isInAggroRadius;
	public bool wasKeyCollected;
	private NavMeshAgent agent;

	//Animation Parameters
	private AnimatorControllerParameter[] animatorHashes;
	private FastEnemyModes currentMode = FastEnemyModes.Patrol;
	private Vector3 currentTarget;
	private EnemyController enemyController;
	private EnemyNormalSoldier enemyNormalSoldierLOS;
	private bool handldedEnemyDeath;

	private readonly float health = maxHealth;
	private FastEnemyMovement movementStatus = FastEnemyMovement.Idle;
	private float timeElapsed;
	private float timePerPatrolTargetChange;

	private ScoreText toScore;
	/*
	 * 12/6/22 Changes:
	 *  1. Implemented the animation controls and triggers to play when the FastEnemy is moving in specific states.
	 * 12/7/22 Changes:
	 *  1. Implemented the transitions from one animation to another for smoother looking movement.
	 *  2. Added the animator parameter hashing for faster parameter setting.
	 *  3. Added idle/walk to sprint transitions
	 *  4. Added checks for player collecting key inside patrol to activate them towards player no matter current aggro radius
	 * 12/8/22 Changes:
	 *  1. Changed the way animations are executed like Lunge Enemy. 
	 */

	// Use this for initialization
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

        timePerPatrolTargetChange = Random.Range(4f, 7f);
		enemyNormalSoldierLOS = GetComponent<EnemyNormalSoldier>();
		animator = GetComponentInChildren<Animator>();
		animatorHashes = animator.parameters;
		agent = GetComponent<NavMeshAgent>();
		toScore = GameObject.Find("Score").GetComponent<ScoreText>();
		if(GameObject.Find("EnemyController")!=null)
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
	private void Update() {
		//Add the time delta
		timeElapsed += Time.deltaTime;

		if (movementStatus != FastEnemyMovement.Dead) {
			switch (currentMode) {
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
	}

	private void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Bullet")

				//health -= 10.0f;
			if (health < 1.0f && !handldedEnemyDeath) {
				handldedEnemyDeath = true;
				//enemyController.RemoveEnemy();

				//Destroy(this);
				//Instantiate(explostion, transform.position, transform.rotation);
				//Destroy(gameObject);
				toScore.addScore();
			}
	}

	private void behaviorPatrol() {
		if (agent.remainingDistance <= .5f) {
			animator.SetBool(animatorHashes[0].nameHash, true);
			changeMovementStatus(FastEnemyMovement.Idle);

			if (wasKeyCollected) {
				tryToPlay(ENEMY_IDLE_TO_SPRINT);

				//animator.Play(ENEMY_IDLE_TO_SPRINT);
				animator.SetBool(animatorHashes[1].nameHash, false);
				changeMode(FastEnemyModes.Manic);
				return;
			}
		}

		enemyNormalSoldierLOS.enabled = false;

		//Switch if we have to
		if (isInAggroRadius) {
			tryToPlay(ENEMY_WALK_TO_RUN);

			//animator.Play(ENEMY_WALK_TO_RUN);
			animator.SetBool(animatorHashes[1].nameHash, false);
			animator.SetBool(animatorHashes[0].nameHash, false);
			changeMode(FastEnemyModes.AggroWalk);
			return;
		}

		if (wasKeyCollected) {
			tryToPlay(ENEMY_WALK_TO_SPRINT);

			//animator.Play(ENEMY_WALK_TO_SPRINT);
			animator.SetBool(animatorHashes[1].nameHash, false);
			changeMode(FastEnemyModes.Manic);
			return;
		}

		//Update speed
		agent.speed = patrollingSpeed;

		//See if the time has gone past
		if (timeElapsed > timePerPatrolTargetChange) {
			decideNewPatrolTarget();
			tryToPlay(ENEMY_IDLE_TO_WALK);

			//animator.Play(ENEMY_IDLE_TO_WALK);
			animator.SetBool(animatorHashes[0].nameHash, false);
			timeElapsed = 0.0f;
		}
	}

	private void behaviorAggroWalk() {
		enemyNormalSoldierLOS.enabled = true;

		//Switch if we have to
		if (!isInAggroRadius) {
			animator.SetBool(animatorHashes[1].nameHash, true);
			changeMovementStatus(FastEnemyMovement.Walking);
			changeMode(FastEnemyModes.Patrol);
			animator.SetBool(animatorHashes[0].nameHash, false);
			return;
		}

		if (wasKeyCollected) {
			tryToPlay(ENEMY_SPRINT);

			//animator.Play(ENEMY_SPRINT);
			animator.SetBool(animatorHashes[1].nameHash, false);
			changeMode(FastEnemyModes.Manic);
			return;
		}

		//Update speed
		//Point at player
		//Update to player position
		agent.speed = aggroSpeed;

		//transform.LookAt(PlayerTransform.position, Vector3.up);
		currentTarget = PlayerTransform.position;
	}

	private void behaviorManic() {
		enemyNormalSoldierLOS.enabled = true;

		//Switch if we have to
		if (!isInAggroRadius) {
			animator.SetBool(animatorHashes[1].nameHash, true);
			changeMovementStatus(FastEnemyMovement.Walking);
			changeMode(FastEnemyModes.Patrol);
			animator.SetBool(animatorHashes[0].nameHash, false);
			return;
		}

		//Update speed
		//Point at player
		//Update to player position
		agent.speed = manicSpeed;

		//transform.LookAt(PlayerTransform.position, Vector3.up);
		currentTarget = PlayerTransform.position;
	}

	private void decideNewPatrolTarget() {
		var new_x_Target = transform.position.x + (Random.value - 0.5f) * 20.0f;
		var new_y_Target = transform.position.y;
		var new_z_Target = transform.position.z + (Random.value - 0.5f) * 20.0f;
		currentTarget = new Vector3(new_x_Target, new_y_Target, new_z_Target);
	}

	private void changeMode(FastEnemyModes newMode) {
		//Reset time
		timeElapsed = 0.0f;

		//Handle changes
		if (newMode == FastEnemyModes.Patrol) decideNewPatrolTarget();

		//Update
		currentMode = newMode;
	}

	public void tryToPlay(string animation) {
		switch (animation) {
			case ENEMY_IDLE:
				if (movementStatus != FastEnemyMovement.Idle && movementStatus != FastEnemyMovement.Dead) {
					animator.Play(animation);
					movementStatus = FastEnemyMovement.Idle;
				}

				break;
			case ENEMY_WALK:
			case ENEMY_IDLE_TO_WALK:
				if (movementStatus != FastEnemyMovement.Walking && movementStatus != FastEnemyMovement.Dead) {
					animator.Play(animation);
					movementStatus = FastEnemyMovement.Walking;
				}

				break;
			case ENEMY_RUN:
			case ENEMY_WALK_TO_RUN:
				if (movementStatus != FastEnemyMovement.Running && movementStatus != FastEnemyMovement.Dead) {
					animator.Play(animation);
					movementStatus = FastEnemyMovement.Running;
				}

				break;
			case ENEMY_SPRINT:
			case ENEMY_IDLE_TO_SPRINT:
			case ENEMY_WALK_TO_SPRINT:
				if (movementStatus != FastEnemyMovement.Sprinting && movementStatus != FastEnemyMovement.Dead) {
					animator.Play(animation);
					movementStatus = FastEnemyMovement.Sprinting;
				}

				break;
			case ENEMY_DEATH:
				if (movementStatus != FastEnemyMovement.Dead) {
					animator.Play(animation);
					movementStatus = FastEnemyMovement.Dead;
				}

				break;
		}
	}

	public void changeMovementStatus(FastEnemyMovement newStatus) {
		switch (newStatus) {
			case FastEnemyMovement.Idle:
				movementStatus = newStatus;
				break;
			case FastEnemyMovement.Walking:
				movementStatus = newStatus;
				break;
			case FastEnemyMovement.Running:
				movementStatus = newStatus;
				break;
			case FastEnemyMovement.Sprinting:
				movementStatus = newStatus;
				break;
		}
	}

	public void keyCollected() {
		wasKeyCollected = true;
	}
}