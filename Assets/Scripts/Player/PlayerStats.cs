using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStats : MonoBehaviour
{
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
    [SerializeField] private Slider _expSlider;
    [SerializeField] private TMP_Text _levelText;

    [Header("Update Panel")]
    [SerializeField] private UpgradeManager _upgrade;

    [SerializeField] private PlayerData _config;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;
    private Color originalColor;
    public float flashInterval = 0.1f;
    private float _currentHealth;
    private bool isDead = false;

    private int _healthUpgradesCount = 0;
    private int _hpRegenUpgradesCount = 0;
    private int _missUpgradesCount = 0;
    

    
    public float MaxHealth
    {
        get
        {
            float percentageBonus = PlayerData.HealthBonusPerLevel;

            float multiplier = Mathf.Pow(1f + percentageBonus, _healthUpgradesCount);

            return _config.baseMaxHealth * multiplier;
        }
    }
    
    public float CurrentHealth => _currentHealth;
    public float HpRegenPercent => (_config.baseHPRegen + PlayerData.HPRegenBonusPerLevel * _hpRegenUpgradesCount);
    public float TotalHpPerSecond => Mathf.Clamp(MaxHealth * HpRegenPercent, 0, MaxHealth * 0.50f);
    public float TotalDodgeChance => Mathf.Clamp(_config.baseMiss + _missUpgradesCount * PlayerData.MissBonusPerLevel, 0f, 80f);


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _levelText.text = currentLevel.ToString();
        originalColor = _spriteRenderer.color;
    }
    private void Start()
    {
        _currentHealth = MaxHealth;
        UIHPBar.Instance.SetupMaxHealth(MaxHealth);
        StartCoroutine(RegenerationRoutine());
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

        UpdateExpUI();
    }
    private void LevelUp()
    {
        currentExp -= expToNextLevel;
        currentLevel++;
        _levelText.text = currentLevel.ToString();
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.15f) + 50;


        if (_upgrade != null)
        {
            _upgrade.OpenUpgradePanel();
        }
    }
    private void UpdateExpUI()
    {
        if (_expSlider != null)
        {
            _expSlider.maxValue = expToNextLevel;
            _expSlider.value = currentExp;
        }
    }

    public void UpgradeMaxHealth()
    {
        float oldMaxHealth = MaxHealth;
        _healthUpgradesCount++;
        float healthMultiplier = (float)MaxHealth / oldMaxHealth;
        _currentHealth = Mathf.RoundToInt(_currentHealth * healthMultiplier);
        
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
    

    public void TakeDamage(int damageAmount)
    {
        if (isInvincible) return;
        if (isDead) return;

        float roll = Random.Range(0f, 100f);
        Color color;
        if (roll <= TotalDodgeChance)
        {
            color = new Color32(100, 250, 220, 255);
            DropingTextManager.Instance.ShowDropingText(transform.position, damageAmount, color, isMiss:true);
            return;
        }
        color = new Color(1f, 1f, 1f);
        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
        _nextRegenTime = Time.time + invincibilityDuration;
        _animator.SetTrigger("Hurt");

        DropingTextManager.Instance.ShowDropingText(transform.position, damageAmount, color);
        UIHPBar.Instance.UpdateHealthBar(_currentHealth);

        if (_currentHealth <= 0)
        {
            Die();
            return;
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
    public void Heal(int healAmount)
    {
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
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
}
