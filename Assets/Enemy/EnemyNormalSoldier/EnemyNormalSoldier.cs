using UnityEngine;

public class EnemyNormalSoldier : MonoBehaviour
{
    private Transform PlayerTransform;
    public float playerVisionRange;

    private Transform Eyesight;
    private GunSystem heldGun;
    private EnemyHP enemyHP;
    private int LayerPlayer;
    private LayerMask WhatEnemyBulletsCanHit;

    private void Start()
    {
        WhatEnemyBulletsCanHit = LayerMask.GetMask("Ground", "Player");
        LayerPlayer = LayerMask.NameToLayer("Player");

        //Grab player head
        if (GameObject.Find("CenterOfMass") != null)
        {
            PlayerTransform = GameObject.Find("CenterOfMass").transform;
        }
        else
        {
            PlayerTransform = transform;
        }

        //References
        var transforms = GetComponentsInChildren<Transform>();

        foreach (var currTransform in transforms)
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

        if (Eyesight == null)
        {
            Debug.LogError("No Eyesight on this enemy");
        }

        //EnemyHP
        enemyHP = GetComponent<EnemyHP>();
    }

    // Update is called once per frame
    public void UpdateCustom()
    {
        if (!(enemyHP.health <= 0))
        {
            //Look at player
            transform.LookAt(PlayerTransform);
            Eyesight.LookAt(PlayerTransform);
            var baseTransformAngles = transform.rotation.eulerAngles;
            var EyesightAngles = Eyesight.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, baseTransformAngles.y, 0f);
            Eyesight.rotation = Quaternion.Euler(EyesightAngles.x, EyesightAngles.y, EyesightAngles.z);
        }

        /*
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

        //Shoot if we can see player
        if (canSeePlayer && !isDead) heldGun.TryShoot();
        */
    }
}