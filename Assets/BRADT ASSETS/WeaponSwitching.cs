using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Animations.Rigging;

public class WeaponSwitching : MonoBehaviour
{

    [Header("References")]
    public List<Transform> weapons;
    public List<GunSystem> gunSystems;

    [Header("Keys")]
    public KeyCode[] keys;

    [Header("Settings")]
    public float switchTime;

    public int selectedWeapon;
    public float timeSinceLastSwitch;

    private void Start()
    {
        SetWeapons();
        Select(selectedWeapon);

        timeSinceLastSwitch = 0f;
    }

    private void SetWeapons()
    {
        weapons = new List<Transform>();
        gunSystems = new List<GunSystem>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = null;
            GunSystem childGunSystem = null;
            try
            {
                child = transform.GetChild(i);
                childGunSystem = child.GetComponent<GunSystem>();
            }
            catch (Exception)
            {
                continue;
            }

            if (child == null || childGunSystem == null)
            {
                continue;
            }

            weapons.Add(child);
            gunSystems.Add(childGunSystem);
        }

        if (keys == null) keys = new KeyCode[weapons.Count];
    }

    private void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]) && timeSinceLastSwitch >= switchTime)
            {
                selectedWeapon = i;
            }
        }

        if (previousSelectedWeapon != selectedWeapon) Select(selectedWeapon);

        timeSinceLastSwitch += Time.deltaTime;
    }

    private void Select(int weaponIndex)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }

        timeSinceLastSwitch = 0f;
    }
}