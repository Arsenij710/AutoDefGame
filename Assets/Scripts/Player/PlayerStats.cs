using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerData _config; 

    private int _currentHealth;

    private int _healthUpgradesCount = 0;
    private int _damageUpgradesCount = 0;

    public static event Action<int, int> OnHealthChanged;
    
    public int MaxHealth => _config.baseMaxHealth + (_healthUpgradesCount * PlayerData.HealthBonusPerLevel);
    public int Damage => _config.baseDamage + (_damageUpgradesCount * PlayerData.DamageBonusPerLevel);
    public int CurrentHealth => _currentHealth;

    private void Start()
    {
        _currentHealth = MaxHealth;

        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
    }

    public void UpgradeMaxHealth()
    {
        _healthUpgradesCount++;
        _currentHealth += PlayerData.HealthBonusPerLevel;

        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
    }

    public void UpgradeDamage()
    {
        _damageUpgradesCount++;
    }

    public void TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth); 

        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    public void Heal(int healAmount)
    {
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);

        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
    }

    private void Die()
    {
        Debug.Log("Игрок погиб! Экран Game Over.");
    }
}
