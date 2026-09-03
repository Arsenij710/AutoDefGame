using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIHPBar : MonoBehaviour
{
    [System.Serializable]
    public struct HPBarData
    {
        public Slider slider;
        public TMP_Text textMesh;
    }
    public static UIHPBar Instance { get; private set; }

    [SerializeField] private HPBarData[] _hpBars;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetupMaxHealth(float maxHealth)
    {
        foreach (var bar in _hpBars)
        {
            if (bar.slider == null) continue;

            bar.slider.maxValue = maxHealth;
            bar.slider.value = maxHealth;
            UpdateBarText(bar);
        }
    }

    public void UpdateHealthBar(float currentHealth)
    {
        foreach (var bar in _hpBars)
        {
            if (bar.slider == null) continue;

            bar.slider.value = currentHealth;
            UpdateBarText(bar);
        }
    }
    private void UpdateBarText(HPBarData bar)
    {
        if (bar.textMesh != null)
        {
            bar.textMesh.text = $"{Mathf.RoundToInt(bar.slider.value)}/{Mathf.RoundToInt(bar.slider.maxValue)}";
        }
    }
}
