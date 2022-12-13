using UnityEngine;

public class EnemyMechSoldier : MonoBehaviour
{
    public float playerVisionRange;
    public Transform PlayerTransform;
    private Transform Eyesight;
    private GunSystem heldGun1;
    private GunSystem heldGun2;
    private EnemyHP enemyHP;
    private int LayerPlayer;
    private LayerMask WhatEnemyBulletsCanHit;
    private EnemyNormalSoldier ems;

    private void Start()
    {
        //Grab EnemyNormalSoldier
        ems = GetComponent<EnemyNormalSoldier>();

        //Grab player head
        if (GameObject.Find("CenterOfMass") != null)
        {
            PlayerTransform = GameObject.Find("CenterOfMass").transform;
        }
        else
        {
            PlayerTransform = transform;
        }

        WhatEnemyBulletsCanHit = LayerMask.GetMask("Ground", "Player");
        LayerPlayer = LayerMask.NameToLayer("Player");

        //References
        var transforms = GetComponentsInChildren<Transform>();

        foreach (var currTransform in transforms)
            switch (currTransform.name)
            {
                case "Eyesight":
                    Eyesight = currTransform;
                    break;
                case "ScifiMech20cmCanonBarrelLeft":
                    heldGun1 = currTransform.GetComponent<GunSystem>();
                    break;
                case "ScifiMech20cmCanonBarrelRight":
                    heldGun2 = currTransform.GetComponent<GunSystem>();
                    break;
            }

        //EnemyHP
        enemyHP = GetComponent<EnemyHP>();
        enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        //Bools to decide behavior
        var canSeePlayer = false;
        RaycastHit raycastHit;

        if (Physics.Raycast(Eyesight.position,
                    Eyesight.forward,
                    out raycastHit,
                    playerVisionRange,
                    WhatEnemyBulletsCanHit))
        {
            var currTransform = raycastHit.collider.transform;
            if (currTransform.gameObject.layer == LayerPlayer) canSeePlayer = true;
        }

        var isDead = enemyHP.health <= 0;

        if (!isDead)
        {
            //Look at player
            transform.LookAt(PlayerTransform);
            Eyesight.LookAt(PlayerTransform);
            var baseTransformAngles = transform.rotation.eulerAngles;
            var EyesightAngles = Eyesight.localRotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, baseTransformAngles.y, 0f);
            Eyesight.localRotation = Quaternion.Euler(EyesightAngles.x, EyesightAngles.y, EyesightAngles.z);

            //Shoot if we can see player
            if (canSeePlayer)
            {
                heldGun1.TryShoot();
                heldGun2.TryShoot();
            }
        }
    }
}