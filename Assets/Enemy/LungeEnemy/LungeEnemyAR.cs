using UnityEngine;

public class LungeEnemyAR : MonoBehaviour {
	private LungeEnemy _parent;

	private void Start() {
		_parent = transform.parent.parent
			   .GetComponent<LungeEnemy>(); //Small Changed, need to go up 1 parent in the tree
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) _parent.isInAggroRadius = true;
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) _parent.isInAggroRadius = false;
	}
}