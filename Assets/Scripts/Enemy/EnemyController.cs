using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("UI HP")]
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Vanish Settings")]
    [SerializeField] private float _visibleDuration = 2f;
    [SerializeField] private float _fadeDuration = 0.5f;

    [Header("Avoidance")]
    [SerializeField] private float _avoidanceForce = 3f;
    private float _avoidanceRadius;

    private Coroutine _fadeCoroutine;

    [Header("Animator")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    private Action<EnemyController> _onDeathCallback;

    private EnemyData _config;
    private EnemyAttack _attackLogic;
    private Transform _playerTransform;
    private PlayerStats _playerStats;
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


    public void Initialize(EnemyData newData, Action<EnemyController> release, PlayerStats player)
    {
        _config = newData;
        _onDeathCallback = release;
        gameObject.SetActive(true);
        int waveNumber = EnemySpawner.Instance.GetCurrentWave;

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
            float maxDimension = Mathf.Max(_capsuleCollider.size.x, _capsuleCollider.size.y);
            _avoidanceRadius = (maxDimension / 2f) * 1.2f;
        }
        if (_enemyLoot != null)
        {
            _enemyLoot.InitializeLoot(_config.lootTable, _config.rarityConfig);
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
        _playerStats = player.GetComponent<PlayerStats>();

        if (_hpSlider != null)
        {
            _hpSlider.maxValue = _currentHealth;
            _hpSlider.value = _currentHealth;
        }
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
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
            StartMovement();
            MoveTowardsPlayer();
            _animator.SetBool("Run", true);
        }
        else
        {
            StopMovement();
            _animator.SetBool("Run", false);
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = ((Vector2)_playerTransform.position - _rb.position).normalized;
        Vector2 avoidanceDirection = GetAvoidanceVector();
        Vector2 finalDirection = direction;

        if (avoidanceDirection.sqrMagnitude > 0.01f)
        {
            Vector2 tangentDirection = new Vector2(-avoidanceDirection.y, avoidanceDirection.x).normalized;

            if (Vector2.Dot(tangentDirection, direction) < 0)
            {
                tangentDirection = -tangentDirection;
            }

            finalDirection = (direction + avoidanceDirection * 0.5f + tangentDirection * 0.8f).normalized;
            Vector2 noise = UnityEngine.Random.insideUnitCircle * 0.1f;
            finalDirection = (finalDirection + noise).normalized;
        }

        Vector2 targetVelocity = finalDirection * _config.Speed;
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
    public void ExecuteAoEDamage()
    {
        if (_isDead || _config == null) return;
        _attackLogic.PerformAoEAttack(_rb.position, _config.AttackRadius, (int)_currentAttack, _config.PlayerLayer);
    }

    public void TakeDamage(float damage, bool isCrit)
    {
        if (_isDead) return;
        _currentHealth -= damage;

        if (GameStatsManager.Instance != null)
        {
            GameStatsManager.Instance.AddDamage(damage);
        }

        if (_hpSlider != null)
        {
            _hpSlider.value = _currentHealth;
        }
        if (_canvasGroup != null)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(FadeHPBarRoutine());
        }
        Color color;
        if (isCrit)
        {
            color = new Color32(255, 40, 25, 255);
        }
        else
        {
            color = new Color32(253, 145, 140, 255);
        }
        DropingTextManager.Instance.ShowDropingText(transform.position, damage, color, isCrit);
        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            _animator.SetTrigger("Hit");
        }
    }
    private IEnumerator FadeHPBarRoutine()
    {
        _canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(_visibleDuration);

        float timer = 0f;
        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        _fadeCoroutine = null;
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

        float luckChance = _playerStats.LootChance;
        _enemyLoot.TryDropLoot(luckChance);

        if (_config != null && CoinPoolManager.Instance != null)
        {
            foreach (var coinGroup in _config.Coins)
            {
                if (coinGroup.coinType == null) continue;

                for (int i = 0; i < coinGroup.count; i++)
                {
                    CoinPoolManager.Instance.SpawnCoin(transform.position, coinGroup.coinType);
                }
            }
        }

        _onDeathCallback?.Invoke(this);
    }

    private Vector2 GetAvoidanceVector()
    {
        Vector2 avoidance = Vector2.zero;
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, _avoidanceRadius);
        int neighborCount = 0;

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject != this.gameObject && neighbor.CompareTag("Enemy"))
            {
                Vector2 awayFromNeighbor = (Vector2)transform.position - (Vector2)neighbor.transform.position;
                float distance = awayFromNeighbor.magnitude;

                if (distance > 0)
                {
                    float strength = Mathf.Clamp01(1f - (distance / _avoidanceRadius));
                    avoidance += awayFromNeighbor.normalized * strength;
                    neighborCount++;
                }
            }
        }

        if (neighborCount > 0)
        {
            Vector2 finalAvoidance = avoidance * _avoidanceForce;

            if (finalAvoidance.sqrMagnitude < 0.05f)
                return Vector2.zero;

            return finalAvoidance;
        }

        return Vector2.zero;
    }
    private void StopMovement()
    {
        if (_rb == null) return;

        _rb.linearVelocity = Vector2.zero;

        _rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
    }

    private void StartMovement()
    {
        if (_rb == null) return;

        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnDrawGizmosSelected()
    {
        if (_config == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _config.AttackRadius);
    }
}
