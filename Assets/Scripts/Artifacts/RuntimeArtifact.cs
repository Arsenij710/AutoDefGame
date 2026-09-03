using System.Collections.Generic;
using UnityEngine;
public enum StatType
{
    FlatHP,
    PercentHP,
    FlatAttack,
    PercentAttack,
    FlatDefence,
    PercentDefence,
    CritChance,
    CritDamage,
    MissChance,
    HpRegen,
    DoubleAttack,
    Vampirism
}

[System.Serializable]
public class ArtifactStat
{
    public StatType type;
    public float baseValue;
    public bool IsPercent;
    public bool isEmpowered;
    public float value => isEmpowered ? baseValue * 1.5f : baseValue;

    public ArtifactStat(StatType type, float value)
    {
        this.type = type;
        baseValue = value;
        IsPercent = type switch
        {
            StatType.PercentAttack or
            StatType.PercentHP or
            StatType.PercentDefence or
            StatType.CritChance or
            StatType.CritDamage or
            StatType.MissChance or
            StatType.HpRegen or
            StatType.DoubleAttack or
            StatType.Vampirism => true,
            _ => false
        };
        isEmpowered = false;
    }
}
public class RuntimeArtifact
{
    public ArtifactData data;
    public ArtifactSet artifactSet;
    public ArtifactRarity rarity;
    public int MaxSubStatsCount;

    [Header("Upgrades")]
    public int level = 0;
    public int maxLevel = 120;

