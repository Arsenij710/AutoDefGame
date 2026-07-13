using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Delay Settings")]
    [SerializeField] private float _delayBeforeAttack = 0.08f;
    private float _timeSinceStopped;

    [Header("Re Attack")]
    [SerializeField] private float _timeBetweenStrikes = 0.45f;
    [SerializeField] private int _maxComboStrikes = 5;

    [SerializeField] private float _damageSpreadPercent = 0.1f;
    [SerializeField] private PlayerData _config;
    [SerializeField] private LayerMask _enemyLayer;

    private Animator _anim;
    private PlayerStats _stats;
    private float _cooldownTimer = 0f;
    private Vector2 _lastDirection = Vector2.right;

    private int _currentComboCount = 0;
    private bool isStoppingCompletely;
    private int _damageUpgradesCount = 0;
    private int _attackSpeedUpgradesCount = 0;
    private int _radiusUpgradesCount = 0;
    private int _critChanceUpgradesCount = 0;
    private int _critDamageUpgradesCount = 0;
    private int _reAttackUpgradesCount = 0;
    public int Damage
    {
        get
        {
            float currentDamage = _config.baseDamage;
            float percent = PlayerData.DamageBonusPerLevel;
            int flatBonus = 5;

            float exponentialDamage = currentDamage * Mathf.Pow(1f + percent, _damageUpgradesCount);

            float totalFlatBonus = _damageUpgradesCount * flatBonus;

            currentDamage = exponentialDamage + totalFlatBonus;

            return Mathf.RoundToInt(currentDamage);
        }
    }
    public float AttackSpeed => _config.attackCooldown - (_attackSpeedUpgradesCount * PlayerData.AttackSpeedBonusPerLevel);
    public float Radius => _config.attackRadius + (_radiusUpgradesCount * PlayerData.RadiusBonusPerLevel);
    public float Offset => _config.attackOffset + (_radiusUpgradesCount * PlayerData.OffsetBonusPerLevel);
    public float CritChance => (_config.baseCritChance + PlayerData.CritChanceBonusPerLevel * _critChanceUpgradesCount) * 100;
    public float CritDamage => _config.baseCritDamage + PlayerData.CritDamageBonusPerLevel * _critDamageUpgradesCount;
    public float TotalDoubleStrikeChance => Mathf.Clamp(_config.baseReAttack + PlayerData.ReAttackBonusPerLevel * _reAttackUpgradesCount, 0f, 50f);


    private void Awake()
    {
        _stats = GetComponent<PlayerStats>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        bool isMoving = Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveY) > 0.01f;
        if (isMoving)
        {
            _timeSinceStopped = 0f;
            _lastDirection = new Vector2(moveX, moveY).normalized;
        }
        else
        {
            _timeSinceStopped += Time.deltaTime;
        }

        isStoppingCompletely = Mathf.Abs(moveX) < 0.01f && Mathf.Abs(moveY) < 0.01f;
        if (isStoppingCompletely && _timeSinceStopped >= _delayBeforeAttack && Time.time >= _cooldownTimer)
        {
            Vector2 attackPoint = (Vector2)transform.position + (_lastDirection * Offset);
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, Radius, _enemyLayer);
            if (hitEnemies.Length > 0)
            {
                _currentComboCount = 0;
                _anim.SetTrigger("Attack");
                _cooldownTimer = Time.time + AttackSpeed;
            }
        }
    }
    public void DealDamageEvent()
    {
        Vector2 attackPoint = (Vector2)transform.position + (_lastDirection * Offset);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, Radius, _enemyLayer);
        AudioManager.Instance.PlayPlayerHit();

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.TryGetComponent<EnemyController>(out var enemy))
            {
                ApplySingleHit(enemy);
            }
        }
        _currentComboCount++;
        if (_currentComboCount < _maxComboStrikes)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= TotalDoubleStrikeChance)
            {
                StartCoroutine(ChainStrikeRoutine());
            }
        }
    }
    private IEnumerator ChainStrikeRoutine()
    {
        if (!isStoppingCompletely) yield break;
        
        yield return new WaitForSeconds(_timeBetweenStrikes);

        Vector2 attackPoint = (Vector2)transform.position + (_lastDirection * Offset);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, Radius, _enemyLayer);

        if (hitEnemies.Length > 0)
        {
            if (_anim != null)
            {
                _anim.Play("Attack", 0, 0f);
                _cooldownTimer = Time.time + AttackSpeed;
            }
        }
    }
    private void ApplySingleHit(EnemyController enemy)
    {
        bool isCrit = false;
        int baseDamage = GetRandomDamage();
        int finalDamage = CalculateDamage(baseDamage, out isCrit);
        enemy.TakeDamage(finalDamage, isCrit);
    }
    public int CalculateDamage(int damage, out bool isCrit)
    {
        float roll = Random.Range(0f, 100f);

        if (roll <= CritChance)
        {
            isCrit = true;
            return (int)(damage * CritDamage);
        }

        isCrit = false;
        return damage;
    }
    public int GetRandomDamage()
    {

        float spread = Damage * _damageSpreadPercent;

        float minDamage = Damage - spread;
        float maxDamage = Damage + spread;

        return Mathf.RoundToInt(Random.Range(minDamage, maxDamage));
    }

    private void OnDrawGizmosSelected()
    {
        if (_config == null) return;

        Gizmos.color = Color.red;
        Vector2 direction = _lastDirection;

        if (!Application.isPlaying) direction = Vector2.right;

        Vector2 attackPoint = (Vector2)transform.position + (direction * Offset);
        Gizmos.DrawWireSphere(attackPoint, Radius);
    }
    public void UpgradeDamage()
    {
        _damageUpgradesCount++;
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
}
