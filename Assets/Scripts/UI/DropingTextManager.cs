using UnityEngine;
using UnityEngine.Pool;

public class DropingTextManager : MonoBehaviour
{
    [Header("Damage Numbers UI")]
    [SerializeField] private DropingTextDisappear _damagePrefab; 

    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxPoolSize = 50;

    public static DropingTextManager Instance { get; private set; }
    private ObjectPool<DropingTextDisappear> _damagePool;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _damagePool = new ObjectPool<DropingTextDisappear>(
           createFunc: () => Instantiate(_damagePrefab, transform),
           actionOnGet: (dmg) => dmg.gameObject.SetActive(true),
           actionOnRelease: (dmg) => dmg.gameObject.SetActive(false),
           actionOnDestroy: (dmg) => Destroy(dmg.gameObject),
           collectionCheck: true,
           defaultCapacity: _defaultCapacity,
           maxSize: _maxPoolSize
       );
    }
    public void ShowDropingText(Vector3 position, int amount, Color color, bool isCrit=false, bool isMiss=false)
    {
        int isTextVisible = PlayerPrefs.GetInt("ShowDropingNumbers", 1);
        if (_damagePrefab != null && isTextVisible == 1)
        {
            Vector3 spawnPos = position + Vector3.up * 0.5f;
            DropingTextDisappear dropingTextPopup = _damagePool.Get();
            dropingTextPopup.transform.position = spawnPos;
            dropingTextPopup.Setup(amount, _damagePool, color, isCrit, isMiss);
        }
    }
}
