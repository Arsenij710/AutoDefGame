using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private InventorySlot _slot;
    private RectTransform _rectTransform;
    private RuntimeArtifact _artifact;
    private void Awake()
    {
        if (_slot == null) _slot = GetComponent<InventorySlot>();
        _rectTransform = GetComponent<RectTransform>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _artifact = _slot.CurrentArtifact;

        if (_artifact == null || _rectTransform == null) return;
        if (!_slot.gameObject.activeInHierarchy) return;

        ArtifactTooltip.Instance.ShowPreview(_artifact, _rectTransform, _slot.IsEquipSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ArtifactTooltip.Instance == null) return;

        if (!ArtifactTooltip.Instance.IsPinned)
        {
            ArtifactTooltip.Instance.HideTooltip();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_artifact == null || _rectTransform == null) return;
        
        if (eventData.button == PointerEventData.InputButton.Left ||
            eventData.button == PointerEventData.InputButton.Right)
        {
            ArtifactTooltip.Instance.PinTooltip(_artifact, _rectTransform, _slot.IsEquipSlot);
        }
    }
}
