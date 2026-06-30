using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    private Action<EnemyController> _onDeathCallback;

    private DamageTextManager _damageText;
    private EnemyData _config;
    private EnemyAttack _attackLogic;
    private Transform _playerTransform;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _capsuleCollider;
    private int _scoreReward;

    private float _currentHealth;
    private float _currentAttack;
    private float _nextAttackTime;
    private float _distanceToPlayer;
    private bool _isDead;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _attackLogic = GetComponent<EnemyAttack>();
        _damageText = FindFirstObjectByType<DamageTextManager>();
    }

    public void Initialize(EnemyData newData, Action<EnemyController> release, int waveNumber)
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

        float healthMultiplier = 1f + (waveNumber - 1) * 0.10f;
        float damageMultiplier = 1f + (waveNumber - 1) * 0.05f;
        _currentHealth = _config.MaxHealth * healthMultiplier;
        _currentAttack = _config.Damage * damageMultiplier;
        _scoreReward = _config.ScoreReward;
        _nextAttackTime = 0f;
        _isDead = false;
        _rb.linearVelocity = Vector2.zero;

        FindPlayer();

    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
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
        _damageText.ShowDamage(transform.position, damage);
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
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        ScoreManager score = FindFirstObjectByType<ScoreManager>();
        score.AddScore(_scoreReward);
        
        if (spawner != null)
        {
            spawner.OnEnemyKilled();
        }
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;
        _animator.SetTrigger("Death");
        _capsuleCollider.enabled = false;

        StartCoroutine(WaitForDeathAnimationCoroutine());
    }
    private IEnumerator WaitForDeathAnimationCoroutine()
    {
        yield return new WaitForEndOfFrame();

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);

        ParticleManager particle = FindFirstObjectByType<ParticleManager>();
        particle.SpawnExperience(transform.position, 1);
        _onDeathCallback?.Invoke(this);
    }

    private void OnDrawGizmosSelected()
    {
        if (_config == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _config.AttackRadius);
    }
}
