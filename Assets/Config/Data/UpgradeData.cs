using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;  
    public int maxCount;

    [HideInInspector]
    public int currentCount = 0;
    public void ApplyUpgrade(PlayerStats stats, PlayerAttack attack)
    {
        currentCount++;
      
        if (stats != null && attack != null)
        {
            if (upgradeName == "Атака")
            {
                attack.UpgradeDamage();
            }
            else if (upgradeName == "Хп")
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
            else if (upgradeName == "Регенерация ХП")
            {
                stats.HPRegenUpgrade();
            }
            else if (upgradeName == "Крит Урон")
            {
                attack.CritDamageUpgrade();
            }
            else if (upgradeName == "Крит Шанс")
            {
                attack.CritChanceUpgrade();
            }
            else if (upgradeName == "Уклонение")
            {
                stats.MissUpgrade();
            }
            else if (upgradeName == "Повторная атака")
            {
                attack.ReAttackUpgrade();
            }
            else if (upgradeName == "Удача")
            {
                stats.LuckUpgrade();
            }

        }
    }

    public bool IsMaxedOut()
    {
        return currentCount >= maxCount;
    }
}
