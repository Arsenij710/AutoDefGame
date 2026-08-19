using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    private LootTable _currentLootTable;

    public void InitializeLoot(LootTable table)
    {
        _currentLootTable = table;
    }

    public void TryDropLoot(float luckChance)
    {
        if (_currentLootTable == null) return;

        GameObject prefabToSpawn = _currentLootTable.GetRandomDrop(luckChance);

        if (prefabToSpawn != null)
        {
            GameObject droppedItem = LootSpawner.Instance.GetPooledLoot(prefabToSpawn);

            if (droppedItem != null)
            {
                if (droppedItem.TryGetComponent<DroppedItem>(out var item))
                {
                    item.Drop(transform.position);
                }
                else
                {
                    droppedItem.transform.position = transform.position;
                }
            }
        }
    }
}
