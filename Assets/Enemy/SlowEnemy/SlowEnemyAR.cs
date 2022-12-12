using UnityEngine;

public class SlowEnemyAR : MonoBehaviour {
	private SlowEnemy parent;

	private void Start() {
		parent = transform.parent.GetComponent<SlowEnemy>();
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) parent.isInAggroRadius = true;
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) parent.isInAggroRadius = false;
	}
}