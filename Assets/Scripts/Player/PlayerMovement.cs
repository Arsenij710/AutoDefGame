using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerData _config;

    private float _speed;
    private Animator _anim;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _direction;

    void Start()
    {
        _speed = _config.baseMoveSpeed;

        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        var dir_x = Input.GetAxisRaw("Horizontal");
        var dir_y = Input.GetAxisRaw("Vertical");
        _direction = new Vector2(dir_x, dir_y).normalized;

        bool isMoving = _direction.magnitude > 0;

        _anim.SetBool("isRun", isMoving);

        if (dir_x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (dir_x < 0)
        {
            _spriteRenderer.flipX = true;

        }
    }
    private void FixedUpdate()
    {
        _rb.linearVelocity = _direction * _speed;
    }
}
