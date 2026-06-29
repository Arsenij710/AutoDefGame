using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;  
    public int maxCount;

    [HideInInspector]
    public int currentCount = 0;

    private PlayerStats stats;
    private PlayerAttack attack;
    public void ApplyUpgrade()
    {
        currentCount++;
        if (stats == null || attack == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            stats = player.GetComponent<PlayerStats>();
            attack = player.GetComponent<PlayerAttack>();
        }
        if (stats != null && attack != null)
        {
            if (upgradeName == "Атака")
            {
                stats.UpgradeDamage();
            }
            else if(upgradeName == "Хп")
            {
                stats.UpgradeMaxHealth();
            }
            else if (upgradeName == "Скорость атаки")
            {
                attack.UpgradeAttackSpeed();
            }
            else if (upgradeName == "Восстановление хп")
            {
                stats.Heal((int)(stats.MaxHealth * 0.3f));
            }
            else if (upgradeName == "Радиус")
            {
                attack.UpgradeRadius();
            }

        }
    }

    public bool IsMaxedOut()
    {
        return currentCount >= maxCount;
    }
    private void OnEnable()
    {
        currentCount = 0;
        stats = null;
        attack = null;
    }
}
