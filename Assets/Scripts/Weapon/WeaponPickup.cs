using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private string pickupMessage = "Press [E] to pick up Weapon";

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
        WeaponHandler weaponHandler = player.GetComponentInChildren<WeaponHandler>();
        if (weaponHandler != null && weaponData != null)
        {
            weaponHandler.EquipWeapon(weaponData);
            InteractionUI.Instance?.HidePrompt();
            gameObject.SetActive(false);
        }
    }
}