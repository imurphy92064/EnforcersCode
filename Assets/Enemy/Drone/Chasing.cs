using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Chasing : MonoBehaviour {

	public Transform target;
	public float speed;
	public GameObject explostion;
	public ScoreText toScore;

	public NavMeshAgent agent;
	private Image healBar;

	private Text healText;

	private float health;
	private float maxHealth;

	// Use this for initialization
	private void Start() {
		speed = 5f;
		health = 100.0f;
		maxHealth = 100.0f;
		healText = transform.Find("EnemyCanvas").Find("HealthBarText").GetComponent<Text>();
		healBar = transform.Find("EnemyCanvas").Find("MaxHealthBar").Find("HealthBar").GetComponent<Image>();
		toScore = GameObject.Find("Score").GetComponent<ScoreText>();
	}

	// Update is called once per frame
	private void Update() {
		transform.LookAt(target, Vector3.up);
		transform.position += transform.forward * speed * Time.deltaTime;
		agent.SetDestination(target.position);
		healText.text = health.ToString();
		healBar.fillAmount = health / maxHealth;
	}

	private void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Bullet") {
			health -= 10;

			if (health < 1) {
				//enemyController.RemoveEnemy();
				Destroy(this);
				Instantiate(explostion, transform.position, transform.rotation);
				Destroy(gameObject);
				toScore.addScore();
			}
		}
	}
}