using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private EnemyController _mainController;

    public void TriggerAnimationAttackDamage()
    {
        if (_mainController != null)
        {
            _mainController.ExecuteAoEDamage();
        }
    }
}
