using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour {
	[SerializeField] //this tag makes your private variable also viewable in the editor for easier debugging
	public List<GameObject> Enemies = new();

	public TextMeshProUGUI enemyCounter;

	//Script just needs to keep track of enemies and display how many are left

	// Start is called before the first frame update
	private void Start() {
		enemyCounter.text = "Enemies In The Area: " + Enemies.Count;
	}

	// Update is called once per frame
	private void Update() {
		if (Enemies.Count == 0) enemyCounter.text = "Find Your Way To The End of The Level!";
	}

	public void RemoveEnemy() {
		Enemies.RemoveAt(0);
		enemyCounter.text = "Enemies Remaining: " + Enemies.Count;
	}
}