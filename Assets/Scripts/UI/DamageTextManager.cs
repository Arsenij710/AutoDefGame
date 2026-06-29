using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class DamageTextManager : MonoBehaviour
{
    [Header("Damage Numbers UI")]
    [SerializeField] private DamageDisappear _damagePrefab; 

    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxPoolSize = 50;

    private ObjectPool<DamageDisappear> _damagePool;
    private void Awake()
    {
        _damagePool = new ObjectPool<DamageDisappear>(
           createFunc: () => Instantiate(_damagePrefab, transform),
           actionOnGet: (dmg) => dmg.gameObject.SetActive(true),
           actionOnRelease: (dmg) => dmg.gameObject.SetActive(false),
           actionOnDestroy: (dmg) => Destroy(dmg.gameObject),
           collectionCheck: true,
           defaultCapacity: _defaultCapacity,
           maxSize: _maxPoolSize
       );
    }
    public void ShowDamage(Vector3 position, int damageAmount)
    {
        int isDamageVisible = PlayerPrefs.GetInt("ShowDamageNumbers", 1);
        if (_damagePrefab != null && isDamageVisible == 1)
        {
            Vector3 spawnPos = position + Vector3.up * 0.5f;
            DamageDisappear damagePopup = _damagePool.Get();
            damagePopup.transform.position = spawnPos;
            damagePopup.Setup(damageAmount, _damagePool);
        }
    }
}
