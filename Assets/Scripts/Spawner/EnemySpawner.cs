using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    public enum WaveDifficulty { Easy, Medium, Huge }

    [Header("Pool Setup")]
    [SerializeField] private EnemyController _universalPrefab;
    [SerializeField] private int _defaultCapacity = 50;
    [SerializeField] private int _maxPoolSize = 100;

    [Header("Wave Setup")]
    [SerializeField] private List<WaveData> _easyWaves; 
    [SerializeField] private List<WaveData> _midWaves; 
    [SerializeField] private List<WaveData> _hugeWaves; 
    [SerializeField] private Transform _playerTransform; 
    [SerializeField] private float _spawnRadius = 15f;
    [SerializeField] private float _timeBetweenWaves = 3f;
    
    [Header("Levels Sequence")]
    [SerializeField] private List<WaveDifficulty> waveStructure = new List<WaveDifficulty>()
    {
        WaveDifficulty.Easy,
        WaveDifficulty.Easy,  
        WaveDifficulty.Easy,  
        WaveDifficulty.Medium,
        WaveDifficulty.Easy,
        WaveDifficulty.Medium,
        WaveDifficulty.Easy,
        WaveDifficulty.Medium,
        WaveDifficulty.Huge
    };

    [Header("Map Bounds")]
    [SerializeField] private EdgeCollider2D _mapEdgeCollider; 
    [SerializeField] private int _maxSpawnAttempts = 15;

    [Header("Update Panel")]
    [SerializeField] private UpgradeManager _upgrade;


    private Vector2[] _polygonPoints;

    private IObjectPool<EnemyController> _enemyPool;
    private int _currentWaveIndex = 0;
    private bool _isSpawning = false;
    private int _activeEnemiesCount = 0;
    private float _waveTimer = 0f;

    private void Awake()
    {
        _enemyPool = new ObjectPool<EnemyController>(
            createFunc: () => Instantiate(_universalPrefab, gameObject.transform),
            actionOnGet: (enemy) => { },
            actionOnRelease: (enemy) => enemy.gameObject.SetActive(false),
            actionOnDestroy: (enemy) => Destroy(enemy.gameObject),
            collectionCheck: false,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxPoolSize
        );

        if (_mapEdgeCollider != null)
        {
            _polygonPoints = _mapEdgeCollider.points;
        }
    }
    private void Start()
    {
        if (waveStructure.Count > 0  && _playerTransform != null)
        {
            StartCoroutine(StartNextWave());
        }
    }
    private IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(_timeBetweenWaves);
        while (_currentWaveIndex < waveStructure.Count)
        {
            WaveDifficulty requiredDifficulty = waveStructure[_currentWaveIndex];
            WaveData currentWave = GetRandomWaveFromPool(requiredDifficulty);

            _isSpawning = true;
            StartCoroutine(SpawnWaveEnemies(currentWave));

            while (_activeEnemiesCount > 0 && _waveTimer < currentWave.Duration)
            {
                _waveTimer += Time.deltaTime;
                yield return null; 
            }
            _isSpawning = false;

            float targetTime = currentWave.Duration * 0.8f;

            if (_activeEnemiesCount == 0 && _waveTimer <= targetTime)
            {
                _upgrade.OpenUpgradePanel();
            }

            _currentWaveIndex++;
            _waveTimer = 0;
            

            yield return new WaitForSeconds(_timeBetweenWaves);
        }
    }
    private WaveData GetRandomWaveFromPool(WaveDifficulty difficulty)
    {
        switch (difficulty)
        {
            case WaveDifficulty.Easy:
                return _easyWaves[Random.Range(0, _easyWaves.Count)];
            case WaveDifficulty.Medium:
                return _midWaves[Random.Range(0, _midWaves.Count)];
            case WaveDifficulty.Huge:
                return _hugeWaves[Random.Range(0, _hugeWaves.Count)];
            default:
                return _easyWaves[0];
        }
    }
    private IEnumerator SpawnWaveEnemies(WaveData wave)
    {
        List<EnemyData> spawnList = new List<EnemyData>();
        foreach (var config in wave.EnemiesInWave)
        {
            for (int i = 0; i < config.Count; i++)
            {
                spawnList.Add(config.EnemyConfig);
            }
        }

        ShuffleList(spawnList);

        int spawnedCount = 0;

        while (_isSpawning && spawnedCount < spawnList.Count)
        {
            EnemyData currentEnemyData = spawnList[spawnedCount];
            Vector3 spawnPosition = GetRandomSpawnPosition();

            EnemyController enemy = _enemyPool.Get();
            enemy.transform.position = spawnPosition;

            enemy.Initialize(currentEnemyData, (e) => _enemyPool.Release(e), _currentWaveIndex + 1);

            spawnedCount++;
            _activeEnemiesCount++;

            yield return new WaitForSeconds(wave.SpawnInterval);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 finalPosition = Vector3.zero;
        bool validPositionFound = false;
        int attempts = 0;

        while (!validPositionFound && attempts < _maxSpawnAttempts)
        {
            attempts++;
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 candidatePosition = _playerTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0f) * _spawnRadius;

            Vector2 localCandidate = _mapEdgeCollider.transform.InverseTransformPoint(candidatePosition);

            if (IsPointInPolygon(_polygonPoints, localCandidate))
            {
                finalPosition = candidatePosition;
                validPositionFound = true;
                break;
            }
        }

        if (!validPositionFound)
        {
            finalPosition = _playerTransform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        }

        return finalPosition;
    }

    private bool IsPointInPolygon(Vector2[] polyPoints, Vector2 point)
    {
        bool isInside = false;
        int j = polyPoints.Length - 1;

        for (int i = 0; i < polyPoints.Length; i++)
        {
            if (((polyPoints[i].y > point.y) != (polyPoints[j].y > point.y)) &&
                (point.x < (polyPoints[j].x - polyPoints[i].x) * (point.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x))
            {
                isInside = !isInside;
            }
            j = i;
        }

        return isInside;
    }
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            T value = list[rnd];
            list[rnd] = list[i];
            list[i] = value;
        }
    }
    public void OnEnemyKilled()
    {
        _activeEnemiesCount--;
    }
}
