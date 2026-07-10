using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.STP;
using UnityEngine.Rendering.Universal;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    private Action<EnemyController> _onDeathCallback;

    private EnemyData _config;
    private EnemyAttack _attackLogic;
    private Transform _playerTransform;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _capsuleCollider;
    private EnemyLoot _enemyLoot;
    private int _scoreReward;

    private float _currentHealth;
    private float _currentAttack;
    private int _rewardExp;
    private float _nextAttackTime;
    private float _distanceToPlayer;
    private bool _isDead;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _attackLogic = GetComponent<EnemyAttack>();
        _enemyLoot = GetComponent<EnemyLoot>();
    }

    public void Initialize(EnemyData newData, Action<EnemyController> release, int waveNumber, PlayerStats player)
    {
        _config = newData;
        _onDeathCallback = release;
        gameObject.SetActive(true);

        if (_spriteRenderer != null && _config.EnemySprite != null)
        {
            _spriteRenderer.sprite = _config.EnemySprite;
            _spriteRenderer.size = _config.SpriteSize;
        }

        if (_animator != null)
        {
            _animator.runtimeAnimatorController = _config.Animator;

            _animator.Rebind();
            _animator.Update(0f);
        }

        if (_capsuleCollider != null && _config != null)
        {
            _capsuleCollider.enabled = true;
            _capsuleCollider.size = _config.colliderSize;
        }
        if (_enemyLoot != null)
        {
            _enemyLoot.InitializeLoot(_config.lootTable);
        }

        float healthMultiplier = Mathf.Pow(1.10f, waveNumber - 1);
        float damageMultiplier = Mathf.Pow(1.08f, waveNumber - 1);
        float expMultiplier = Mathf.Pow(1.07f, waveNumber - 1);

        _currentHealth = _config.MaxHealth * healthMultiplier;
        _currentAttack = _config.Damage * damageMultiplier;
        _scoreReward = _config.ScoreReward;
        _rewardExp = Mathf.RoundToInt(_config.BaseExp * expMultiplier);
        _nextAttackTime = 0f;
        _isDead = false;
        _rb.linearVelocity = Vector2.zero;

        _playerTransform = player.transform;

    }

    private void Update()
    {
        if (_isDead || _playerTransform == null) return;

        _distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

        _nextAttackTime += Time.deltaTime;
        if (_nextAttackTime > _config.AttackCooldown)
        {
            if (_animator != null && _distanceToPlayer <= _config.AttackRadius)
            {
                TryAttack();
                _animator.SetTrigger("Attack");
            }

            _nextAttackTime = 0;
        }
    }
    private void FixedUpdate()
    {
        if (_isDead || _playerTransform == null) return;


        if (_distanceToPlayer > _config.StoppingDistance)
        {
            MoveTowardsPlayer();
            _animator.SetBool("Run", true);
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            _animator.SetBool("Run", false);
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = ((Vector2)_playerTransform.position - _rb.position).normalized;
        Vector2 targetVelocity = direction * _config.Speed;

        _rb.linearVelocity = targetVelocity;

        if (direction.x > 0.01f)
        {
            _spriteRenderer.transform.localScale = new Vector3(1f,1f, 1f);
        }
        else if (direction.x < -0.01f)
        {
            _spriteRenderer.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
    private void TryAttack()
    {
        _nextAttackTime += Time.deltaTime;
        if (_nextAttackTime > _config.AttackCooldown)
        {
            if (_animator != null)
            {
                _animator.SetTrigger("Attack");
            }

            _nextAttackTime = 0;
        }
    }
    public void ExecuteAoEDamage()
    {
        if (_isDead || _config == null) return;
        _attackLogic.PerformAoEAttack(_rb.position, _config.AttackRadius, (int)_currentAttack, _config.PlayerLayer);
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;
        _currentHealth -= damage;
        Color color = new Color32(253, 145, 140, 255);
        DamageTextManager.Instance.ShowDamage(transform.position, damage, color);
        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            _animator.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        ScoreManager.Instance.AddScore(_scoreReward);
        EnemySpawner.Instance.OnEnemyKilled();
        
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;
        _animator.SetTrigger("Death");
        AudioManager.Instance.PlayEnemyDeath();
        _capsuleCollider.enabled = false;

        StartCoroutine(WaitForDeathAnimationCoroutine());
    }
    private IEnumerator WaitForDeathAnimationCoroutine()
    {
        yield return new WaitForEndOfFrame();

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);

        ParticleManager.Instance.SpawnExperience(transform.position, _rewardExp);
        _enemyLoot.TryDropLoot();

        _onDeathCallback?.Invoke(this);
    }

    private void OnDrawGizmosSelected()
    {
        if (_config == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _config.AttackRadius);
    }
}
