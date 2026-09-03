using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Sprite _defaultFrame;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _textBackImage;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Image _rarityFrame;

    [Header("Rarity Sprites")]
    [SerializeField] private Sprite[] _raritySprites;

    [Header("Equip Slot Options")]
    public bool IsEquipSlot;

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

        if (_textBackImage != null)
        {
            _textBackImage.enabled = true;
        }

        if (_levelText != null)
            _levelText.text = artifact.level >= 0 ? $"+{artifact.level}" : "";

        int rarityIndex = (int)artifact.rarity;

        if (_raritySprites != null && rarityIndex < _raritySprites.Length)
        {
            _rarityFrame.sprite = _raritySprites[rarityIndex];
        }

    }
    public void ClearSlot()
    {
        if (this == null || _iconImage == null) return;
        CurrentArtifact = null;
        _iconImage.enabled = false;
        _textBackImage.enabled = false;
        _rarityFrame.sprite = _defaultFrame;
        if (_levelText != null) _levelText.text = "";
    }
    
}
