using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [Header("Baffs")]
    public List<UpgradeData> allUpgrades; 

    [Header("UI Buttons")]
    public UpgradeButton[] uiButtons;

    [Header("Reset")]
    public Button rerollButton;
    public TextMeshProUGUI rerollText;

    public static bool IsUpgradeOpen { get; private set; }

    private PlayerStats stats;
    private PlayerAttack attack;
    private int rerollsAvailable = 1;
    void Awake()
    {
        foreach (var upgrade in allUpgrades)
        {
            upgrade.currentCount = 0;
        }
        stats = FindFirstObjectByType<PlayerStats>();
        attack = stats.GetComponent<PlayerAttack>();
    }
    public void OpenUpgradePanel()
    {
        ShowCursor();
        Time.timeScale = 0f;
        gameObject.SetActive(true);
        rerollsAvailable = 1;
        IsUpgradeOpen = true;

        UpdateRerollButtonUI();
        GenerateUpgradeSelection();
    }
    public void RerollUpgrades()
    {
        if (rerollsAvailable > 0)
        {
            rerollsAvailable--;
            UpdateRerollButtonUI();
            GenerateUpgradeSelection(); 
        }
    }
    private void UpdateRerollButtonUI()
    {
        rerollText.text = $"{rerollsAvailable}/1";
        rerollButton.interactable = (rerollsAvailable > 0);
    }
    private void GenerateUpgradeSelection()
    {
        List<UpgradeData> availablePool = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < uiButtons.Length; i++)
        {
            int randomIndex = Random.Range(0, availablePool.Count);
            UpgradeData selectedUpgrade = availablePool[randomIndex];

            uiButtons[i].Setup(selectedUpgrade, ClosePanel, stats, attack);

            availablePool.RemoveAt(randomIndex);
        }
    }
    private void ClosePanel()
    {
        gameObject.SetActive(false);
        IsUpgradeOpen = false;
        Time.timeScale = 1f;
        HideCursor();
    }
    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
