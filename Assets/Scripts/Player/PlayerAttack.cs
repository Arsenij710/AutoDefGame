using TMPro.EditorUtilities;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Delay Settings")]
    [SerializeField] private float _delayBeforeAttack = 0.08f;
    private float _timeSinceStopped;

    [SerializeField] private PlayerData _config;
    [SerializeField] private LayerMask _enemyLayer;

    private Animator _anim;
    private PlayerStats _stats;
    private float _cooldownTimer = 0f;
    private Vector2 _lastDirection = Vector2.right;

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

        bool isStoppingCompletely = Mathf.Abs(moveX) < 0.01f && Mathf.Abs(moveY) < 0.01f;
        if (isStoppingCompletely && _timeSinceStopped >= _delayBeforeAttack && Time.time >= _cooldownTimer)
        {
            _anim.SetTrigger("Attack");
            _cooldownTimer = Time.time + _config.attackCooldown;
        }
    }
    public void DealDamageEvent()
    {
        Vector2 attackPoint = (Vector2)transform.position + (_lastDirection * _config.attackOffset);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, _config.attackRadius, _enemyLayer);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.TryGetComponent<EnemyController>(out var enemy))
            {
                enemy.TakeDamage(_stats.Damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_config == null) return;

        Gizmos.color = Color.red;
        Vector2 direction = _lastDirection;

        if (!Application.isPlaying) direction = Vector2.right;

        Vector2 attackPoint = (Vector2)transform.position + (direction * _config.attackOffset);
        Gizmos.DrawWireSphere(attackPoint, _config.attackRadius);
    }
}
