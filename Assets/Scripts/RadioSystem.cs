using System;
using UnityEngine;

public class RadioSystem : MonoBehaviour
{
    [Header("Radio Settings")]
    [SerializeField] private KeyCode toggleRadioKey = KeyCode.R;
    [SerializeField] private float currentFrequency = 90.0f;
    [SerializeField] private float minFrequency = 88.0f;
    [SerializeField] private float maxFrequency = 108.0f;
    [SerializeField] private float frequencyStepSpeed = 5.0f;
    [SerializeField] private float tolerance = 0.4f;

    [Header("Target Zone (Dinamico)")]
    [SerializeField] private float targetFrequency = 104.5f;
    [SerializeField] private AudioClip currentVoiceClip;

    [Header("Audio Components")]
    [SerializeField] private AudioSource staticAudioSource;
    [SerializeField] private AudioSource voiceAudioSource;
    
    public event Action<bool> OnRadioToggled;
    public event Action<float> OnFrequencyChanged;

    private bool _isRadioActive;

    public bool IsRadioActive => _isRadioActive;
    public float CurrentFrequency => currentFrequency;

    private void Update()
    {
        if (Input.GetKeyDown(toggleRadioKey))
        {
            _isRadioActive = !_isRadioActive;
            OnRadioToggled?.Invoke(_isRadioActive);

            if (!_isRadioActive)
            {
                StopAllRadioAudio();
            }
        }

        if (!_isRadioActive) return;

        // Ajuste de Frecuencia con A/D o Flechas
        float input = Input.GetAxis("Horizontal");
        if (Mathf.Abs(input) > 0.01f)
        {
            currentFrequency += input * frequencyStepSpeed * Time.deltaTime;
            currentFrequency = Mathf.Clamp(currentFrequency, minFrequency, maxFrequency);
            OnFrequencyChanged?.Invoke(currentFrequency);
        }

        ProcessRadioAudio();
    }

    private void ProcessRadioAudio()
    {
        bool isTuned = Mathf.Abs(currentFrequency - targetFrequency) <= tolerance;

        if (isTuned && currentVoiceClip != null)
        {
            if (staticAudioSource.isPlaying) staticAudioSource.Stop();
            
            if (!voiceAudioSource.isPlaying)
            {
                voiceAudioSource.clip = currentVoiceClip;
                voiceAudioSource.Play();
            }
        }
        else
        {
            if (voiceAudioSource.isPlaying) voiceAudioSource.Stop();
            if (!staticAudioSource.isPlaying) staticAudioSource.Play();
        }
    }

    private void StopAllRadioAudio()
    {
        if (staticAudioSource.isPlaying) staticAudioSource.Stop();
        if (voiceAudioSource.isPlaying) voiceAudioSource.Stop();
    }
    
    public void SetZoneFrequency(float newTargetFreq, AudioClip newVoiceClip)
    {
        targetFrequency = newTargetFreq;
        currentVoiceClip = newVoiceClip;
    }
}