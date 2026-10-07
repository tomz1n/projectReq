using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float healAmount = 25f;

    public void Pickup(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            playerHealth = player.GetComponentInChildren<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            if (playerHealth.CurrentHealth < playerHealth.MaxHealth)
            {
                playerHealth.Heal(healAmount);

                InteractionUI.Instance?.HidePrompt();

                gameObject.SetActive(false);
            }
        }
    }
}