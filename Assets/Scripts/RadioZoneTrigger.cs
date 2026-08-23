using UnityEngine;

public class RadioZoneTrigger : MonoBehaviour
{
    [SerializeField] private RadioSystem.RadioStation[] zoneStations;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RadioSystem radio = other.GetComponent<RadioSystem>();
            if (radio != null)
            {
                radio.SetZoneStations(zoneStations);
            }
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
        }
    }
}