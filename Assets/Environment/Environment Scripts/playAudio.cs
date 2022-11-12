using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playAudio : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource audio;
    
    void Start()
    {
         audio = GetComponent<AudioSource>();
    }
    public void audioStart()
    {
        audio.Play();
    }
}
