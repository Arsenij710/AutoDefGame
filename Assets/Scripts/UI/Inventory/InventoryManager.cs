using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Content Panels")]
    [SerializeField] private GameObject _inventoryContent;
    [SerializeField] private GameObject _craftContent;
    [SerializeField] private GameObject _talentsContent;
    [SerializeField] private GameObject _statsContent;
    private void Start()
    {
        ShowInventoryTab();
    }
    public void ShowInventoryTab()
    {
        _inventoryContent.SetActive(true);
        _craftContent.SetActive(false);
        _talentsContent.SetActive(false);
        _statsContent.SetActive(false);
    }
    public void ShowCraftTab()
    {
        _inventoryContent.SetActive(false);
        _craftContent.SetActive(true);
        _talentsContent.SetActive(false);
        _statsContent.SetActive(false);
    }
    public void ShowTalentTab()
    {
        _inventoryContent.SetActive(false);
        _craftContent.SetActive(false);
        _talentsContent.SetActive(true);
        _statsContent.SetActive(false);
    }
    public void ShowStatsTab()
    {
        _inventoryContent.SetActive(false);
        _craftContent.SetActive(false);
        _talentsContent.SetActive(false);
        _statsContent.SetActive(true);
    }
}
