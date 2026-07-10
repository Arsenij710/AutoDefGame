using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LootSpawner : MonoBehaviour
{
    public static LootSpawner Instance { get; private set; }

    private Dictionary<string, ObjectPool<GameObject>> _pools = new Dictionary<string, ObjectPool<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void RegisterNewPool(string key, GameObject prefab)
    {
        _pools[key] = new ObjectPool<GameObject>(
            createFunc: () => {
                GameObject obj = Instantiate(prefab, transform);
                obj.name = prefab.name;
                return obj;
            },
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 5,
            maxSize: 20
        );
    }
    public GameObject GetPooledLoot(GameObject prefab)
    {
        string key = prefab.name; 

        if (!_pools.ContainsKey(key))
        {
            RegisterNewPool(key, prefab);
        }

        return _pools[key].Get();
    }

    public void ReturnLootToPool(GameObject prefab, GameObject instance)
    {
        string key = prefab.name;

        if (_pools.ContainsKey(key))
        {
            _pools[key].Release(instance);
        }
        else
        {
            Destroy(instance);
        }
    }
}
