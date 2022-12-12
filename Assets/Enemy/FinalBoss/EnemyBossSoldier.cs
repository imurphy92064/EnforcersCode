using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossSoldier : MonoBehaviour
{
   public float playerVisionRange;
	public Transform PlayerHead;
	private Transform Eyesight;
	private GunSystem heldGun;
	private EnemyHP enemyHP;
	private int LayerPlayer;
	private LayerMask WhatEnemyBulletsCanHit;

	private void Start() {
		WhatEnemyBulletsCanHit = LayerMask.GetMask("Ground", "Player");
		LayerPlayer = LayerMask.NameToLayer("Player");

		//References
		var transforms = GetComponentsInChildren<Transform>();

		foreach (var currTransform in transforms)
			switch (currTransform.name) {
				case "Eyesight":
					Eyesight = currTransform;
					break;
				case "AssaultRifle":
					heldGun = currTransform.GetComponent<GunSystem>();
					break;
			}

		//EnemyHP
		enemyHP = GetComponent<EnemyHP>();
		enabled = false;
	}

	// Update is called once per frame
	private void Update() {
		//Look at player
		transform.LookAt(PlayerHead);
		Eyesight.LookAt(PlayerHead);
		var baseTransformAngles = transform.rotation.eulerAngles;
		var EyesightAngles = Eyesight.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(0f, baseTransformAngles.y, 0f);
		Eyesight.rotation = Quaternion.Euler(EyesightAngles.x, EyesightAngles.y, EyesightAngles.z);

		//Bools to decide behavior
		var canSeePlayer = false;
		RaycastHit raycastHit;

		if (Physics.Raycast(Eyesight.position,
					Eyesight.forward,
					out raycastHit,
					playerVisionRange,
					WhatEnemyBulletsCanHit)) {
			var currTransform = raycastHit.collider.transform;
			if (currTransform.gameObject.layer == LayerPlayer) canSeePlayer = true;
		}

		var isDead = enemyHP.health <= 0;

		//Shoot if we can see player
		if (canSeePlayer && !isDead) heldGun.TryShoot();

		/*
		 * 1. LOS of player
		 * 2. Can I shoot
		 * 3. Move
		 * 4. Reload
		 */
	}
}
