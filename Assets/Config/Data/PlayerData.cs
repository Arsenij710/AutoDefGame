using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string ClassName;

    public int baseMaxHealth = 100;
    public int baseDamage = 30;
    public int baseDefence = 50;
    public int AttackSpeed = 200;
    public float baseMoveSpeed = 5f;
    public float baseHPRegen = 0.3f; // %
    public float baseCritChance = 5f; // %
    public float baseCritDamage = 150f; // %
    public float baseMiss = 3f; // %
    public float baseReAttack = 2f; // %
    public float baseLuck = 1f; // %
    public float baseVampirism = 0f; // %
    public float attackRadius = 1.2f;
    public float attackOffset = 0.5f;

    public const float HealthBonusPerLevel = 0.2f; // *
    public const float DamageBonusPerLevel = 0.12f; // *
    public const float DefenceBonusPerLevel = 0.15f; // *
    public const float AttackSpeedBonusPerLevel = 25f;
    public const float RadiusBonusPerLevel = 0.2f;
    public const float OffsetBonusPerLevel = 0.125f;
    public const float HPRegenBonusPerLevel = 0.5f; // %
    public const float CritChanceBonusPerLevel = 2.5f; // %
    public const float CritDamageBonusPerLevel = 10f; // %
    public const float MissBonusPerLevel = 1f; // %
    public const float ReAttackBonusPerLevel = 1f; // %
    public const float LuckGoldBonusPerLevel = 0.5f; // %
    public const float LuckLootBonusPerLevel = 0.5f; // %
    public const float VampirismBonusPerLevel = 1f; // %

}
