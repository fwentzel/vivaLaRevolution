
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string volumeParameter = "MasterVolume";
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider slider;
    [SerializeField] float multiplier = 30;
    [SerializeField] Toggle toggle;

    private void Awake()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
        toggle.onValueChanged.AddListener(HandleToggleValueChanged);
    }
    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
    }
    private void HandleSliderValueChanged(float value)
    {
        mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiplier);
        toggle.isOn = slider.value>slider.minValue;
    }
    private void HandleToggleValueChanged(bool enableSound)
    {
        slider.value = enableSound ? slider.maxValue : slider.minValue;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }
    void Update()
    {

    }
}
