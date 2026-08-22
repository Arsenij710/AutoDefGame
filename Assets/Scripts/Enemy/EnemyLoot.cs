using UnityEditor.ShaderGraph;
using UnityEngine;
using static DroppedItem;

public class EnemyLoot : MonoBehaviour
{
    private LootTable _currentLootTable;
    private RarityConfig _rarityConfig;

    public void InitializeLoot(LootTable table, RarityConfig rarityConfig)
    {
        _currentLootTable = table;
        _rarityConfig = rarityConfig;
    }

    public void TryDropLoot(float luckChance)
    {
        if (_currentLootTable == null) return;

        LootDrop dropData = _currentLootTable.GetRandomDrop(luckChance);
        if (dropData != null && dropData.itemPrefab != null)
        {
            GameObject droppedItem = LootSpawner.Instance.GetPooledLoot(dropData.itemPrefab);

            if (droppedItem != null)
            {
                if (droppedItem.TryGetComponent<DroppedItem>(out var item))
                {
                    if (item.lootType == LootType.Artifact && dropData.artifactSet != null)
                    {
                        var set = dropData.artifactSet;
                        if (set.setArtifacts.Count > 0)
                        {
                            ArtifactData randomPiece = set.setArtifacts[Random.Range(0, set.setArtifacts.Count)];
                            ArtifactRarity rarity = _rarityConfig.DetermineRarity(EnemySpawner.Instance.GetCurrentWave);

                            RuntimeArtifact runtimeArtifact = new RuntimeArtifact(randomPiece, set, rarity);
                            item.SetupArtifact(runtimeArtifact);
                        }
                    }
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
