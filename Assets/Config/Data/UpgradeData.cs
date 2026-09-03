using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;  
    public int maxCount;

    [HideInInspector]
    public int currentCount = 0;
    public void ApplyUpgrade(PlayerStats stats)
    {
        currentCount++;
      
        if (stats != null)
        {
            if (upgradeName == "Атака")
            {
                stats.UpgradeDamage();
            }
            else if (upgradeName == "Хп")
            {
                stats.UpgradeMaxHealth();
            }
            else if (upgradeName == "Защита")
            {
                stats.UpgradeDefence();
            }
            else if (upgradeName == "Скорость атаки")
            {
                stats.UpgradeAttackSpeed();
            }
            else if (upgradeName == "Восстановление хп")
            {
                stats.Heal(stats.MaxHealth * 0.3f);
            }
            else if (upgradeName == "Радиус")
            {
                stats.UpgradeRadius();
            }
            else if (upgradeName == "Регенерация ХП")
            {
                stats.HPRegenUpgrade();
            }
            else if (upgradeName == "Крит Урон")
            {
                stats.CritDamageUpgrade();
            }
            else if (upgradeName == "Крит Шанс")
            {
                stats.CritChanceUpgrade();
            }
            else if (upgradeName == "Уклонение")
            {
                stats.MissUpgrade();
            }
            else if (upgradeName == "Повторная атака")
            {
                stats.ReAttackUpgrade();
            }
            else if (upgradeName == "Удача")
            {
                stats.LuckUpgrade();
            }
            else if (upgradeName == "Вампиризм")
            {
                stats.VampirismUpgrade();
            }

        }
    }

    public bool IsMaxedOut()
    {
        return currentCount >= maxCount;
    }
}
