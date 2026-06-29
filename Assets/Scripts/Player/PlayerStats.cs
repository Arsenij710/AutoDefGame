using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.Interpolators;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1.0f; 
    private bool isInvincible = false;

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

    private UIHPBar _hpBar;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;
    private Color originalColor;
    public float flashInterval = 0.1f;
    private int _currentHealth;
    private bool isDead = false;

    private int _healthUpgradesCount = 0;
    private int _damageUpgradesCount = 0;

    
    public int MaxHealth => _config.baseMaxHealth + (_healthUpgradesCount * PlayerData.HealthBonusPerLevel);
    public int Damage => _config.baseDamage + (_damageUpgradesCount * PlayerData.DamageBonusPerLevel);
    public int CurrentHealth => _currentHealth;

    private void Awake()
    {
        _hpBar = FindAnyObjectByType<UIHPBar>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _levelText.text = currentLevel.ToString();
        originalColor = _spriteRenderer.color;
    }
    private void Start()
    {
        _currentHealth = MaxHealth;

        if (_hpBar != null)
        {
            _hpBar.SetupMaxHealth(MaxHealth);
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
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.2f) + 50;


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
        if (isInvincible) return;
        if (isDead) return;

        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
        _animator.SetTrigger("Hurt");

        if (_hpBar != null)
        {
            _hpBar.UpdateHealthBar(_currentHealth);
        }

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

        if (_hpBar != null)
        {
            _hpBar.UpdateHealthBar(_currentHealth);
        }
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
        UIManager uiManager = FindFirstObjectByType<UIManager>();

        if (uiManager != null)
        {
            uiManager.StartCoroutine(uiManager.TriggerGameOver());
        }
    }
}
