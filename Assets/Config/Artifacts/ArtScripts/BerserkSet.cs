using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BerserkSet", menuName = "Artifacts/Sets/BerserkSet")]
public class BerserkSet : ArtifactSet
{
    [Header("Balance Settings")]
    [SerializeField] private float _damageBonus = 0.2f;
    [SerializeField] private float _attackSpeedBonus = 300f;
    [SerializeField] private float _critChanceBonus = 0.15f;
    [SerializeField] private float _6piecesDamageBonus = 0.75f;
    [SerializeField] private float _critDamageBonus = 50f;
    
    private Coroutine _activeBuffCoroutine;

    public override void Apply2PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.ModifyDamage(_damageBonus); 
        }
    }

    public override void Apply4PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.ModifyAttackSpeed(_attackSpeedBonus);
            stats.ModifyCritChance(_critChanceBonus);
        }
    }
    public override void Apply6PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.OnHealthChangedEvent += () => OnPlayerTakeDamage(stats);
        }
    }
    private void OnPlayerTakeDamage(PlayerStats stats)
    {
        if (_activeBuffCoroutine != null)
        {
            stats.StopCoroutine(_activeBuffCoroutine);
        }
        else
        {
            stats.ModifyDamage(_6piecesDamageBonus);
            stats.ModifyCritDamage(_critDamageBonus);
        }

        _activeBuffCoroutine = stats.StartCoroutine(BuffTimerRoutine(stats));
    }
    private IEnumerator BuffTimerRoutine(PlayerStats stats)
    {
        yield return new WaitForSeconds(8.0f);

        stats.ModifyDamage(-_6piecesDamageBonus);
        stats.ModifyCritDamage(-_critDamageBonus);
        _activeBuffCoroutine = null;
    }
    public override void Remove2PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null) stats.ModifyDamage(-_damageBonus);
    }

    public override void Remove4PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.ModifyAttackSpeed(-_attackSpeedBonus);
            stats.ModifyCritChance(-_critChanceBonus);

        }
    }
    public override void Remove6PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.OnHealthChangedEvent -= () => OnPlayerTakeDamage(stats);

            if (_activeBuffCoroutine != null)
            {
                stats.StopCoroutine(_activeBuffCoroutine);
                _activeBuffCoroutine = null;
                stats.ModifyDamage(-_6piecesDamageBonus);
                stats.ModifyCritDamage(-_critDamageBonus);
            }
        }
    }
}
