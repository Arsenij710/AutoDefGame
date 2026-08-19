using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixDropDown : MonoBehaviour, IPointerClickHandler
{
    [Header("Настройки слоя")]
    [SerializeField] private string _targetSortingLayer = "UI_Upper";
    [SerializeField] private int _targetSortingOrder = 999; 

    public void OnPointerClick(PointerEventData eventData)
    {
        Invoke(nameof(ApplyFix), 0.01f);
    }

    private void ApplyFix()
    {
        Transform dropdownList = transform.Find("Dropdown List");

        if (dropdownList == null)
        {
            dropdownList = transform.Find("DropdownList");
        }

        if (dropdownList != null)
        {
            Canvas listCanvas = dropdownList.GetComponent<Canvas>();
            if (listCanvas == null)
            {
                listCanvas = dropdownList.gameObject.AddComponent<Canvas>();
            }
            listCanvas.overrideSorting = true;
            listCanvas.sortingLayerName = _targetSortingLayer;
            listCanvas.sortingOrder = _targetSortingOrder;

            GraphicRaycaster raycaster = dropdownList.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                dropdownList.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
    }
}
