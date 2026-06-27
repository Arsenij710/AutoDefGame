using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    private EnemyData _config;
    private EnemyAttack _attackLogic;
    private Transform _playerTransform;
    private Rigidbody2D _rb;

    private float _currentHealth;
    private float _nextAttackTime;
    private bool _isDead;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _attackLogic = GetComponent<EnemyAttack>();
    }

    public void Initialize(EnemyData newData)
    {
        _config = newData;

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

        _currentHealth = _config.MaxHealth;
        _nextAttackTime = 1f;
        _isDead = false;
        _rb.linearVelocity = Vector2.zero;

        FindPlayer();

        gameObject.SetActive(true);
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    private void FixedUpdate()
    {
        if (_isDead || _playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(_rb.position, _playerTransform.position);

        if (distanceToPlayer > _config.StoppingDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            _rb.linearVelocity = Vector2.zero; 
            TryAttack();
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
        _attackLogic.PerformAoEAttack(_rb.position, _config.AttackRadius, _config.Damage, _config.PlayerLayer);
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;

        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (_config == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _config.AttackRadius);
    }
}
