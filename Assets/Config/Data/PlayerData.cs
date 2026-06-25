using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int baseMaxHealth = 100;
    public int baseDamage = 30;
    public float baseMoveSpeed = 5f;

    public const int HealthBonusPerLevel = 20;
    public const int DamageBonusPerLevel = 3;

    public float attackCooldown = 2f; 
    public float attackRadius = 1.3f;
    public float attackOffset = 0.5f;
}
