using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Delay Settings")]
    [SerializeField] private float _delayBeforeAttack = 0.08f;
    private float _timeSinceStopped;

    [SerializeField] private float _damageSpreadPercent = 0.1f;
    [SerializeField] private PlayerData _config;
    [SerializeField] private LayerMask _enemyLayer;

    private Animator _anim;
    private PlayerStats _stats;
    private AudioManager _audio;
    private float _cooldownTimer = 0f;
    private Vector2 _lastDirection = Vector2.right;

    private int _attackSpeedUpgradesCount = 0;
    private int _radiusUpgradesCount = 0;
    public float AttackSpeed => _config.attackCooldown - (_attackSpeedUpgradesCount * PlayerData.AttackSpeedBonusPerLevel);
    public float Radius => _config.attackRadius + (_radiusUpgradesCount * PlayerData.RadiusBonusPerLevel);
    public float Offset => _config.attackOffset + (_radiusUpgradesCount * PlayerData.OffsetBonusPerLevel);


    private void Awake()
    {
        _stats = GetComponent<PlayerStats>();
        _anim = GetComponent<Animator>();
        _audio = FindFirstObjectByType<AudioManager>();
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

        bool isStoppingCompletely = Mathf.Abs(moveX) < 0.01f && Mathf.Abs(moveY) < 0.01f;
        if (isStoppingCompletely && _timeSinceStopped >= _delayBeforeAttack && Time.time >= _cooldownTimer)
        {
            _anim.SetTrigger("Attack");
            _cooldownTimer = Time.time + AttackSpeed;
        }
    }
    public void UpgradeAttackSpeed()
    {
        _attackSpeedUpgradesCount++;
    }
    public void UpgradeRadius()
    {
        _radiusUpgradesCount++;
    }
    public void DealDamageEvent()
    {
        Vector2 attackPoint = (Vector2)transform.position + (_lastDirection * Offset);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, Radius, _enemyLayer);
        _audio.PlayPlayerHit();

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.TryGetComponent<EnemyController>(out var enemy))
            {
                enemy.TakeDamage(GetRandomDamage());
            }
        }
    }
    public int GetRandomDamage()
    {
        int baseDamage = _stats.Damage;

        float spread = baseDamage * _damageSpreadPercent;

        float minDamage = baseDamage - spread;
        float maxDamage = baseDamage + spread;

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
}
