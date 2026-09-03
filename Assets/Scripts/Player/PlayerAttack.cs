using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Delay Settings")]
    [SerializeField] private float _delayBeforeAttack = 0.08f;
    private float _timeSinceStopped;

    [Header("Re Attack")]
    [SerializeField] private float _timeBetweenStrikes = 0.1f;
    [SerializeField] private int _maxComboStrikes = 5;

    [Header("Slash")]
    [SerializeField] private Animator _slashAnimator;
    [SerializeField] private string _animationStateName = "Slash";

    [Header("Slash Scale")]
    [SerializeField] private float _minScale = 0.9f;
    [SerializeField] private float _maxScale = 3.5f;

    [Header("Radius Attack")]
    [SerializeField] private float _maxRadius = 5.0f;

    [SerializeField] private float _damageSpreadPercent = 0.1f;
    [SerializeField] private PlayerData _config;
    [SerializeField] private LayerMask _enemyLayer;

    private Animator _anim;
    private PlayerStats _stats;
    private float _cooldownTimer = 0f;
    private Vector2 _lastDirection = Vector2.right;

    private int _currentComboCount = 0;
    private bool isStoppingCompletely;

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
            Vector2 attackPoint = (Vector2)transform.position + (_lastDirection * _stats.Offset);
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, _stats.Radius, _enemyLayer);
            if (hitEnemies.Length > 0)
            {
                _currentComboCount = 0;
                _anim.SetFloat("AttackSpeed", _stats.AnimSpeedMultiplier);
                _anim.SetTrigger("Attack");
                _cooldownTimer = Time.time + _stats.AttackSpeedDelay;
            }
        }
    }
    public void DealDamageEvent()
    {
        UpdateSlashScale();
        _slashAnimator.gameObject.SetActive(true);
        _slashAnimator.speed = _stats.AnimSpeedMultiplier;
        _slashAnimator.Play(_animationStateName, -1, 0f);
        _slashAnimator.Update(0f);

        Vector2 attackPoint = (Vector2)transform.position + (_lastDirection * _stats.Offset);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, _stats.Radius, _enemyLayer);
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
            if (roll <= _stats.TotalDoubleStrikeChance)
            {
                StartCoroutine(ChainStrikeRoutine());
            }
        }
    }
    private IEnumerator ChainStrikeRoutine()
    {
        if (!isStoppingCompletely) yield break;

        float currentAnimSpeed = _anim.GetFloat("AttackSpeed");
        if (currentAnimSpeed <= 0f) currentAnimSpeed = 1f;

        float actualDelay = _timeBetweenStrikes / currentAnimSpeed;
        yield return new WaitForSeconds(actualDelay);

        Vector2 attackPoint = (Vector2)transform.position + (_lastDirection * _stats.Offset);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, _stats.Radius, _enemyLayer);

        if (hitEnemies.Length > 0)
        {
            if (_anim != null)
            {
                _anim.SetFloat("AttackSpeed", _stats.AnimSpeedMultiplier);
                _anim.Play("Attack", 0, 0f);
                _cooldownTimer = Time.time + _stats.AttackSpeedDelay;
            }
        }
    }
    private void ApplySingleHit(EnemyController enemy)
    {
        bool isCrit = false;
        float baseDamage = GetRandomDamage();
        float finalDamage = CalculateDamage(baseDamage, out isCrit);
        enemy.TakeDamage(finalDamage, isCrit);

        if (_stats.Vampirism > 0)
        {
            float healAmount = finalDamage * (_stats.Vampirism / 100f);
            _stats.Heal(healAmount);
        }
    }
    public float CalculateDamage(float damage, out bool isCrit)
    {
        float roll = Random.Range(0f, 100f);

        if (roll <= _stats.CritChance)
        {
            isCrit = true;
            return damage * _stats.CritDamage;
        }

        isCrit = false;
        return damage;
    }
    public float GetRandomDamage()
    {

        float spread = _stats.Damage * _damageSpreadPercent;

        float minDamage = _stats.Damage - spread;
        float maxDamage = _stats.Damage + spread;

        return Random.Range(minDamage, maxDamage);
    }
    private void UpdateSlashScale()
    {
        if (_slashAnimator == null) return;

        float progress = Mathf.InverseLerp(_config.attackRadius, _maxRadius, _stats.Radius);
        float targetScale = Mathf.Lerp(_minScale, _maxScale, progress);

        _slashAnimator.transform.localScale = new Vector3(targetScale, targetScale, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        if (_config == null) return;

        Gizmos.color = Color.red;
        Vector2 direction = _lastDirection;

        if (!Application.isPlaying) direction = Vector2.right;

        Vector2 attackPoint = (Vector2)transform.position + (direction * _stats.Offset);
        Gizmos.DrawWireSphere(attackPoint, _stats.Radius);
    }
    
}
