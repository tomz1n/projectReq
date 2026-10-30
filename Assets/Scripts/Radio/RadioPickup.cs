using UnityEngine;

public class RadioPickup : MonoBehaviour
{
    [SerializeField] private string pickupMessage = "Press [E] to grab the radio";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InteractionUI.Instance?.ShowPrompt(pickupMessage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InteractionUI.Instance?.HidePrompt();
        }
    }
    
    public void Pickup(GameObject player)
    {
        RadioSystem radio = player.GetComponent<RadioSystem>();
        if (radio != null)
        {
            radio.EquipRadio();

            RadioUI radioUI = FindObjectOfType<RadioUI>();
            if (radioUI != null)
            {
                radioUI.UnlockRadioHUD(true);
            }

            InteractionUI.Instance?.HidePrompt(); 
            gameObject.SetActive(false);
        }
    }
}