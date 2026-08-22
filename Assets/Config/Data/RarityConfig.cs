using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RarityChance
{
    public ArtifactRarity Rarity;
    [Range(0f, 1f)] public float Chance;
}

[System.Serializable]
public struct WaveRarityRule
{
    public int MinWave;                    
    public List<RarityChance> Chances;
}
[CreateAssetMenu(fileName = "NewRarityConfig", menuName = "Artifacts/RarityConfig")]
public class RarityConfig : ScriptableObject
{
    [SerializeField] private List<WaveRarityRule> _waveRules;
    public ArtifactRarity DetermineRarity(int currentWave)
    {
        WaveRarityRule activeRule = default;
        bool ruleFound = false;

        for (int i = _waveRules.Count - 1; i >= 0; i--)
        {
            if (currentWave >= _waveRules[i].MinWave)
            {
                activeRule = _waveRules[i];
                ruleFound = true;
                break;
            }
        }
        if (!ruleFound && _waveRules.Count > 0)
            activeRule = _waveRules[0];

        float roll = Random.value;
        float cumulative = 0f;

        foreach (var rarityChance in activeRule.Chances)
        {
            cumulative += rarityChance.Chance;
            if (roll <= cumulative)
            {
                return rarityChance.Rarity;
            }
        }

        return ArtifactRarity.Common;
    }
}
