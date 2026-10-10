using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RadioZoneTrigger : MonoBehaviour
{
    [SerializeField] private RadioSystem.RadioStation[] zoneStations;

    [Header("Efecto de Interferencia / Geiger")]
    [SerializeField] private AudioSource staticAudioSource;
    [SerializeField] private float maxVolume = 0.8f;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.3f;

    private SphereCollider _sphereCollider;
    private RadioSystem _currentRadio;
    private bool _playerInZone;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;

        if (staticAudioSource != null)
        {
            staticAudioSource.loop = true;
            staticAudioSource.playOnAwake = false;
            staticAudioSource.volume = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RadioSystem radio = other.GetComponent<RadioSystem>();
            if (radio != null)
            {
                _currentRadio = radio;
                radio.SetZoneStations(zoneStations);
                _playerInZone = true;
            }
        }
    }

    private void Update()
    {
        if (!_playerInZone || _currentRadio == null) return;
        
        if (_currentRadio.HasRadio)
        {
            float distance = Vector3.Distance(_currentRadio.transform.position, transform.position);
            float radius = _sphereCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

            float t = Mathf.Clamp01(distance / radius);
            float proximityFactor = 1f - t;

            if (staticAudioSource != null)
            {
                if (!staticAudioSource.isPlaying) staticAudioSource.Play();
                
                staticAudioSource.volume = Mathf.Lerp(0f, maxVolume, proximityFactor);
                staticAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, proximityFactor);
            }
        }
        else
        {
            StopStaticSound();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RadioSystem radio = other.GetComponent<RadioSystem>();
            if (radio != null)
            {
                radio.ClearZoneStations();
            }

            _playerInZone = false;
            _currentRadio = null;
            StopStaticSound();
        }
    }

    private void StopStaticSound()
    {
        if (staticAudioSource != null && staticAudioSource.isPlaying)
        {
            staticAudioSource.Stop();
            staticAudioSource.volume = 0f;
        }
    }
}