using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    [SerializeField] private Toggle _firstTabToggle;
    [SerializeField] private Transform _unselectedContainer;
    [SerializeField] private Transform _selectedContainer;
    private Toggle[] _allToggles;
    private void Awake()
    {
        _allToggles = GetComponentsInChildren<Toggle>(true);
        foreach (var toggle in _allToggles)
        {
            toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle, isOn));

            if (toggle.isOn)
            {
                OnToggleChanged(toggle, true);
            }
        }
    }
    public void OnToggleChanged(Toggle toggle, bool isOn)
    {
        if (isOn)
        {
            toggle.transform.SetParent(_selectedContainer, false);
        }
        else
        {
            toggle.transform.SetParent(_unselectedContainer, false);
        }
    }
    private void OnEnable()
    {
        ResetToFirstTab();
    }
    public void ResetToFirstTab()
    {
        if (_firstTabToggle == null) return;

        _firstTabToggle.isOn = true;

        foreach (var toggle in _allToggles)
        {
            bool isFirst = (toggle == _firstTabToggle);
            toggle.SetIsOnWithoutNotify(isFirst);
            toggle.transform.SetParent(isFirst ? _selectedContainer : _unselectedContainer, false);
        }
    }
}