    [Header("Characteristics")]
    public ArtifactStat mainStat;
    public List<ArtifactStat> subStats = new List<ArtifactStat>();
    private float mainStatGrowth;
    private float mainStatBaseValue;
    public RuntimeArtifact(ArtifactData data, ArtifactSet set, ArtifactRarity rarity)
    {
        this.data = data;
        this.rarity = rarity;
        artifactSet = set;

        MaxSubStatsCount = StatUtils.GenerateSubstatsCount(rarity);
        GenerateMainStat();
        GenerateSubStats();
    }
    private void GenerateMainStat()
    {

        StatType mainType = data.slotType switch
        {
            ArtifactSlotType.Helmet => StatType.FlatDefence,
            ArtifactSlotType.Armor => StatType.FlatHP,
            ArtifactSlotType.Gauntlets => GetRandomMainStat(),
            ArtifactSlotType.Greaves => GetRandomMainStat(),
            ArtifactSlotType.Weapon => StatType.FlatAttack,
            ArtifactSlotType.Ring => GetRandomMainStat(),
            _ => StatType.FlatAttack
        };

        float baseValue = GetInitialMainStatValue(mainType, rarity);

        mainStat = new ArtifactStat(mainType, baseValue);
    }
    private float GetInitialMainStatValue(StatType type, ArtifactRarity rarity)
    {
        float rarityMult = rarity switch
        {
            ArtifactRarity.Mythical => 3f,
            ArtifactRarity.Legendary => 2f,
            ArtifactRarity.Epic => 1.4f,
            ArtifactRarity.Rare => 1f,
            ArtifactRarity.Uncommon => 0.7f,
            ArtifactRarity.Common => 0.4f,
            _ => 0.1f
        };
        switch (type)
        {
            case StatType.FlatHP:
                mainStatBaseValue = 500f * rarityMult;
                mainStatGrowth = 12f * rarityMult;
                break;

            case StatType.FlatAttack:
                mainStatBaseValue = 120f * rarityMult;
                mainStatGrowth = 1.5f * rarityMult;
                break;

            case StatType.FlatDefence:
                mainStatBaseValue = 85f * rarityMult;
                mainStatGrowth = 1.2f * rarityMult;
                break;

            case StatType.PercentHP:
                mainStatBaseValue = 14f * rarityMult;
                mainStatGrowth = 0.18f * rarityMult;
                break;

            case StatType.PercentDefence:
                mainStatBaseValue = 14f * rarityMult;
                mainStatGrowth = 0.18f * rarityMult;
                break;

            case StatType.PercentAttack:
                mainStatBaseValue = 15f * rarityMult;
                mainStatGrowth = 0.18f * rarityMult;
                break;

            case StatType.CritChance:
                mainStatBaseValue = 5f * rarityMult;
                mainStatGrowth = 0.08f * rarityMult;
                break;

            case StatType.CritDamage:
                mainStatBaseValue = 12f * rarityMult;
                mainStatGrowth = 0.16f * rarityMult;
                break;

            case StatType.MissChance:
                mainStatBaseValue = 3f * rarityMult;
                mainStatGrowth = 0.08f * rarityMult;
                break;

            case StatType.HpRegen:
                mainStatBaseValue = 2f * rarityMult;
                mainStatGrowth = 0.05f * rarityMult;
                break;
            
            case StatType.DoubleAttack:
                mainStatBaseValue = 2f * rarityMult;
                mainStatGrowth = 0.05f * rarityMult;
                break;

            case StatType.Vampirism:
                mainStatBaseValue = 15f * rarityMult;
                mainStatGrowth = 0.2f * rarityMult;
                break;

            default:
                mainStatBaseValue = 1f * rarityMult;
                mainStatGrowth = 0.1f * rarityMult;
                break;
        }
        return mainStatBaseValue;
    }
    private StatType GetRandomMainStat()
    {
        StatType[] validStats = new StatType[]
        {
        StatType.PercentHP,
        StatType.PercentAttack,
        StatType.PercentDefence,
        StatType.CritChance,
        StatType.CritDamage,
        StatType.MissChance,
        StatType.HpRegen,
        StatType.DoubleAttack,
        StatType.Vampirism
        };

        return validStats[Random.Range(0, validStats.Length)];
    }
    private void GenerateSubStats()
    {
        List<StatType> availableTypes = new List<StatType>((StatType[])System.Enum.GetValues(typeof(StatType)));
        availableTypes.Remove(mainStat.type);

        for (int i = 0; i < MaxSubStatsCount; i++)
        {
            int randomIndex = Random.Range(0, availableTypes.Count);
            StatType selectedType = availableTypes[randomIndex];
            availableTypes.RemoveAt(randomIndex);

            float randomValue = GetRandomSubStatValue(selectedType);
            subStats.Add(new ArtifactStat(selectedType, randomValue));
        }

        if (rarity == ArtifactRarity.Mythical && subStats.Count > 0)
        {
            int empoweredCount = Random.Range(1, 3);

            List<int> statIndices = new List<int>();
            for (int i = 0; i < subStats.Count; i++) statIndices.Add(i);

            for (int i = 0; i < empoweredCount; i++)
            {
                if (statIndices.Count == 0) break;

                int randIndexPos = Random.Range(0, statIndices.Count);
                int targetStatIndex = statIndices[randIndexPos];
                statIndices.RemoveAt(randIndexPos);

                subStats[targetStatIndex].isEmpowered = true;
            }
        }
    }
    private float GetRandomSubStatValue(StatType type)
    {
        float rarityMultiplier = rarity switch
        {
            ArtifactRarity.Common => 0.4f,
            ArtifactRarity.Uncommon => 0.7f,
            ArtifactRarity.Rare => 1f,
            ArtifactRarity.Epic => 1.4f,
            ArtifactRarity.Legendary => 2f,
            ArtifactRarity.Mythical => 3f,
            _ => 0
        };
        float baseValue = type switch
        {
            StatType.FlatAttack => Random.Range(35f, 65f),
            StatType.FlatHP => Random.Range(100f, 250f),
            StatType.FlatDefence => Random.Range(30f, 50f),
            StatType.PercentAttack => Random.Range(4.1f, 5.8f),
            StatType.PercentHP => Random.Range(4.1f, 5.8f),
            StatType.PercentDefence => Random.Range(4.1f, 5.8f),
            StatType.CritChance => Random.Range(1.5f, 2.4f),
            StatType.CritDamage => Random.Range(4.5f, 6.3f),
            StatType.MissChance => Random.Range(1f, 2.4f),
            StatType.HpRegen => Random.Range(1f, 1.9f),
            StatType.DoubleAttack => Random.Range(0.9f, 2f),
            StatType.Vampirism => Random.Range(4f, 6.5f),
            _ => 0
        };

        return baseValue * rarityMultiplier;
    }
    public void Upgrade()
    {
        if (level >= maxLevel) return;
        level++;
        mainStat.baseValue = CalculateMainStatValue(level);

        if (level % 10 == 0 && subStats.Count > 0)
        {
            int randomSubIndex = Random.Range(0, subStats.Count);
            ArtifactStat randomSub = subStats[randomSubIndex];

            float increase = GetRandomSubStatValue(randomSub.type);
            randomSub.baseValue += increase;
        }
    }
    private float CalculateMainStatValue(int currentLevel)
    {
        float levelCurve = Mathf.Pow(currentLevel, 1.8f);
        float finalValue = mainStatBaseValue + (mainStatGrowth * levelCurve);

        return finalValue;
    }
}
