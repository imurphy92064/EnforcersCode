using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    private AudioSource audioSource;
    private Transform TransformToStickTo = null;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (TransformToStickTo == null)
        {
            return;
        }

        //Teleport
        transform.position = TransformToStickTo.position;
        transform.rotation = TransformToStickTo.rotation;
    }

    public void playSound(AudioClip audioClip, Transform pTransformToStickTo, float volumeScale)
    {
        //Stop
        audioSource.Stop();

        //Teleport
        TransformToStickTo = pTransformToStickTo;
        transform.position = TransformToStickTo.position;
        transform.rotation = TransformToStickTo.rotation;

        //Play
        audioSource.PlayOneShot(audioClip, volumeScale);
    }
}