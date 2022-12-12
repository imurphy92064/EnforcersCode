using UnityEngine;
using UnityEngine.AI;

public class SlowEnemy : MonoBehaviour {

	//Const
	public const float patrollingSpeed = 3.0f;
	public const float aggroSpeed = 5.0f;
	public const float manicSpeed = 10.0f;
	private const float timePerPatrolTargetChange = 2.0f;
	public Transform PlayerTransform;
	public bool isInAggroRadius;
	public bool wasKeyCollected;
	private NavMeshAgent agent;

	private SlowEnemyModes currentMode = SlowEnemyModes.Patrol;
	private Vector3 currentTarget;
	private EnemyController enemyController;
	private float timeElapsed;

	private ScoreText toScore;

	// Use this for initialization
	private void Start() {
		agent = GetComponent<NavMeshAgent>();
		toScore = GameObject.Find("Score").GetComponent<ScoreText>();
		enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
		currentTarget = transform.position;
		agent.speed = patrollingSpeed;
		isInAggroRadius = false;

		//Register callback
		//EnforcersEvents.keyCollected.AddListener(keyCollected);
	}

	// Update is called once per frame
	private void Update() {
		//Add the time delta
		timeElapsed += Time.deltaTime;

		switch (currentMode) {
			case SlowEnemyModes.Patrol:
				behaviorPatrol();
				break;
			case SlowEnemyModes.AggroWalk:
				behaviorAggroWalk();
				break;
			case SlowEnemyModes.Manic:
				behaviorManic();
				break;
		}

		//Walk to target
		agent.SetDestination(currentTarget);
	}

	private void behaviorPatrol() {
		//Switch if we have to
		if (isInAggroRadius) {
			changeMode(SlowEnemyModes.AggroWalk);
			return;
		}

		//Update speed
		agent.speed = patrollingSpeed;

		//See if the time has gone past
		if (timeElapsed > timePerPatrolTargetChange) {
			decideNewPatrolTarget();
			timeElapsed = 0.0f;
		}
	}

	private void behaviorAggroWalk() {
		//Switch if we have to
		if (!isInAggroRadius) {
			changeMode(SlowEnemyModes.Patrol);
			return;
		}

		if (wasKeyCollected) {
			changeMode(SlowEnemyModes.Manic);
			return;
		}

		//Update speed
		//Point at player
		//Update to player position
		agent.speed = aggroSpeed;
		transform.LookAt(PlayerTransform.position, Vector3.up);
		currentTarget = PlayerTransform.position;
	}

	private void behaviorManic() {
		//Switch if we have to
		if (!isInAggroRadius) {
			changeMode(SlowEnemyModes.Patrol);
			return;
		}

		//Update speed
		//Point at player
		//Update to player position
		agent.speed = manicSpeed;
		transform.LookAt(PlayerTransform.position, Vector3.up);
		currentTarget = PlayerTransform.position;
	}

	private void decideNewPatrolTarget() {
		var new_x_Target = transform.position.x + (Random.value - 0.5f) * 20.0f;
		var new_y_Target = transform.position.y;
		var new_z_Target = transform.position.z + (Random.value - 0.5f) * 20.0f;
		currentTarget = new Vector3(new_x_Target, new_y_Target, new_z_Target);
	}

	private void changeMode(SlowEnemyModes newMode) {
		//Reset time
		timeElapsed = 0.0f;

		//Handle changes
		if (newMode == SlowEnemyModes.Patrol) decideNewPatrolTarget();

		//Update
		currentMode = newMode;
	}

	public void keyCollected() {
		wasKeyCollected = true;
	}
}

public enum SlowEnemyModes { Patrol, AggroWalk, Manic }