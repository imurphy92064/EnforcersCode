using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CubedEventTriggerMain : MonoBehaviour
{
    private GameObject ThingsToSpawn;

    private void Start()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform currTransform in transforms)
        {
            switch (currTransform.name)
            {
                case "ThingsToSpawn":
                    ThingsToSpawn = currTransform.gameObject;
                    break;
            }
        }
        ThingsToSpawn.SetActive(false);
    }

    public void SpawnThings()
    {
        ThingsToSpawn.SetActive(true);
    }
}
