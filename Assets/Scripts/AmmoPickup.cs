using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 7;

    public void Pickup(GameObject player)
    {
        WeaponHandler weaponHandler = player.GetComponentInChildren<WeaponHandler>();
        if (weaponHandler != null)
        {
            bool wasPickedUp = weaponHandler.AddAmmo(ammoAmount);
            if (wasPickedUp)
            {
                InteractionUI.Instance?.HidePrompt();
                gameObject.SetActive(false); // Desaparece la cajita de balas
            }
        }
    }
}