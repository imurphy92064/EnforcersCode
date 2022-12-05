using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPool : MonoBehaviour
{
    public const int PoolSize = 16;
    public GameObject SpeakerPrefab;

    private List<GameObject> SpeakerGameObjects;
    private List<Speaker> Speakers;
    private static AudioPool Singleton = null;
    private static int currIndex = 0;

    void Start()
    {
        //Register ourselves as the singleton
        Singleton = this;

        //Spawn our objects
        SpeakerGameObjects = new List<GameObject>();
        Speakers = new List<Speaker>();
        for (int i = 0; i < PoolSize; i++)
        {
            SpeakerGameObjects.Add(Instantiate(SpeakerPrefab));
            Speakers.Add(SpeakerGameObjects[i].GetComponent<Speaker>());
            SpeakerGameObjects[i].transform.position = Vector3.zero;
            SpeakerGameObjects[i].transform.rotation = Quaternion.identity;
        }
    }

    public static void playSound(AudioClip audioClip, Transform pTransformToStickTo)
    {
        //Bail for null pointers
        if (Singleton == null || Singleton.Speakers[currIndex] == null)
        {
            return;
        }

        //Play
        Singleton.Speakers[currIndex].playSound(audioClip, pTransformToStickTo);

        //Inc
        currIndex++;
        currIndex = currIndex >= PoolSize ? 0 : currIndex;
    }
}