using UnityEngine;

public class RadioZoneTrigger : MonoBehaviour
{
    [SerializeField] private float zoneTargetFrequency = 104.5f;
    [SerializeField] private AudioClip voiceAudioClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RadioSystem radio = other.GetComponent<RadioSystem>();
            if (radio != null)
            {
                radio.SetZoneFrequency(zoneTargetFrequency, voiceAudioClip);
            }
        }
    }
}