using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : MonoBehaviour
{
    [Header("Damage Numbers UI")]
    [SerializeField] private DamageDisappear _damagePrefab;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    private IObjectPool<DamageDisappear> _damagePool;

    private EnemyData _config;
    private EnemyAttack _attackLogic;
    private Transform _playerTransform;
    private Rigidbody2D _rb;
    private Canvas _canvas;
    private CapsuleCollider2D _capsuleCollider;

    private float _currentHealth;
    private float _nextAttackTime;
    private float _distanceToPlayer;
    private bool _isDead;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _attackLogic = GetComponent<EnemyAttack>();
        _canvas = GameObject.FindWithTag("EffectsCanvas").GetComponent<Canvas>();
        _damagePool = new ObjectPool<DamageDisappear>(
            createFunc: () => Instantiate(_damagePrefab, _canvas.transform),            
            actionOnGet: (dmg) => dmg.gameObject.SetActive(true),            
            actionOnRelease: (dmg) => dmg.gameObject.SetActive(false),    
            actionOnDestroy: (dmg) => Destroy(dmg.gameObject),    
            collectionCheck: true,
            defaultCapacity: 20,               
            maxSize: 50                        
        );
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

        if (_capsuleCollider != null && _config != null)
        {
            _capsuleCollider.size = _config.colliderSize;
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
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
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
        int isDamageVisible = PlayerPrefs.GetInt("ShowDamageNumbers", 1);
        if (_damagePrefab != null && isDamageVisible == 1)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;

            DamageDisappear damagePopup = _damagePool.Get();

            damagePopup.transform.position = spawnPos;

            damagePopup.Setup(damage, _damagePool);
        }
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.OnEnemyKilled();
        }
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
