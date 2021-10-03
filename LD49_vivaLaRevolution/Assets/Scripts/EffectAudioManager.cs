
using UnityEngine;

public class EffectAudioManager : MonoBehaviour
{

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] windowClips;
    [SerializeField] AudioClip[] molotovClips;
    [SerializeField] AudioClip[] healingClips;
    [SerializeField] [Range(0,0.4f)] float windowsSoundVolume=0.1f;
    [SerializeField] [Range(0,0.7f)] float molotovSoundVolume=0.4f;
    [SerializeField] [Range(0,0.7f)] float healingSoundVolume=0.4f;
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
        audioSource.transform.position=pos;
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
        audioSource.transform.position=pos;
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
        audioSource.transform.position=pos;
        int rdmIndex = Random.Range(0, healingClips.Length);
        if (rdmIndex <= healingClips.Length - 1)
        {
            audioSource.volume = healingSoundVolume;
            audioSource.clip = healingClips[rdmIndex];
            audioSource.Play();
        }
    }
}
