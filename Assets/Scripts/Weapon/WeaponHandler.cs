using System.Collections;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private RadioSystem radioSystem;

    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private GameObject weaponMeshObject;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask shootableLayers;
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private bool hasWeapon = false;
    [SerializeField] private bool isHolstered = false;
    
    [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private KeyCode holsterKey = KeyCode.H;
    
    public WeaponData CurrentWeapon => currentWeapon;

    private int _currentAmmo;
    private int _reserveAmmo;
    private float _nextTimeToShoot;
    private bool _isAiming;
    private bool _isReloading;

    public bool HasWeapon => hasWeapon;
    public bool IsHolstered => isHolstered;
    public bool IsAiming => _isAiming;
    public bool IsReloading => _isReloading;
    
    public int CurrentAmmo => _currentAmmo;
    public int ReserveAmmo => _reserveAmmo;

    private void Awake()
    {
        UpdateMeshState();
    }
    
    private void Start()
    {
        if (currentWeapon != null && hasWeapon)
        {
            _currentAmmo = currentWeapon.magazineCapacity;
            _reserveAmmo = currentWeapon.initialReserveAmmo;
        }

        UpdateMeshState();
    }

    private void Update()
    {
        if (radioSystem != null && radioSystem.IsRadioActive)
        {
            _isAiming = false;
            return;
        }
        
        if (!hasWeapon) return;
        
        if (Input.GetKeyDown(holsterKey) && !_isReloading)
        {
            ToggleHolster();
            return;
        }
        
        if (isHolstered || _isReloading)
        {
            _isAiming = false;
            return;
        }

        // 5. RECARGA (Tecla R)
        if (Input.GetKeyDown(reloadKey) && _currentAmmo < currentWeapon.magazineCapacity && _reserveAmmo > 0)
        {
            StartCoroutine(ReloadRoutine());
            return;
        }
        
        _isAiming = Input.GetKey(aimKey);

        if (_isAiming && Input.GetKeyDown(shootKey) && Time.time >= _nextTimeToShoot)
        {
            _nextTimeToShoot = Time.time + currentWeapon.fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (currentWeapon == null) return;

        if (_currentAmmo <= 0)
        {
            PlaySound(currentWeapon.emptyClickSound);
            return;
        }

        _currentAmmo--; 
        PlaySound(currentWeapon.shootSound);

        if (currentWeapon.muzzleFlashPrefab != null && firePoint != null)
        {
            GameObject flash = Instantiate(currentWeapon.muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(flash, 0.2f);
        }

        if (currentWeapon.isRaycast && firePoint != null)
        {
            Debug.DrawRay(firePoint.position, firePoint.forward * currentWeapon.range, Color.red, 2f);

            if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, currentWeapon.range, shootableLayers))
            {
                Debug.DrawLine(firePoint.position, hit.point, Color.green, 2f);

                IDamageable target = hit.collider.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(currentWeapon.damage);
                }
            }
        }
    }

    private IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        _isAiming = false;

        PlaySound(currentWeapon.reloadSound);

        float reloadTime = currentWeapon.reloadSound != null ? currentWeapon.reloadSound.length : 1.5f;
        yield return new WaitForSeconds(reloadTime);
        
        int ammoNeeded = currentWeapon.magazineCapacity - _currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, _reserveAmmo);

        _currentAmmo += ammoToReload;
        _reserveAmmo -= ammoToReload;

        _isReloading = false;
    }
    
    public bool AddAmmo(int amount)
    {
        if (!hasWeapon || currentWeapon == null) return false;
        if (_reserveAmmo >= currentWeapon.maxReserveAmmo) return false; 

        _reserveAmmo = Mathf.Clamp(_reserveAmmo + amount, 0, currentWeapon.maxReserveAmmo);
        return true;
    }

    public void ToggleHolster()
    {
        if (!hasWeapon) return;

        isHolstered = !isHolstered;
        _isAiming = false;
        UpdateMeshState();
    }

    private void UpdateMeshState()
    {
        if (weaponMeshObject != null)
        {
            bool shouldBeActive = hasWeapon && !isHolstered;
            weaponMeshObject.SetActive(shouldBeActive);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        hasWeapon = true;
        isHolstered = false;

        _currentAmmo = newWeapon.magazineCapacity;
        _reserveAmmo = newWeapon.initialReserveAmmo;

        UpdateMeshState();
    }
}