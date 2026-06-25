using TMPro.EditorUtilities;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
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

        if (moveX != 0 || moveY != 0)
        {
            _lastDirection = new Vector2(moveX, moveY).normalized;
            return;
        }

        _cooldownTimer += Time.deltaTime;

        if (_cooldownTimer >= _config.attackCooldown)
        {
            _anim.SetTrigger("Attack");
            _cooldownTimer = 0f;
        }
    }
    public void DealDamageEvent()
    {
        Vector2 attackPoint = (Vector2)transform.position + (_lastDirection * _config.attackOffset);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, _config.attackRadius, _enemyLayer);
        Debug.Log("Attack");

        //foreach (Collider2D enemyCollider in hitEnemies)
        //{
        //    if (enemyCollider.TryGetComponent<Enemy>(out var enemy))
        //    {
        //        enemy.TakeDamage(_stats.Damage);
        //    }
        //}
    }

    private void OnDrawGizmosSelected()
    {
        if (_config == null) return;

        Gizmos.color = Color.red;
        Vector2 direction = _lastDirection;

        // Если игра не запущена, рисуем зону просто справа от персонажа
        if (!Application.isPlaying) direction = Vector2.right;

        Vector2 attackPoint = (Vector2)transform.position + (direction * _config.attackOffset);
        Gizmos.DrawWireSphere(attackPoint, _config.attackRadius);
    }
}
