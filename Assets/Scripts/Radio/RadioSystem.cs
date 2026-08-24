using System;
using UnityEngine;

public class RadioSystem : MonoBehaviour
{
    [System.Serializable]
    public struct RadioStation
    {
        public string stationName;
        public float frequency;
        public float tolerance;
        public AudioClip voiceClip;
    }
    
    [SerializeField] private KeyCode toggleRadioKey = KeyCode.R;
    [SerializeField] private float currentFrequency = 90.0f;
    [SerializeField] private float minFrequency = 88.0f;
    [SerializeField] private float maxFrequency = 108.0f;
    [SerializeField] private float frequencyStepSpeed = 5.0f;
    [SerializeField] private float tolerance = 0.4f;
    
    [SerializeField] private RadioStation[] activeZoneStations;
    
    [SerializeField] private AudioSource staticAudioSource;
    [SerializeField] private AudioSource voiceAudioSource;
    
    [SerializeField] private bool hasRadio = false;
    
    public bool HasRadio => hasRadio;
    
    public event Action<bool> OnRadioToggled;
    public event Action<float> OnFrequencyChanged;

    private bool _isRadioActive;

    public bool IsRadioActive => _isRadioActive;
    public float CurrentFrequency => currentFrequency;

    private void Update()
    {
        if (!hasRadio) return;
        
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
        AudioClip clipToPlay = null;
        
        if (activeZoneStations != null)
        {
            foreach (var station in activeZoneStations)
            {
                float stationTol = station.tolerance > 0 ? station.tolerance : tolerance;
                if (Mathf.Abs(currentFrequency - station.frequency) <= stationTol)
                {
                    clipToPlay = station.voiceClip;
                    break;
                }
            }
        }

        if (clipToPlay != null)
        {
            if (staticAudioSource != null && staticAudioSource.isPlaying) 
                staticAudioSource.Stop();
            
            if (voiceAudioSource != null)
            {
                if (voiceAudioSource.clip != clipToPlay || !voiceAudioSource.isPlaying)
                {
                    voiceAudioSource.clip = clipToPlay;
                    voiceAudioSource.Play();
                }
            }
        }
        else
        {
            if (voiceAudioSource != null && voiceAudioSource.isPlaying) 
                voiceAudioSource.Stop();
                
            if (staticAudioSource != null && !staticAudioSource.isPlaying) 
                staticAudioSource.Play();
        }
    }

    private void StopAllRadioAudio()
    {
        if (staticAudioSource != null && staticAudioSource.isPlaying) staticAudioSource.Stop();
        if (voiceAudioSource != null && voiceAudioSource.isPlaying) voiceAudioSource.Stop();
    }

    public void SetZoneStations(RadioStation[] stations)
    {
        activeZoneStations = stations;
    }

    public void ClearZoneStations()
    {
        activeZoneStations = null;
        StopAllRadioAudio();
    }
    
    public void EquipRadio()
    {
        hasRadio = true;
    }
}