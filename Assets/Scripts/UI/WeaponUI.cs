using UnityEngine;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject weaponPanel;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI holsterHintText;

    [Header("Referencia al Jugador")]
    [SerializeField] private WeaponHandler weaponHandler;

    [Header("Textos de Control Personalizables")]
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

        // 1. Si no tiene arma conseguida, oculta la UI por completo
        if (!weaponHandler.HasWeapon)
        {
            weaponPanel.SetActive(false);
            return;
        }

        // Si tiene un arma, activa el panel
        weaponPanel.SetActive(true);

        // 2. Si el arma está enfundada/guardada
        if (weaponHandler.IsHolstered)
        {
            if (weaponNameText != null) 
                weaponNameText.text = holsteredStatusText;

            if (ammoText != null) 
                ammoText.text = "- / -";

            if (holsterHintText != null) 
                holsterHintText.text = $"[{holsterKeyName}] {drawMessage}";
        }
        // 3. Si el arma está equipada y lista para usar
        else
        {
            // Toma el nombre dinámicamente desde el WeaponData
            if (weaponNameText != null && weaponHandler.CurrentWeapon != null) 
            {
                weaponNameText.text = weaponHandler.CurrentWeapon.weaponName;
            }

            // Muestra las balas dinámicamente
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