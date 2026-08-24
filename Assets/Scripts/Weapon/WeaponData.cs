using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "REQ/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "9mm Pistol";
    public bool isRaycast = true; 
    
    public int magazineCapacity = 7; 
    public int initialReserveAmmo = 21; 
    public int maxReserveAmmo = 60;  
    
    public float damage = 25f;
    public float range = 15f;
    public float fireRate = 0.5f;
    
    public GameObject muzzleFlashPrefab;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptyClickSound;
}