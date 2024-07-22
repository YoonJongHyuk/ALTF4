using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOpenDoor : MonoBehaviour
{
    public AudioClip openDoor;
    AudioSource audioSource;

    bool isStartTrue = false;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ShootPuzzle.openDoor && !isStartTrue)
        {
            StartSound();
        }
    }

    void StartSound()
    {
        audioSource.clip = openDoor;
        audioSource.Play();
        isStartTrue = true;
    }
}
