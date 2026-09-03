using TMPro;
using UnityEngine;

public class CharactersStats : MonoBehaviour
{
    [Header("UI Text Fields")]
    [SerializeField] private TMP_Text _attackText;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _defenseText;
    [SerializeField] private TMP_Text _critChanceText;
    [SerializeField] private TMP_Text _critDamageText;
    [SerializeField] private TMP_Text _missChanceText;
    [SerializeField] private TMP_Text _hpRegenText;
    [SerializeField] private TMP_Text _attackSpeedText;
    [SerializeField] private TMP_Text _doubleAttackText;
    [SerializeField] private TMP_Text _luckText;
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private TMP_Text _radiusText;
    [SerializeField] private TMP_Text _vampirismText;

    [SerializeField] private PlayerStats _stats;

    public void UpdateStatsUI()
    {
        if (_stats == null) return;

        _attackText.text = _stats.Damage.ToString("0.#");
        _defenseText.text = _stats.Defence.ToString("0.#");
        _hpText.text = _stats.MaxHealth.ToString("0.#");
        _critChanceText.text = $"{_stats.CritChance:0.#} %";
        _critDamageText.text = $"{_stats.CritDamage * 100:0.#} %"; 
        _missChanceText.text = $"{_stats.TotalDodgeChance:0.#} %"; 
        _hpRegenText.text = $"{_stats.HpRegenPercent:0.#} %"; 
        _attackSpeedText.text = $"{_stats.AttackSpeed:0.#} ({_stats.AttackSpeedDelay:0.#} c.)"; 
        _doubleAttackText.text = $"{_stats.TotalDoubleStrikeChance:0.#} %"; 
        _luckText.text = $"{_stats.Luck:0.#}"; 
        _speedText.text = $"{_stats.Speed:0.#}"; 
        _radiusText.text = $"{_stats.Radius * 100:0.#}"; 
        _vampirismText.text = $"{_stats.Vampirism:0.#} %"; 
    }
    private void OnEnable()
    {
        _stats.OnStatsChanged += UpdateStatsUI;
        UpdateStatsUI();
    }

    private void OnDisable()
    {
        _stats.OnStatsChanged -= UpdateStatsUI;
    }
}
