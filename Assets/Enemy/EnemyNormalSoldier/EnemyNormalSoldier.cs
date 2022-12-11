using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyNormalSoldier : MonoBehaviour
{
    public float playerVisionRange;
    public Transform PlayerHead;
    private LayerMask WhatEnemyBulletsCanHit;
    private int LayerPlayer;
    private Transform Eyesight;
    private GunSystem heldGun;
    private EnemyHP enemyHP;
    
    

    void Start()
    {
        WhatEnemyBulletsCanHit = LayerMask.GetMask("Ground", "Player");
        LayerPlayer = LayerMask.NameToLayer("Player");

        //References
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform currTransform in transforms)
        {
            switch (currTransform.name)
            {
                case "Eyesight":
                    Eyesight = currTransform;
                    break;
                case "WEAPON_AR":
                    heldGun = currTransform.GetComponent<GunSystem>();
                    break;
                case "WEAPON_SHOTGUN":
                    heldGun = currTransform.GetComponent<GunSystem>();
                    break;
                case "ScifiMech20cmCanonBarrelLeft":
                    heldGun = currTransform.GetComponent<GunSystem>();
                    break;
                case "ScifiMech20cmCanonBarrelRight":
                    heldGun = currTransform.GetComponent<GunSystem>();
                    break;
            }
        }

        //EnemyHP
        enemyHP = GetComponent<EnemyHP>();
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Look at player
        transform.LookAt(PlayerHead);
        Eyesight.LookAt(PlayerHead);
        Vector3 baseTransformAngles = transform.rotation.eulerAngles;
        Vector3 EyesightAngles = Eyesight.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, baseTransformAngles.y, 0f);
        Eyesight.rotation = Quaternion.Euler(EyesightAngles.x, EyesightAngles.y, EyesightAngles.z);

        //Bools to decide behavior
        bool canSeePlayer = false;
        RaycastHit raycastHit;
        if (Physics.Raycast(Eyesight.position, Eyesight.forward, out raycastHit, playerVisionRange, WhatEnemyBulletsCanHit))
        {
            Transform currTransform = raycastHit.collider.transform;
            if (currTransform.gameObject.layer == LayerPlayer)
            {
                canSeePlayer = true;
            }
        }
        bool isDead = enemyHP.health <= 0;

        //Shoot if we can see player
        if (canSeePlayer&&!isDead)
        {
            heldGun.TryShoot();
        }

        /*
         * 1. LOS of player
         * 2. Can I shoot
         * 3. Move
         * 4. Reload
         */
    }
}