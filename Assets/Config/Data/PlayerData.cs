using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int baseMaxHealth = 100;
    public int baseDamage = 30;
    public float baseMoveSpeed = 5f;
    public float baseHPRegen = 0.03f; // %
    public float baseCritChance = 0.05f; // %
    public float baseCritDamage = 1.50f; // %
    public float baseMiss = 3f; // %
    public float baseReAttack = 2f; // %

    public const float HealthBonusPerLevel = 0.2f; // %
    public const float DamageBonusPerLevel = 0.12f; // %
    public const float AttackSpeedBonusPerLevel = 0.1f;
    public const float RadiusBonusPerLevel = 0.1f;
    public const float OffsetBonusPerLevel = 0.0625f;
    public const float HPRegenBonusPerLevel = 0.005f; // %
    public const float CritChanceBonusPerLevel = 0.025f; // %
    public const float CritDamageBonusPerLevel = 0.10f; // %
    public const float MissBonusPerLevel = 1f; // %
    public const float ReAttackBonusPerLevel = 1f; // %

    public float attackCooldown = 1.5f; 
    public float attackRadius = 1.3f;
    public float attackOffset = 0.5f;
}
