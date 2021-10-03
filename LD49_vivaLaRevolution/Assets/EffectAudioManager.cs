﻿
using UnityEngine;

public class EffectAudioManager : MonoBehaviour
{

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] windowClips;
    [SerializeField] AudioClip[] molotovClips;
    [SerializeField] [Range(0,0.4f)] float windowsSoundVolume=0.1f;
    [SerializeField] [Range(0,0.7f)] float molotovSoundVolume=0.4f;
    public static EffectAudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    public void PlayWindowClip(Vector3 pos)
    {
        transform.position=pos;
        int rdmIndex = Random.Range(0, windowClips.Length);
        if (rdmIndex <= windowClips.Length - 1)
        {
            audioSource.volume = windowsSoundVolume;
            audioSource.clip = windowClips[rdmIndex];
            audioSource.Play();
        }

    }

    public void PlayMolotovClip(Vector3 pos)
    {
        transform.position=pos;
        int rdmIndex = Random.Range(0, molotovClips.Length);
        if (rdmIndex <= molotovClips.Length - 1)
        {
            audioSource.volume = molotovSoundVolume;
            audioSource.clip = molotovClips[rdmIndex];
            audioSource.Play();
        }
    }
}
