using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDuration = 0.7f;
    [SerializeField] private float _dashCooldown = 1f;

    [SerializeField] private PlayerData _config;

    private Animator _anim;
    private Rigidbody2D _rb;
    private PlayerStats _stats;
    private Vector2 _direction;
    private Vector2 _dashDirection;
    private bool _isDashing = false;
    private bool _canDash = true;
    private bool _isFacingRight = true;

    void Start()
    {
        _stats = GetComponent<PlayerStats>();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (_isDashing) return;
        var dir_x = Input.GetAxisRaw("Horizontal");
        var dir_y = Input.GetAxisRaw("Vertical");
        _direction = new Vector2(dir_x, dir_y).normalized;

        bool isMoving = _direction.magnitude > 0;
        _anim.SetBool("isRun", isMoving);

        if (dir_x > 0 && !_isFacingRight)
        {
            Flip();
        }
        else if (dir_x < 0 && _isFacingRight)
        {
            Flip();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
        {
            StartCoroutine(DashRoutine());
        }
    }
    private IEnumerator DashRoutine()
    {
        _canDash = false;
        _isDashing = true;

        _dashDirection = _direction.normalized;
        if (_dashDirection == Vector2.zero)
        {
            _dashDirection = new Vector2(transform.localScale.x, 0f).normalized;
        }

        if (_stats != null) _stats.isInvincible = true;
        if (_anim != null)
        {
            _anim.SetTrigger("Dash");
        }
        _rb.linearVelocity = _dashDirection * _dashSpeed;

        yield return new WaitForSeconds(_dashDuration);

        if (_stats != null) _stats.isInvincible= false;
        _isDashing = false;

        yield return new WaitForSeconds(_dashCooldown);

        _canDash = true;
    }
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;

        Vector3 curScale = transform.localScale;
        curScale.x *= -1;
        transform.localScale = curScale;
    }
    private void FixedUpdate()
    {
        if (_isDashing) return;
        _rb.linearVelocity = _direction * _stats.Speed;
    }
}
