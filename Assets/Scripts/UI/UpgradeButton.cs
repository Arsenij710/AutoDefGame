using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public TextMeshProUGUI limitText;
    public TextMeshProUGUI labelText;

    private Button buttonComponent;
    private UpgradeData currentData;
    private Action onUpgradedCallback;
    private void Awake()
    {
        buttonComponent = GetComponent<Button>();
    }
    public void Setup(UpgradeData data, Action onUpgraded)
    {
        currentData = data;
        onUpgradedCallback = onUpgraded;

        UpdateUI();

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(ClickUpgrade);
    }
    public void UpdateUI()
    {
        if (currentData == null) return;
        if (currentData.maxCount < 500)
        {
            limitText.text = $"{currentData.currentCount}/{currentData.maxCount}";
        }
        else
        {
            limitText.text = $"+";

        }
        labelText.text = currentData.upgradeName;
        buttonComponent.interactable = !currentData.IsMaxedOut();
    }
    private void ClickUpgrade()
    {
        if (currentData != null && !currentData.IsMaxedOut())
        {
            currentData.ApplyUpgrade();
            onUpgradedCallback?.Invoke(); 
        }
    }
}
