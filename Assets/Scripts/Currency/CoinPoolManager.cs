using UnityEngine;
using UnityEngine.Pool;

public class CoinPoolManager : MonoBehaviour
{
    public static CoinPoolManager Instance { get; private set; }

    [Header("Pool Configurations")]
    [SerializeField] private Coin _coinPrefab;
    [SerializeField] private int _defaultCapacity = 30;
    [SerializeField] private int _maxPoolSize = 150;

    private ObjectPool<Coin> _pool;
    private Transform _playerTransform;
    private PlayerStats _stats;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _pool = new ObjectPool<Coin>(
            CreateCoin, OnGetCoin, OnReleaseCoin, OnDestroyCoin,
            false, _defaultCapacity, _maxPoolSize
        );
    }
    private void Start()
    {
        _stats = FindFirstObjectByType<PlayerStats>();
        if (_stats != null) _playerTransform = _stats.transform;
    }

    public void SpawnCoin(Vector2 spawnPosition, CoinTypeData typeData)
    {
        if (_playerTransform == null || typeData == null) return;

        int currWave = EnemySpawner.Instance.GetCurrentWave;
        float waveMultiplier = 1f + ((currWave - 1) * 0.1f);
        float finalValue = typeData.baseValue * waveMultiplier;
        finalValue *= _stats.GoldMultiplier;

        int calculatedValue = Mathf.RoundToInt(finalValue);
        Coin coin = _pool.Get();
        coin.transform.position = spawnPosition;
        coin.Init(_playerTransform, _pool, typeData, calculatedValue);
    }
    private Coin CreateCoin() => Instantiate(_coinPrefab, transform);
    private void OnGetCoin(Coin coin) => coin.gameObject.SetActive(true);
    private void OnReleaseCoin(Coin coin) => coin.gameObject.SetActive(false);
    private void OnDestroyCoin(Coin coin) => Destroy(coin.gameObject);
}
