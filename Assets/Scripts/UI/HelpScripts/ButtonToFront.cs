using UnityEngine;
using UnityEngine.UI;

public class ButtonToFront : MonoBehaviour
{
    [Header("Настройки слоев")]
    [SerializeField] private int _normalSortOrder = 1;
    [SerializeField] private int _activeSortOrder = 10;

    private Toggle _toggle;
    private Canvas _canvas;
    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _canvas = GetComponent<Canvas>();

        if (GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }
    }
    private void Start()
    {
        _toggle.onValueChanged.AddListener(OnToggleChanged);

        OnToggleChanged(_toggle.isOn);
    }

    public void OnToggleChanged(bool isActive)
    {
        if (_canvas == null)
        {
            _canvas = GetComponent<Canvas>();
        }

        if (_canvas != null)
        {
            _canvas.sortingOrder = isActive ? _activeSortOrder : _normalSortOrder;
        }
    }

    private void OnDestroy()
    {
        _toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
}
