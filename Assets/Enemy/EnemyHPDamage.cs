using UnityEngine;
using UnityEngine.UI;

public class EnemyHPDamage : MonoBehaviour {
	public float maxHealth = 100f;
	public float health;
	public ScoreText toScore;
	private GameObject explosion;
	private bool handldedEnemyDeath;
	private Image HealthBar;
	private Text HealthBarText;

	// Use this for initialization
	private void Start() {
		health = maxHealth;
		toScore = GameObject.Find("Score").GetComponent<ScoreText>();

		var transforms = GetComponentsInChildren<Transform>();

		foreach (var currTransform in transforms)
			switch (currTransform.name) {
				case "HealthBar":
					HealthBar = currTransform.GetComponent<Image>();
					break;
				case "HealthBarText":
					HealthBarText = currTransform.GetComponent<Text>();
					break;
			}
	}

	private void Update() {
		//Update health
		HealthBarText.text = health.ToString();
		HealthBar.fillAmount = health / maxHealth;
	}

	public void takeDamage(float damage) {
		health -= damage;
		health = health < 0f ? 0f : health;

		if (health <= 0f && !handldedEnemyDeath) {
			//enemyController.RemoveEnemy();
			toScore.addScore();
			handldedEnemyDeath = true;

			//Instantiate(explosion, transform.position, transform.rotation);
			Destroy(gameObject);
		}
	}
}