using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net;

public class GunSystem : MonoBehaviour
{
    //Gun stats
    public int GunID;
    public int damage;
    public bool canZoom;
    public float timeBetweenShooting;
    public float spread;
    public float range;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize;
    public int bulletsPerTap;
    public int bulletsShotInThisBurst;
    public bool IsPlayerGun;
    public int bulletsMag;
    public int bulletsReserve;
    private bool readyToShoot = true;
    private bool reloading = false;

    private const float TracerSpeed = 300f;
    private int ghetto = 0;

    //Reference
    public Transform BulletDirectionTransform;
    public Transform attackPoint;
    private LayerMask WhatPlayerBulletsCanHit;
    private LayerMask WhatEnemyBulletsCanHit;
    private int LayerPlayer;
    private int LayerEnemy;

    //Graphics
    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;
    public TrailRenderer BulletTrail;
    public TextMeshProUGUI text;

    //Recoil
    private Recoil Recoil_Script;

    //Sound
    private AudioSource sound;
    public AudioClip reloadSound;
    public AudioClip shootSound;

    public void Start()
    {
        WhatPlayerBulletsCanHit = LayerMask.GetMask("Ground", "Enemy");
        WhatEnemyBulletsCanHit = LayerMask.GetMask("Ground", "Player");
        LayerPlayer = LayerMask.NameToLayer("Player");
        LayerEnemy = LayerMask.NameToLayer("Enemy");

        bulletsMag = magazineSize;
        sound = GetComponent<AudioSource>();
        Recoil_Script = GameObject.Find("CameraRot/CameraRecoil").GetComponent<Recoil>();
    }

    public void Update()
    {
        //SetText
        if (IsPlayerGun)
        {
            text.SetText(bulletsMag + " / " + bulletsReserve);
        }
    }

    public void TryShoot()
    {
        //Shoot
        if (readyToShoot && !reloading && bulletsMag > 0)
        {
            bulletsShotInThisBurst = bulletsPerTap;
            
            //Play sound
            ghetto++;
            if (ghetto >= 20)
            {
                ghetto = 0;
                sound.Stop();
            }
            sound.PlayOneShot(shootSound);

            //Fire
            Shoot();
            if (IsPlayerGun)
            {
                Recoil_Script.RecoilFire();
            }
        }
    }

    public void TryReload()
    {
        if (bulletsMag < magazineSize && !reloading)
        {
            Reload();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Spread
        float x = UnityEngine.Random.Range(-spread, spread);
        float y = UnityEngine.Random.Range(-spread, spread);

        // Calculate Direction with Spread
        Vector3 direction = BulletDirectionTransform.forward + new Vector3(x, y, 0);
        //Vector3 forward = transform.TransformDirection(Vector3.forward) * 20;
        TrailRenderer trail = Instantiate(BulletTrail, attackPoint.position, Quaternion.identity);

        //RayCast
        RaycastHit raycastHit;
        Vector3 endpoint = Vector3.left;
        if (IsPlayerGun)
        {
            //Player Gun
            if (Physics.Raycast(BulletDirectionTransform.position, direction, out raycastHit, range, WhatPlayerBulletsCanHit))
            {
                endpoint = raycastHit.point;

                Transform currTransform = raycastHit.collider.transform;
                if (currTransform.gameObject.layer == LayerEnemy)
                {
                    EscalateAndDamage(currTransform, false);
                }

                // Graphics
                //GameObject t_newHole = Instantiate(bulletHoleGraphic, raycastHit.point + raycastHit.normal * 0.001f, Quaternion.identity) as GameObject;
                //t_newHole.transform.LookAt(raycastHit.point + raycastHit.normal);
                //Destroy(t_newHole, 5f);
                GameObject t_newMuzzle = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity) as GameObject;
                Destroy(t_newMuzzle, 0.5f);
            }
            else
            {
                endpoint = BulletDirectionTransform.position + BulletDirectionTransform.forward * 100.0f;
            }
            StartCoroutine(SpawnTrail(trail, endpoint));
        }
        else
        {
            //Enemy Gun
            if (Physics.Raycast(BulletDirectionTransform.position, direction, out raycastHit, range, WhatEnemyBulletsCanHit))
            {
                endpoint = raycastHit.point;

                Transform currTransform = raycastHit.collider.transform;
                if (currTransform.gameObject.layer == LayerPlayer)
                {
                    EscalateAndDamage(currTransform, true);
                }

                // Graphics
                //GameObject t_newHole = Instantiate(bulletHoleGraphic, raycastHit.point + raycastHit.normal * 0.001f, Quaternion.identity) as GameObject;
                //t_newHole.transform.LookAt(raycastHit.point + raycastHit.normal);
                //Destroy(t_newHole, 5f);
                GameObject t_newMuzzle = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity) as GameObject;
                Destroy(t_newMuzzle, 0.5f);
            }
            else
            {
                endpoint = BulletDirectionTransform.position + BulletDirectionTransform.forward * 100.0f;
            }
            StartCoroutine(SpawnTrail(trail, endpoint));
        }

        //Bullet dec
        bulletsMag--;
        bulletsShotInThisBurst--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShotInThisBurst > 0 && bulletsMag > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void EscalateAndDamage(Transform pTransform, bool isPlayerTransform)
    {
        Transform currTransform = pTransform;
        if (isPlayerTransform)
        {
            PlayerHP toDamage = null;
            while (toDamage == null)
            {
                //Check for mission failed
                if (currTransform == null)
                {
                    return;
                }

                //Attempt to fetch script
                try
                {
                    toDamage = currTransform.GetComponent<PlayerHP>();
                }
                catch (Exception) { }

                //Escalate once
                currTransform = currTransform.parent;
            }
            toDamage.takeDamage(damage);
        }
        else
        {
            EnemyHP toDamage = null;
            while (toDamage == null)
            {
                //Check for mission failed
                if (currTransform == null)
                {
                    return;
                }

                //Attempt to fetch script
                try
                {
                    toDamage = currTransform.GetComponent<EnemyHP>();
                }
                catch (Exception) { }

                //Escalate once
                currTransform = currTransform.parent;
            }
            toDamage.takeDamage(damage);
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint)
    {
        Vector3 startPosition = Trail.transform.position;
        Vector3 direction = (HitPoint - Trail.transform.position).normalized;

        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float startingDistance = distance;

        while (distance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * TracerSpeed;

            yield return null;
        }
        Trail.transform.position = HitPoint;
        Destroy(Trail.gameObject, Trail.time*10f);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        sound.PlayOneShot(reloadSound);
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        int bulletsThatNeedToBeAdded = magazineSize - bulletsMag;
        int bulletsThatCanBeAdded = bulletsThatNeedToBeAdded < bulletsReserve ?
            bulletsThatNeedToBeAdded :
            bulletsReserve;
        bulletsMag += bulletsThatCanBeAdded;
        bulletsReserve -= bulletsThatCanBeAdded;
        reloading = false;
    }
}