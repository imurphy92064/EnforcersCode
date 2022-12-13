using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public PickupType pickupType;
    public GameObject PrefabRedKey;
    public GameObject PrefabGreenKey;
    public GameObject PrefabBlueKey;
    public GameObject PrefabHealth;
    public GameObject PrefabArmor;
    public GameObject PrefabAmmo;
    public GameObject PrefabPistol;
    public GameObject PrefabAssultRifle;
    public GameObject PrefabShotgun;
    public GameObject PrefabSniper;

    private const float SecondsPerUpDown = 3.5f;
    private float currTime = 0.0f;
    private Transform Raised;
    private bool hasBeenClaimed = false;
    private GameObject PrefabInstance;

    private void Start()
    {
        //Give random currTime
        currTime = Random.value * 10f;

        //Get point
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform currTransform in transforms)
        {
            if (currTransform.name == "Raised")
            {
                Raised = currTransform;
            }
        }

        //Create object
        switch (pickupType)
        {
            case PickupType.RedKey:
                PrefabInstance = Instantiate(PrefabRedKey);
                break;
            case PickupType.GreenKey:
                PrefabInstance = Instantiate(PrefabGreenKey);
                break;
            case PickupType.BlueKey:
                PrefabInstance = Instantiate(PrefabBlueKey);
                break;
            case PickupType.Health:
                PrefabInstance = Instantiate(PrefabHealth);
                break;
            case PickupType.Armor:
                PrefabInstance = Instantiate(PrefabArmor);
                break;
            case PickupType.Ammo:
                PrefabInstance = Instantiate(PrefabAmmo);
                break;
            case PickupType.Pistol:
                PrefabInstance = Instantiate(PrefabPistol);
                break;
            case PickupType.AssultRifle:
                PrefabInstance = Instantiate(PrefabAssultRifle);
                break;
            case PickupType.Shotgun:
                PrefabInstance = Instantiate(PrefabShotgun);
                break;
            case PickupType.Sniper:
                PrefabInstance = Instantiate(PrefabSniper);
                break;
        }
    }

    private void Update()
    {
        if (hasBeenClaimed)
        {
            return;
        }

        //Move point
        currTime += Time.deltaTime;
        while (currTime > SecondsPerUpDown)
        {
            currTime -= SecondsPerUpDown;
        }
        float progress = currTime / SecondsPerUpDown;
        float sinValue = Mathf.Sin(progress * 2.0f * Mathf.PI);
        PrefabInstance.transform.position = new Vector3(Raised.position.x,
            Raised.position.y + sinValue * 0.25f,
            Raised.position.z);
        PrefabInstance.transform.rotation = Quaternion.Euler(0f,
            progress * 360.0f,
            0f);
    }

    public void TriggerEntered(Collider other)
    {
        if (!hasBeenClaimed)
        {
            //Switch depending on what it is
            switch (pickupType)
            {
                case PickupType.RedKey:
                    Globals.hasRedKey = true;
                    break;
                case PickupType.GreenKey:
                    Globals.hasGreenKey = true;
                    break;
                case PickupType.BlueKey:
                    Globals.hasBlueKey = true;
                    break;
                case PickupType.Health:
                    if (other.CompareTag("Player"))
                    {
                        var PlayerHP = other.GetComponent<PlayerHP>();
                        if (PlayerHP.health >= PlayerHP.MaxHealth)
                        {
                            return;
                        }
                        else
                        {
                            PlayerHP.health += 50;
                            PlayerHP.health = PlayerHP.health > PlayerHP.MaxHealth ?
                                PlayerHP.MaxHealth :
                                PlayerHP.health;
                        }
                    }
                    break;
                case PickupType.Armor:
                    if (other.CompareTag("Player"))
                    {
                        var PlayerHP = other.GetComponent<PlayerHP>();
                        if (PlayerHP.shield >= PlayerHP.MaxShield)
                        {
                            return;
                        }
                        else
                        {
                            PlayerHP.shield += 50;
                            PlayerHP.shield = PlayerHP.shield > PlayerHP.MaxShield ?
                                PlayerHP.MaxShield :
                                PlayerHP.shield;
                        }
                    }
                    break;
                case PickupType.Ammo:
                    GiveAmmo(other);
                    break;
                case PickupType.Pistol:
                    Globals.hasPistol = true;
                    break;
                case PickupType.AssultRifle:
                    Globals.hasAssultRifle = true;
                    break;
                case PickupType.Shotgun:
                    Globals.hasShotgun = true;
                    break;
                case PickupType.Sniper:
                    Globals.hasSniper = true;
                    break;
            }

            //Claim
            hasBeenClaimed = true;

            //Destroy
            Destroy(PrefabInstance);
            //Destroy(gameObject);
        }
    }

    public void GiveAmmo(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var Player = other.GetComponent<Player>();
            foreach (GunSystem gun in Player.weaponSwitching.gunSystems)
            {
                switch (gun.gunType)
                {
                    case GunSystem.GunType.Pistol:
                        gun.bulletsReserve += 30;
                        break;
                    case GunSystem.GunType.AssultRifle:
                        gun.bulletsReserve += 60;
                        break;
                    case GunSystem.GunType.Shotgun:
                        gun.bulletsReserve += 240;
                        break;
                    case GunSystem.GunType.Sniper:
                        gun.bulletsReserve += 10;
                        break;
                }
            }
        }
    }
}

public enum PickupType
{
    RedKey,
    GreenKey,
    BlueKey,
    Health,
    Armor,
    Ammo,
    Pistol,
    AssultRifle,
    Shotgun,
    Sniper,
}