using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("Equip Slot Options")]
    public bool IsEquipSlot;
    [SerializeField] private ArtifactSlotType _slotType;

    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _rarityFrame;
    [SerializeField] private Image _backText;
    [SerializeField] private TextMeshProUGUI _levelText;
    public RuntimeArtifact CurrentArtifact;
    public void Setup(RuntimeArtifact artifact)
    {
        if (this == null || _iconImage == null) return;
        CurrentArtifact = artifact;

        if (artifact == null)
        {
            ClearSlot();
            return;
        }

        _iconImage.sprite = artifact.data.icon;
        if (_iconImage != null)
        {
            _iconImage.enabled = true;
        }
        if (_backText != null)
        {
            _backText.enabled = true;
        }

        if (_levelText != null)
            _levelText.text = artifact.level >= 0 ? $"{artifact.level} lvl" : "";

        if (_rarityFrame != null)
        { 
            _rarityFrame.color = StatUtils.GetRarityColor(artifact.rarity);
            _rarityFrame.enabled = true;
        }

    }
    public void ClearSlot()
    {
        if (this == null || _iconImage == null) return;
        CurrentArtifact = null;
        _iconImage.enabled = false;
        _backText.enabled = false;
        _rarityFrame.enabled = false;
        if (_levelText != null) _levelText.text = "";
    }
    
}
