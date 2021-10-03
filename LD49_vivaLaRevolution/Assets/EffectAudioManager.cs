
using UnityEngine;

public class EffectAudioManager : MonoBehaviour
{

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] windowClips;
    [SerializeField] AudioClip[] molotovClips;
    public static EffectAudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    public void PlayWindowClip()
    {
        int rdmIndex = Random.Range(0, windowClips.Length);
        audioSource.clip = windowClips[rdmIndex];
        audioSource.Play();
    }

    public void PlayMolotovClip()
    {
        int rdmIndex = Random.Range(0, windowClips.Length);
        audioSource.clip = molotovClips[rdmIndex];
        audioSource.Play();
    }
}
