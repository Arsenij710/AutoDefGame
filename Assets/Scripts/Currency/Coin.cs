using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Coin : MonoBehaviour
{
    private Transform _playerTransform;
    private IObjectPool<Coin> _pool;
    private SpriteRenderer _spriteRenderer;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _acceleration = 3f;

    [Header("Animation")]
    [SerializeField] private float _scatterDuration = 0.5f;
    [SerializeField] private float _minHorizontalForce = 2f; 
    [SerializeField] private float _maxHorizontalForce = 5f;
    [SerializeField] private float _arcHeight = 1.5f;        
    [SerializeField] private float _idleDelay = 0.2f;

    private float _currentSpeed;
    private int _finalCoinValue;
    private bool _isFlying = false;
    private bool _isScattering = false;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Init(Transform player, IObjectPool<Coin> pool, CoinTypeData config, int calculatedValue)
    {
        _playerTransform = player;
        _pool = pool;
        _currentSpeed = _moveSpeed;

        _spriteRenderer.sprite = config.coinSprite;
        _spriteRenderer.color = config.color;
        _finalCoinValue = calculatedValue;

        _isFlying = false;
        StartCoroutine(ScatterAndHoldRoutine());
    }
    private IEnumerator ScatterAndHoldRoutine()
    {
        _isScattering = true;

        Vector2 startPos = transform.position;

        float chooseSide = Random.Range(0, 2) == 0 ? -1f : 1f;
        float horizontalForce = Random.Range(_minHorizontalForce, _maxHorizontalForce) * chooseSide;

        Vector2 targetLandingPos = startPos + new Vector2(horizontalForce, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < _scatterDuration)
        {
            elapsedTime += Time.deltaTime;
            float linearProgress = elapsedTime / _scatterDuration;

            float currentX = Mathf.Lerp(startPos.x, targetLandingPos.x, linearProgress);

            float arcY = Mathf.Sin(linearProgress * Mathf.PI) * _arcHeight;
            float currentY = Mathf.Lerp(startPos.y, targetLandingPos.y, linearProgress) + arcY;

            transform.position = new Vector3(currentX, currentY, transform.position.z);
            yield return null;
        }

        _isScattering = false;

        yield return new WaitForSeconds(_idleDelay);

        _isFlying = true;
    }
    private void Update()
    {
        if (_isScattering || !_isFlying || _playerTransform == null) return;

        _currentSpeed += _acceleration * Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);
        if (distanceToPlayer < 0.3f)
        {
            CollectCoin();
            return;
        }
        transform.position = Vector2.MoveTowards(
            transform.position,
            _playerTransform.position,
            _currentSpeed * Time.deltaTime
        );
    }
    private void CollectCoin()
    {
        _isFlying = false;

        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.AddGold(_finalCoinValue);
        }

        StopAllCoroutines();
        _pool.Release(this);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isFlying && collision.CompareTag("Player"))
        {
            CollectCoin();
        }
    }
}
