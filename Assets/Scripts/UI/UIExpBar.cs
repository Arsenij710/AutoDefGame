using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIExpBar : MonoBehaviour
{
    [System.Serializable]
    public struct ExpBarData
    {
        public Slider slider;
        public TMP_Text levelTextOnly;
        public TMP_Text levelTextWithPrefix;
    }
    public static UIExpBar Instance { get; private set; }

    [SerializeField] private ExpBarData[] _expBars;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void UpdateExpBar(float currentExp, float maxExp, int currentLevel = -1)
    {
        foreach (var bar in _expBars)
        {
            if (bar.slider == null) continue;

            bar.slider.maxValue = maxExp;
            bar.slider.value = currentExp;

            if (bar.slider == null) continue;

            if (currentLevel != -1)
            {
                if (bar.levelTextOnly != null)
                {
                    bar.levelTextOnly.text = currentLevel.ToString();
                }

                if (bar.levelTextWithPrefix != null)
                {
                    bar.levelTextWithPrefix.text = $"Lvl. {currentLevel}";
                }
            }
        }
    }
}
