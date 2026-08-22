using UnityEngine;

[CreateAssetMenu(fileName = "BerserkSet", menuName = "Artifacts/Sets/BerserkSet")]
public class BerserkSet : ArtifactSet
{
    [Header("Balance Settings")]
    [SerializeField] private float _damageBonus = 15f;
    [SerializeField] private float _attackSpeedBonus = 0.25f;

    public override void Apply2PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            //stats.ModifyDamage(_damageBonus); 
            Debug.Log("Берсерк (2): Урон увеличен!");
        }
    }

    public override void Apply4PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            //stats.ModifyAttackSpeed(_attackSpeedBonus);
            Debug.Log("Берсерк (4): Скорость атаки бешеная!");
        }
    }
    public override void Apply6PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            //stats.ModifyAttackSpeed(_attackSpeedBonus);
            Debug.Log("Берсерк (6): Скорость атаки бешеная!");
        }
    }

    public override void Remove2PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        //if (stats != null) stats.ModifyDamage(-_damageBonus);
    }

    public override void Remove4PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        //if (stats != null) stats.ModifyAttackSpeed(-_attackSpeedBonus);
    }
    public override void Remove6PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        //if (stats != null) stats.ModifyAttackSpeed(-_attackSpeedBonus);
    }
}
