using UnityEngine;

public class FastEnemyAR : MonoBehaviour {
	private FastEnemy parent;

	private void Start() {
		parent = transform.parent.parent.GetComponent<FastEnemy>(); //Small Changed, need to go up 1 parent in the tree
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) parent.isInAggroRadius = true;
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) parent.isInAggroRadius = false;
	}
}