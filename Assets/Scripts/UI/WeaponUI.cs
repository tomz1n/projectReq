using UnityEngine;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private GameObject weaponPanel;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI holsterHintText;
    
    [SerializeField] private WeaponHandler weaponHandler;
    
    [SerializeField] private string holsterKeyName = "H";
    [SerializeField] private string holsterMessage = "Enfundar";
    [SerializeField] private string drawMessage = "Equipar";
    [SerializeField] private string holsteredStatusText = "Guardada";

    private void Awake()
    {
        if (weaponPanel != null)
        {
            weaponPanel.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (weaponHandler == null) return;

        if (!weaponHandler.HasWeapon)
        {
            weaponPanel.SetActive(false);
            return;
        }

        weaponPanel.SetActive(true);

        if (weaponHandler.IsHolstered)
        {
            if (weaponNameText != null) 
                weaponNameText.text = holsteredStatusText;

            if (ammoText != null) 
                ammoText.text = "- / -";

            if (holsterHintText != null) 
                holsterHintText.text = $"[{holsterKeyName}] {drawMessage}";
        }
        else
        {
            if (weaponNameText != null && weaponHandler.CurrentWeapon != null) 
            {
                weaponNameText.text = weaponHandler.CurrentWeapon.weaponName;
            }

            if (ammoText != null) 
            {
                ammoText.text = $"{weaponHandler.CurrentAmmo} / {weaponHandler.ReserveAmmo}";
            }

            if (holsterHintText != null) 
            {
                holsterHintText.text = $"[{holsterKeyName}] {holsterMessage}";
            }
        }
    }
}