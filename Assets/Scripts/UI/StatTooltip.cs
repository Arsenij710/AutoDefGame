using UnityEngine;
using UnityEngine.EventSystems;

public class StatTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [TextArea(2, 5)]
    [SerializeField] private string _descriptionText = "Описание этой характеристики...";
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowStatTooltip(_descriptionText, _rectTransform);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideStatTooltip();
        }
    }
}
