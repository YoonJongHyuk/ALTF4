using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChickenSound : MonoBehaviour
{
    public AudioClip ChickenSound;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.clip = ChickenSound;
        audioSource.Play();
    }
}
