using Unity.VisualScripting;
using UnityEngine;

public static class StatUtils
{
    static Color CommonColor = new Color32(160, 160, 160, 255);
    static Color UncommonColor = new Color32(50, 190, 35, 255);
    static Color RareColor = new Color32(35, 90, 190, 255);
    static Color EpicColor = new Color32(120, 35, 190, 255);
    static Color LegendaryColor = new Color32(240, 160, 60, 255);
    static Color MythicalColor = new Color32(190, 35, 35, 255);
    public static string GetStatName(StatType StatType)
    {
        return StatType switch
        {
            StatType.FlatHP or
            StatType.PercentHP => "HP",
            StatType.FlatAttack or
            StatType.PercentAttack => "Сила атаки",
            StatType.FlatDefence or
            StatType.PercentDefence => "Защита",
            StatType.CritChance => "Крит. шанс",
            StatType.CritDamage => "Крит. урон",
            StatType.MissChance => "Шанс уворота",
            StatType.HpRegen => "Регенерация HP",
            _ => ""
        };
    }
    public static string GetElementName(ArtifactSlotType StatType)
    {
        return StatType switch
        {
            ArtifactSlotType.Helmet => "Шлем",
            ArtifactSlotType.Armor => "Броня",
            ArtifactSlotType.Gauntlets => "Перчатки",
            ArtifactSlotType.Greaves => "Ботинки",
            ArtifactSlotType.Weapon=> "Оружие",
            ArtifactSlotType.Ring => "Кольцо",
            _ => ""
        };
    }
    public static Color GetRarityColor(ArtifactRarity rarity) => rarity switch
    {
        ArtifactRarity.Common => CommonColor,
        ArtifactRarity.Uncommon => UncommonColor,
        ArtifactRarity.Rare => RareColor,
        ArtifactRarity.Epic => EpicColor,
        ArtifactRarity.Legendary => LegendaryColor,
        ArtifactRarity.Mythical => MythicalColor,
        _ => Color.white
    };
    public static string FormatStatForUI(ArtifactStat stat)
    {
        string formattedValue = stat.value.ToString("0.#");
        string percent = stat.IsPercent ? "%" : "";

        return $"{formattedValue} {percent}";
    }

}
