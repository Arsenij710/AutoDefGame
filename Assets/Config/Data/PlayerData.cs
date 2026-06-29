using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int baseMaxHealth = 100;
    public int baseDamage = 30;
    public float baseMoveSpeed = 5f;

    public const int HealthBonusPerLevel = 20;
    public const int DamageBonusPerLevel = 10;
    public const float AttackSpeedBonusPerLevel = 0.1f;
    public const float RadiusBonusPerLevel = 0.1f;
    public const float OffsetBonusPerLevel = 0.0625f;

    public float attackCooldown = 1.5f; 
    public float attackRadius = 1.3f;
    public float attackOffset = 0.5f;
}
