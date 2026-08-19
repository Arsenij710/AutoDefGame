using UnityEngine;
using UnityEngine.UI;

public class ReseterSettingsButton : MonoBehaviour
{
    [System.Serializable]
    public struct TabElements
    {
        public Toggle toggleComponent;
        public GameObject checkmarkImage;
        public GameObject settingsPanel;
    }

    [SerializeField] private TabElements[] _allTabs;

    [SerializeField] private int _defaultTabIndex = 0;

    private void OnEnable()
    {
        if (_allTabs == null || _allTabs.Length == 0) return;

        for (int i = 0; i < _allTabs.Length; i++)
        {
            bool isDefault = (i == _defaultTabIndex);
            ButtonToFront toggleScript = _allTabs[i].toggleComponent.GetComponent<ButtonToFront>();

            if (_allTabs[i].settingsPanel != null)
            {
                _allTabs[i].settingsPanel.SetActive(isDefault);
            }

            //if (_allTabs[i].checkmarkImage != null)
            //{
            //    _allTabs[i].checkmarkImage.SetActive(isDefault);
            //}

            if (_allTabs[i].toggleComponent != null)
            {
                _allTabs[i].toggleComponent.SetIsOnWithoutNotify(isDefault);
            }
            if (toggleScript != null)
            {
                toggleScript.OnToggleChanged(isDefault);
            }
        }
    }
}
