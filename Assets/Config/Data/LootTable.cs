using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootDrop
{
    public GameObject itemPrefab; 
    [Range(0, 100)] public float weight; 
}

[CreateAssetMenu(fileName = "NewLootTable", menuName = "Loot/LootTable")]
public class LootTable : ScriptableObject
{
    [Range(0, 100)] public float generalDropChance = 5f; 
    public List<LootDrop> possibleDrops;

    public GameObject GetRandomDrop(float luckChance)
    {
        float randomRoll = Random.Range(0f, 100f);
        float finalChance = generalDropChance + luckChance;
        if (randomRoll > finalChance) return null;

        float totalWeight = 0;
        foreach (var drop in possibleDrops) totalWeight += drop.weight;

        float roll = Random.Range(0f, totalWeight);
        float s = 0;

        foreach (var drop in possibleDrops)
        {
            s += drop.weight;
            if (roll <= s) return drop.itemPrefab;
        }

        return null;
    }
}
