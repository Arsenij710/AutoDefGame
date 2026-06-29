using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    [Header("Настройки притягивания")]
    [SerializeField] private float _magnetRadius = 4f; 
    [SerializeField] private float _flySpeed = 10f;
    [SerializeField] private int _expPerParticle = 20; 

    private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles;
    private Transform _playerTransform;
    private PlayerStats _playerStats;
    void Awake()
    {
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
    public void SpawnExperience(Vector3 position, int amountOfParticles)
    {
        Vector3 spawnPosition = new Vector3(position.x, position.y, 0f);
        var emitParams = new ParticleSystem.EmitParams
        {
            position = spawnPosition
        };

        _particleSystem.Emit(emitParams, amountOfParticles);

    }

    void Update()
    {
        if (_playerTransform == null) return;

        int numParticlesAlive = _particleSystem.GetParticles(_particles);

        Vector3 playerPos = _playerTransform.position;

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 particleWorldPos = _particles[i].position;
            particleWorldPos.z = 0f;

            float distance = Vector3.Distance(particleWorldPos, playerPos);

            if (distance <= _magnetRadius)
            {
                particleWorldPos = Vector3.MoveTowards(particleWorldPos, playerPos, _flySpeed * Time.deltaTime);

                if (distance < 0.2f)
                {
                    _particles[i].remainingLifetime = 0;

                    if (_playerStats != null)
                    {
                        _playerStats.AddExperience(_expPerParticle);
                    }
                }
                _particles[i].position = particleWorldPos;
            }
        }

        _particleSystem.SetParticles(_particles, numParticlesAlive);
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
