using UnityEngine;
using UnityEngine.UI;

public class UIHPBar : MonoBehaviour
{
    private Slider _slider;
    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetupMaxHealth(int maxHealth)
    {
        _slider.maxValue = maxHealth;
        _slider.value = maxHealth;
    }

    public void UpdateHealthBar(int currentHealth)
    {
        _slider.value = currentHealth;
    }
}
