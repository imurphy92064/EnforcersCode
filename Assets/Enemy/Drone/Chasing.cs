using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chasing : MonoBehaviour {

	public Transform target;
	public float speed;

	private float health;
	private float maxHealth;
	public GameObject explostion;
	public ScoreText toScore;

	private Text healText;
	private Image healBar;

	public EnemyController enemyController;

	public UnityEngine.AI.NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		speed = 5f;
		health = 100.0f;
		maxHealth = 100.0f;
		healText = transform.Find("EnemyCanvas").Find("HealthBarText").GetComponent<Text>();
		healBar = transform.Find("EnemyCanvas").Find("MaxHealthBar").Find("HealthBar").GetComponent<Image>();
		toScore= GameObject.Find("Score").GetComponent<ScoreText>();
		enemyController= GameObject.Find("EnemyController").GetComponent<EnemyController>();
	}
	
	// Update is called once per frame
	void Update () {
	
		
		transform.LookAt(target, Vector3.up);
		transform.position += transform.forward * speed * Time.deltaTime;
		agent.SetDestination(target.position);
		healText.text = health.ToString();
		healBar.fillAmount = health / maxHealth;
	
	
	
	}

	void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Bullet") {
			health -= 10;
			if(health < 1) {
				enemyController.RemoveEnemy();
				Destroy(this);
				Instantiate(explostion, transform.position, transform.rotation);
				Destroy(gameObject);
				toScore.addScore();
			}
		}
	}
}





