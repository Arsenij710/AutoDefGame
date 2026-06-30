using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Visuals")]
    public Sprite EnemySprite;
    public Vector2 SpriteSize = new Vector2(3f, 3f);

    public RuntimeAnimatorController Animator;

    [Header("Movement Settings")]
    public float Speed = 4f;
    public float StoppingDistance = 1.2f;

    [Header("Stats")]
    public float MaxHealth = 100f;
    public int ScoreReward = 20;

    [Header("Attack Settings")]
    public int Damage = 10;
    public float AttackCooldown = 3f;
    public float AttackRadius = 1.5f;
    public LayerMask PlayerLayer;

    [Header("Collider Settings")]
    public Vector2 colliderSize = new Vector2(1f, 0.7f);     
}
