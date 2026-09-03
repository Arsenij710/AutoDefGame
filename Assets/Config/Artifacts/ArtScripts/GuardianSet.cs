using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "GuardianSet", menuName = "Artifacts/Sets/GuardianSet")]
public class GuardianSet : ArtifactSet
{
    [Header("Balance Settings")]
    [SerializeField] private float _hpBonus = 0.2f;
    [SerializeField] private float _4piecesBonus = 0.6f;
    [SerializeField] private float _damageBonus = 0.25f;
    [SerializeField] private float _heal = 0.3f;
    [SerializeField] private float _cooldownTime = 60f;

    private bool _isAttackApplied;
    private bool _isDefenseApplied;
    private float _currentAddedAttackBonus = 0f;
    private float _lastProcTime = -999f;
    public override void Apply2PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.ModifyHp(_hpBonus);
        }
    }

    public override void Apply4PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.OnHealthChangedEvent += () => OnHealthChanged(stats);
        }
        OnHealthChanged(stats);
    }
    public void OnHealthChanged(PlayerStats stats)
    {
        float hpPercent = stats.CurrentHealth / stats.MaxHealth;

        if (hpPercent > 0.5f)
        {
            if (!_isAttackApplied)
            {
                RemoveAllBuffs(stats);
                stats.ModifyDamage(_4piecesBonus);
                _isAttackApplied = true;
            }
        }
        else
        {
            if (!_isDefenseApplied)
            {
                RemoveAllBuffs(stats);
                stats.ModifyDefence(_4piecesBonus);
                _isDefenseApplied = true;
            }
        }
    }
    private void RemoveAllBuffs(PlayerStats stats)
    {
        if (_isAttackApplied)
        {
            stats.ModifyDamage(-_4piecesBonus);
            _isAttackApplied = false;
        }

        if (_isDefenseApplied)
        {
            stats.ModifyDefence(-_4piecesBonus);
            _isDefenseApplied = false;
        }
    }
    public override void Apply6PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.OnPreventDeath += () => TryPreventDeath(stats);
            stats.OnDefenseChanged += () => RecalculateAttackBonus(stats);
            RecalculateAttackBonus(stats);
        }
    }
    private void RecalculateAttackBonus(PlayerStats stats)
    {
        RemoveAttackBonus(stats);

        _currentAddedAttackBonus = stats.Defence * _damageBonus;
        stats.ModifyDamage(_currentAddedAttackBonus, false);
    }
    private void RemoveAttackBonus(PlayerStats stats)
    {
        if (_currentAddedAttackBonus > 0)
        {
            stats.ModifyDamage(-_currentAddedAttackBonus, false);
            _currentAddedAttackBonus = 0f;
        }
    }
    private bool TryPreventDeath(PlayerStats stats)
    {
        if (Time.time >= _lastProcTime + _cooldownTime)
        {
            _lastProcTime = Time.time;

            float healAmount = stats.MaxHealth * _heal;
            stats.Heal(healAmount);

            return true;
        }

        return false;
    }
    public override void Remove2PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null) stats.ModifyHp(-_hpBonus);
    }

    public override void Remove4PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.OnHealthChangedEvent -= () => OnHealthChanged(stats);

            RemoveAllBuffs(stats);
        }
    }
    public override void Remove6PiecesBonus(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.OnPreventDeath -= () => TryPreventDeath(stats);
            stats.OnDefenseChanged -= () => RecalculateAttackBonus(stats);
            RemoveAttackBonus(stats);
        }
        
    }
}
