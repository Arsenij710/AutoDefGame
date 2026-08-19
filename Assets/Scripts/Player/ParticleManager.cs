using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    [Header("Exp Settings")]
    [SerializeField] private float _magnetRadius = 4f; 
    [SerializeField] private float _flySpeed = 10f;
    [SerializeField] private int _expPerParticle = 20;

    [Header("color Exp")]
    [SerializeField] private Color _lowExpColor = new Color32(115, 255, 115, 255);
    [SerializeField] private Color _mediumExpColor = new Color32(50, 180, 255, 255);
    [SerializeField] private Color _highExpColor = new Color32(200, 80, 255, 255);
    [SerializeField] private Color _epicExpColor = new Color32(255, 215, 0, 255);

    public static ParticleManager Instance { get; private set; }

    private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles;
    private Transform _playerTransform;
    private PlayerStats _playerStats;
    private int _currExp;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _particleSystem = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
    }
    void Start()
    {
        var player = FindFirstObjectByType<PlayerStats>();
        if (player != null)
        {
            _playerTransform = player.transform;
            _playerStats = player;
        }
    }
    public void SpawnExperience(Vector3 position, int totalExp)
    {
        _currExp = totalExp;

        Vector3 spawnPosition = new Vector3(position.x, position.y, 0f);
        Color finalColor = GetColorForExp();

        var emitParams = new ParticleSystem.EmitParams();
        emitParams.position = spawnPosition;
        emitParams.randomSeed = (uint)totalExp;
        emitParams.startColor = finalColor;

        _particleSystem.Emit(emitParams, 1);
    }

    void LateUpdate()
    {
        if (_playerTransform == null) return;

        int numParticlesAlive = _particleSystem.GetParticles(_particles);

        Vector3 playerPos = _playerTransform.position;

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 particleWorldPos = _particles[i].position;
            float distance = Vector3.Distance(particleWorldPos, playerPos);

            if (distance <= _magnetRadius)
            {
                particleWorldPos = Vector3.MoveTowards(particleWorldPos, playerPos, _flySpeed * Time.deltaTime);

                if (distance < 0.2f)
                {
                    uint expFromSeed = _particles[i].randomSeed;
                    int finalExp = expFromSeed > 0 ? (int)expFromSeed : _expPerParticle;

                    _particles[i].remainingLifetime = 0;

                    if (_playerStats != null)
                    {
                        _playerStats.AddExperience(finalExp);
                    }
                }
            }
            _particles[i].position = particleWorldPos;
        }
        _particleSystem.SetParticles(_particles, numParticlesAlive);
    }
    private Color GetColorForExp()
    {
        float mult = EnemySpawner.Instance.GetExpMult;
        if (_currExp < 25 * mult)
        {
            return _lowExpColor;
        }
        else if (_currExp <= 45 * mult)
        {
            return _mediumExpColor;
        }
        else if (_currExp < 100 * mult)
        {
            return _highExpColor;
        }
        else
        {
            return _epicExpColor;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (_playerTransform != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);

            Gizmos.DrawSphere(_playerTransform.position, _magnetRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_playerTransform.position, _magnetRadius);
        }
    }
}
