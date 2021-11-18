
using UnityEngine;

public class EffectAudioManager : MonoBehaviour
{
    public static EffectAudioManager instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [Header("Break into building sounds")]
    [Range(0, 0.4f)]
    [SerializeField] private float windowsSoundVolume = 0.1f;
    [SerializeField] private AudioClip[] windowClips;

    [Header("Item sounds")]
    [Range(0, 0.7f)]
    [SerializeField] private float molotovSoundVolume = 0.4f;
    [SerializeField] private AudioClip[] molotovClips;
    [Space()]
    [Range(0, 0.7f)]
    [SerializeField] private float healingSoundVolume = 0.4f;
    [SerializeField] private AudioClip[] healingClips;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    public void PlayWindowClip(Vector3 pos)
    {
        audioSource.transform.position = pos;
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
        audioSource.transform.position = pos;
        int rdmIndex = Random.Range(0, molotovClips.Length);
        if (rdmIndex <= molotovClips.Length - 1)
        {
            audioSource.volume = molotovSoundVolume;
            audioSource.clip = molotovClips[rdmIndex];
            audioSource.Play();
        }
    }

    public void PlayHealingClip(Vector3 pos)
    {
        audioSource.transform.position = pos;
        int rdmIndex = Random.Range(0, healingClips.Length);
        if (rdmIndex <= healingClips.Length - 1)
        {
            audioSource.volume = healingSoundVolume;
            audioSource.clip = healingClips[rdmIndex];
            audioSource.Play();
        }
    }
}
