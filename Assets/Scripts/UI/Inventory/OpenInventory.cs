using UnityEngine;
using UnityEngine.EventSystems;

public class OpenInventory : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryWindow;
    private bool _isPaused = false;

    void Update()
    {
        if (UIManager.IsGameOver) return;
        if (UpgradeManager.IsUpgradeOpen) return;
        if (UIManager.Instance._isPaused ) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_isPaused)
            {
                UnToggleInventory();
            }
            else
            {
                ToggleInventory();
            }
        }
    }
    public void ToggleInventory()
    {
        ShowCursor();

        _isPaused = true;
        _inventoryWindow.SetActive(true);
        Time.timeScale = 0f;
        ArtifactTooltip.Instance?.OnInventoryOpened();

        if (GameStatsManager.Instance != null)
        {
            GameStatsManager.Instance.StopTimer();
        }
        InventoryUI.Instance?.UpdateInventoryUI();
    }

    public void UnToggleInventory()
    {
        ArtifactTooltip.Instance?.HideTooltip();
        HideCursor();
        _isPaused = false;
        _inventoryWindow.SetActive(false);

        Time.timeScale = 1f;
        if (GameStatsManager.Instance != null)
        {
            GameStatsManager.Instance.StartTimer();
        }
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
