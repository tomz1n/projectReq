using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private MenuManager menuManager;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    
    public delegate void HealthChangedHandler(float current, float max);
    public event HealthChangedHandler OnHealthChanged;

    private bool _isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        Debug.Log($"<color=red>[PLAYER SALUD]</color> Vida actual: {currentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (_isDead) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        Debug.Log("<color=black>[PLAYER MUERTO]</color> Has caído.");
        
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        WeaponHandler weapon = GetComponent<WeaponHandler>();
        if (weapon != null) weapon.enabled = false;

        if (menuManager != null)
        {
            menuManager.ShowGameOver();
        }
        else
        {
            MenuManager manager = FindObjectOfType<MenuManager>();
            if (manager != null) manager.ShowGameOver();
        }
    }
}