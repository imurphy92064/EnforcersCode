using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedKey : MonoBehaviour
{
    private const float SecondsPerUpDown = 3.5f;
    private float currTime = 0.0f;
    private Transform ItemTransform;
    private bool hasBeenClaimed = false;

    private void Start()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach(Transform curr in transforms)
        {
            if (curr.name == "RedKey")
            {
                ItemTransform = curr;
            }
        }
    }

    private void Update()
    {
        currTime += Time.deltaTime;
        if (currTime > SecondsPerUpDown)
        {
            currTime -= SecondsPerUpDown;
        }
        float progress = currTime / SecondsPerUpDown;
        float sinValue = Mathf.Sin(progress * 2.0f * Mathf.PI);
        ItemTransform.transform.localPosition = new Vector3(0.0f, sinValue*0.25f, 0.0f);
        ItemTransform.transform.localRotation = Quaternion.Euler(sinValue * 20.0f, progress * 360.0f, progress * 360.0f);
    }

    public void TriggerEntered()
    {
        if (!hasBeenClaimed)
        {
            hasBeenClaimed = true;
            Globals.hasRedKey = true;
            Destroy(gameObject);
        }
    }
}