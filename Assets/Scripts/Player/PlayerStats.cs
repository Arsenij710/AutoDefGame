using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    [SerializeField] private PlayerData _config; 

    private UIHPBar _hpBar;
    private int _currentHealth;

    private int _healthUpgradesCount = 0;
    private int _damageUpgradesCount = 0;

    
    public int MaxHealth => _config.baseMaxHealth + (_healthUpgradesCount * PlayerData.HealthBonusPerLevel);
    public int Damage => _config.baseDamage + (_damageUpgradesCount * PlayerData.DamageBonusPerLevel);
    public int CurrentHealth => _currentHealth;

    private void Awake()
    {
        _hpBar = FindAnyObjectByType<UIHPBar>();
    }
    private void Start()
    {
        _currentHealth = MaxHealth;

        if (_hpBar != null)
        {
            _hpBar.SetupMaxHealth(MaxHealth);
        }
    }

    public void UpgradeMaxHealth()
    {
        _healthUpgradesCount++;
        _currentHealth += PlayerData.HealthBonusPerLevel;

        if (_hpBar != null)
        {
            _hpBar.SetupMaxHealth(MaxHealth);
            _hpBar.UpdateHealthBar(_currentHealth);
        }
    }

    public void UpgradeDamage()
    {
        _damageUpgradesCount++;
    }

    public void TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);

        if (_hpBar != null)
        {
            _hpBar.UpdateHealthBar(_currentHealth);
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    public void Heal(int healAmount)
    {
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);

        if (_hpBar != null)
        {
            _hpBar.UpdateHealthBar(_currentHealth);
        }
    }

    private void Die()
    {
        UIManager uiManager = FindFirstObjectByType<UIManager>();

        if (uiManager != null)
        {
            uiManager.TriggerGameOver();
        }

        //gameObject.SetActive(false);
    }
}
