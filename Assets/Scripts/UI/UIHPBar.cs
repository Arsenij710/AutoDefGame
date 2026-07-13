using UnityEngine;
using UnityEngine.UI;

public class UIHPBar : MonoBehaviour
{
    public static UIHPBar Instance { get; private set; }

    private Slider _slider;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _slider = GetComponent<Slider>();
    }

    public void SetupMaxHealth(float maxHealth)
    {
        _slider.maxValue = maxHealth;
        _slider.value = maxHealth;
    }

    public void UpdateHealthBar(float currentHealth)
    {
        _slider.value = currentHealth;
    }
}
