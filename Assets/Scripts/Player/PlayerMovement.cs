using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerData _config;

    private Animator _anim;
    private Rigidbody2D _rb;
    private Vector2 _direction;
    private float _speed;
    private bool _isFacingRight = true;

    void Start()
    {
        _speed = _config.baseMoveSpeed;

        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
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
        _rb.linearVelocity = _direction * _speed;
    }
}
