using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStats : MonoBehaviour
{
    public event Action OnStatsChanged;
    public event Action OnHealthChangedEvent;
    public event Func<bool> OnPreventDeath;
    public event Action OnDefenseChanged;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1.0f; 
    public bool isInvincible { get; set; } = false;
    private float _nextRegenTime;

    [Header("DeathSettings")]
    public float delayBeforeUI = 1.2f;

    [Header("Level System")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    [Header("Update Panel")]
    [SerializeField] private UpgradeManager _upgrade;

    [Header("Config")]
    [SerializeField] private PlayerData _config;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;
    private Color originalColor;
    private float _minAttackSpeed = 2.0f;
    public float flashInterval = 0.1f;
    private float _currentHealth;
    private bool isDead = false;

    private int _damageUpgradesCount = 0;
    private int _healthUpgradesCount = 0;
    private int _defenceUpgradesCount = 0;
    private int _hpRegenUpgradesCount = 0;
    private int _missUpgradesCount = 0;
    private int _luckUpgradesCount = 0;
    private int _attackSpeedUpgradesCount = 0;
    private int _radiusUpgradesCount = 0;
    private int _critChanceUpgradesCount = 0;
    private int _critDamageUpgradesCount = 0;
    private int _reAttackUpgradesCount = 0;
    private int _vampirismUpgradesCount = 0;

    private float _artFlatHP;
    private float _artPercentHP;
    private float _artFlatAttack;
    private float _artPercentAttack;
    private float _artFlatDefence;
    private float _artPercentDefence;
    private float _artCritChance;
    private float _artCritDamage;
    private float _artMissChance;
    private float _artHpRegen;
    private float _artAttackSpeed;
    private float _artDoubleAttack;
    private float _artVampirism;

    private float _damagePercentModifier = 0;
    private float _damageFlatModifier = 0;
    private float _hpModifier = 0;
    private float _defenceModifier = 0;
    private float _attackSpeedeModifier = 0; 
    private float _critChanceModifier = 0f;
    private float _critDamageModifier = 0f;

    public float BaseMaxHealth
    {
        get
        {
            float percentageBonus = PlayerData.HealthBonusPerLevel;

            float multiplier = Mathf.Pow(1f + percentageBonus, _healthUpgradesCount);

            return _config.baseMaxHealth * multiplier;
        }
    }

    public float MaxHealth => (BaseMaxHealth + _artFlatHP) * (1f + (_artPercentHP / 100f) + _hpModifier);
    public float BaseDamage
    {
        get
        {
            float currentDamage = _config.baseDamage;
            float percent = PlayerData.DamageBonusPerLevel;
            int flatBonus = 5;

            float exponentialDamage = currentDamage * Mathf.Pow(1f + percent, _damageUpgradesCount);

            float totalFlatBonus = _damageUpgradesCount * flatBonus;

            currentDamage = exponentialDamage + totalFlatBonus;

            return currentDamage;
        }
    }
    public float Damage => (BaseDamage + _artFlatAttack + _damageFlatModifier) * (1f + (_artPercentAttack / 100f) + _damagePercentModifier) ;
    public float BaseDefence
    {
        get
        {
            float percentageBonus = PlayerData.DefenceBonusPerLevel;
            float multiplier = Mathf.Pow(1f + percentageBonus, _defenceUpgradesCount);
            return _config.baseDefence * multiplier;
        }
    }
    public float Defence => (BaseDefence + _artFlatDefence) * (1f + (_artPercentDefence / 100f) + _defenceModifier);
    public float CurrentHealth => _currentHealth;
    public float BaseHpRegenPercent => _config.baseHPRegen + (PlayerData.HPRegenBonusPerLevel * _hpRegenUpgradesCount);
    public float HpRegenPercent => BaseHpRegenPercent + _artHpRegen;
    public float TotalHpPerSecond => Mathf.Clamp(MaxHealth * HpRegenPercent / 100, 0, MaxHealth * 0.50f);
    public float BaseTotalDodgeChance => _config.baseMiss + _missUpgradesCount * PlayerData.MissBonusPerLevel;
    public float TotalDodgeChance => BaseTotalDodgeChance + _artMissChance;
    public float GoldMultiplier => _config.baseLuck + (PlayerData.LuckGoldBonusPerLevel * _luckUpgradesCount);
    public float LootChance => _config.baseLuck * (PlayerData.LuckLootBonusPerLevel * _luckUpgradesCount);
    public float Luck => _config.baseLuck + _luckUpgradesCount;
    public float BaseAttackSpeed => _config.AttackSpeed + (_attackSpeedUpgradesCount * PlayerData.AttackSpeedBonusPerLevel);
    public float AttackSpeed => BaseAttackSpeed + _artAttackSpeed + _attackSpeedeModifier;
    public float AttackSpeedDelay
    {
        get
        {
            float currentCooldown = _minAttackSpeed / (AttackSpeed / _config.AttackSpeed);
            return Mathf.Max(currentCooldown, 0.2f);
        }
    }
    public float AnimSpeedMultiplier => AttackSpeed * 2 / _config.AttackSpeed;
    public float Radius => _config.attackRadius + (_radiusUpgradesCount * PlayerData.RadiusBonusPerLevel);
    public float Offset => _config.attackOffset + (_radiusUpgradesCount * PlayerData.OffsetBonusPerLevel);
    public float BaseCritChance => _config.baseCritChance + (PlayerData.CritChanceBonusPerLevel * _critChanceUpgradesCount);
    public float CritChance => BaseCritChance + _artCritChance + _critChanceModifier;
    public float BaseCritDamage => _config.baseCritDamage + (PlayerData.CritDamageBonusPerLevel * _critDamageUpgradesCount);
    public float CritDamage => (BaseCritDamage + _artCritDamage + _critDamageModifier) / 100;
    public float BaseDoubleStrikeChance => _config.baseReAttack + (PlayerData.ReAttackBonusPerLevel * _reAttackUpgradesCount);
    public float TotalDoubleStrikeChance => BaseDoubleStrikeChance + _artDoubleAttack;
    public float Speed => _config.baseMoveSpeed;
    public float BaseVampirism => _config.baseVampirism + (PlayerData.VampirismBonusPerLevel * _vampirismUpgradesCount);
    public float Vampirism => BaseVampirism + _artVampirism;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        originalColor = _spriteRenderer.color;
    }
    private void Start()
    {
        RecalculateArtifactStats();
        _currentHealth = MaxHealth;
        UIHPBar.Instance.SetupMaxHealth(MaxHealth);
        UIExpBar.Instance.UpdateExpBar(currentExp, expToNextLevel, currentLevel);
        StartCoroutine(RegenerationRoutine());
    }

    public void RecalculateArtifactStats()
    {
        float oldMaxHealth = MaxHealth;

        _artFlatHP = 0;
        _artPercentHP = 0;
        _artFlatAttack = 0;
        _artPercentAttack = 0;
        _artFlatDefence = 0;
        _artPercentDefence = 0;
        _artCritChance = 0;
        _artCritDamage = 0;
        _artMissChance = 0;
        _artHpRegen = 0;
        _artDoubleAttack = 0;
        _artVampirism = 0;

        if (ArtifactInventory.Instance != null)
        {
            foreach (var artifact in ArtifactInventory.Instance.equipmentSlots.Values)
            {
                if (artifact == null) continue;

                ProcessStat(artifact.mainStat.type, artifact.mainStat.value);

                if (artifact.subStats != null)
                {
                    foreach (var subStat in artifact.subStats)
                    {
                        ProcessStat(subStat.type, subStat.value);
                    }
                }
            }
        }
        float newMaxHealth = MaxHealth;
        if (oldMaxHealth > 0 && newMaxHealth != oldMaxHealth)
        {
            float healthMultiplier = newMaxHealth / oldMaxHealth;
            _currentHealth *= healthMultiplier;
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, newMaxHealth);
        }

        UIHPBar.Instance.SetupMaxHealth(MaxHealth);
        UIHPBar.Instance.UpdateHealthBar(_currentHealth);
    }
    public void NotifyStatsChanged()
    {
        OnStatsChanged?.Invoke();
        OnDefenseChanged?.Invoke();
    }
    void ProcessStat(StatType type, float value)
    {
        switch (type)
        {
            case StatType.FlatHP: _artFlatHP += value; break;
            case StatType.PercentHP: _artPercentHP += value; break;
            case StatType.FlatAttack: _artFlatAttack += value; break;
            case StatType.PercentAttack: _artPercentAttack += value; break;
            case StatType.FlatDefence: _artFlatDefence += value; break;
            case StatType.PercentDefence: _artPercentDefence += value; break;
            case StatType.CritChance: _artCritChance += value; break;
            case StatType.CritDamage: _artCritDamage += value; break;
            case StatType.MissChance: _artMissChance+= value; break;
            case StatType.HpRegen: _artHpRegen += value; break;
            case StatType.DoubleAttack: _artDoubleAttack += value; break;
            case StatType.Vampirism: _artVampirism += value; break;
        }
    }

    private IEnumerator RegenerationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (Time.time >= _nextRegenTime && _currentHealth < MaxHealth)
            {
                _currentHealth += TotalHpPerSecond;
                _currentHealth = Mathf.Min(_currentHealth, MaxHealth);
                UIHPBar.Instance.UpdateHealthBar(_currentHealth);
            }
        }
    }
    public void AddExperience(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        UIExpBar.Instance.UpdateExpBar(currentExp, expToNextLevel, currentLevel);
    }
    private void LevelUp()
    {
        currentExp -= expToNextLevel;
        currentLevel++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.15f) + 50;


        if (_upgrade != null)
        {
            _upgrade.OpenUpgradePanel();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isInvincible) return;
        if (isDead) return;
        float roll = UnityEngine.Random.Range(0f, 100f);
        Color color;
        if (roll <= TotalDodgeChance)
        {
            color = new Color32(100, 250, 220, 255);
            DropingTextManager.Instance.ShowDropingText(transform.position, damageAmount, color, isMiss:true);
            return;
        }

        color = new Color(1f, 1f, 1f);
        float damageReduction = 15000f / (15000f + Defence);
        float finalDamage = damageAmount * damageReduction;
        finalDamage = Mathf.Max(finalDamage, 1f);

        _currentHealth -= finalDamage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
        OnHealthChangedEvent?.Invoke();
        _nextRegenTime = Time.time + invincibilityDuration;
        _animator.SetTrigger("Hurt");

        if (GameStatsManager.Instance != null)
        {
            GameStatsManager.Instance.AddReceivedDamage(finalDamage);
        }

        DropingTextManager.Instance.ShowDropingText(transform.position, finalDamage, color);

        if (_currentHealth <= 0)
        {
            bool isSaved = OnPreventDeath?.Invoke() ?? false;

            if (isSaved)
            {
                UIHPBar.Instance.UpdateHealthBar(_currentHealth);
            }
            else
            {
                UIHPBar.Instance.UpdateHealthBar(0);
                Die();
                return;
            }
        }
        else
        {
            UIHPBar.Instance.UpdateHealthBar(_currentHealth);
        }

        StartCoroutine(BecomeInvincibleCoroutine());
    }
    private IEnumerator BecomeInvincibleCoroutine()
    {
        isInvincible = true;
        Color clr = originalColor;

        yield return new WaitForSeconds(0.4f);
        float timer = 0.4f;

        while (timer < invincibilityDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0.2f, 1.0f, (Mathf.Sin(timer * 25f) + 1f) / 2f);

            clr.a = alpha;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = clr;
            }

            yield return null;
        }

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = originalColor;
        }

        isInvincible = false;
    }
    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
        OnHealthChangedEvent?.Invoke();
        Color color = new Color(0.168f, 0.938f, 0.294f);
        DropingTextManager.Instance.ShowDropingText(transform.position, healAmount, color);
        UIHPBar.Instance.UpdateHealthBar(_currentHealth);
    }

    private void Die()
    {
        isDead = true;

        var movement = GetComponent<PlayerMovement>();
        var attack = GetComponent<PlayerAttack>();
        _rb.linearVelocity = Vector2.zero;
        if (movement != null) movement.enabled = false;
        if (attack != null) attack.enabled = false;
        _animator.SetTrigger("Death");
        AudioManager.Instance.PlayPlayerDeath();

        FreezeAllEnemies();
        StartCoroutine(GameOverCoroutine());
    }
    private void FreezeAllEnemies()
    {
        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.enabled = false;
                enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }

            var enemyAnim = enemy.GetComponentInChildren<Animator>();
            if (enemyAnim != null)
            {
                enemyAnim.SetTrigger("Ability");
            }

            var enemyCollider = enemy.GetComponent<CapsuleCollider2D>();
            if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }
        }
    }
    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeUI);

        UIManager.Instance.StartCoroutine(UIManager.Instance.TriggerGameOver());
    }
    public void UpgradeMaxHealth()
    {
        float oldMaxHealth = MaxHealth;
        _healthUpgradesCount++;
        float healthMultiplier = MaxHealth / oldMaxHealth;
        _currentHealth = _currentHealth * healthMultiplier;
        
        UIHPBar.Instance.SetupMaxHealth(MaxHealth);
        UIHPBar.Instance.UpdateHealthBar(_currentHealth);
        
    }
    public void HPRegenUpgrade()
    {
        _hpRegenUpgradesCount++;
    }
    public void MissUpgrade()
    {
        _missUpgradesCount++;
    }
    public void LuckUpgrade()
    {
        _luckUpgradesCount++;
    }
    public void UpgradeDamage()
    {
        _damageUpgradesCount++;
    }
    public void UpgradeDefence()
    {
        _defenceUpgradesCount++;
        OnDefenseChanged?.Invoke();
    }
    public void UpgradeAttackSpeed()
    {
        _attackSpeedUpgradesCount++;
    }
    public void UpgradeRadius()
    {
        _radiusUpgradesCount++;
    }
    public void CritChanceUpgrade()
    {
        _critChanceUpgradesCount++;
    }
    public void CritDamageUpgrade()
    {
        _critDamageUpgradesCount++;
    }
    public void ReAttackUpgrade()
    {
        _reAttackUpgradesCount++;
    }
    public void VampirismUpgrade()
    {
        _vampirismUpgradesCount++;
    }

    public void ModifyDamage(float bonusMultiplier, bool isPercent = true)
    {
        if (isPercent)
        {
            _damagePercentModifier += bonusMultiplier;
            _damagePercentModifier = Mathf.Max(_damagePercentModifier, 0f);
        }
        else
        {
            _damageFlatModifier += bonusMultiplier;
            _damageFlatModifier = Mathf.Max(_damageFlatModifier, 0f);
        }
        OnStatsChanged?.Invoke();
    }
    public void ModifyHp(float bonusMultiplier)
    {
        _hpModifier += bonusMultiplier;
        _hpModifier = Mathf.Max(_hpModifier, 0f);
    }
    public void ModifyDefence(float bonusMultiplier)
    {
        _defenceModifier += bonusMultiplier;
        _defenceModifier = Mathf.Max(_defenceModifier, 0f);
        OnDefenseChanged?.Invoke();
    }
    public void ModifyCritDamage(float bonusMultiplier)
    {
        _critDamageModifier += bonusMultiplier;
        _critDamageModifier = Mathf.Max(_critDamageModifier, 0f);
    }
    public void ModifyCritChance(float bonusMultiplier)
    {
        _critChanceModifier += bonusMultiplier;
        _critChanceModifier = Mathf.Max(_critChanceModifier, 0f);
    }
    public void ModifyAttackSpeed(float bonusAttackSpeed)
    {
        _attackSpeedeModifier += bonusAttackSpeed;
        _attackSpeedeModifier =  Mathf.Max(_attackSpeedeModifier, -100f);
    }
}
