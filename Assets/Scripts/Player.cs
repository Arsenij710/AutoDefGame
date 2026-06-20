using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _direction;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        var dir_x = Input.GetAxisRaw("Horizontal");
        var dir_y = Input.GetAxisRaw("Vertical");
        _direction = new Vector2(dir_x, dir_y).normalized;

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
