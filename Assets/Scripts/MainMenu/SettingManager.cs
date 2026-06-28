using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("Вкладки (Content Panels)")]
    [SerializeField] private GameObject _videoContent;
    [SerializeField] private GameObject _audioContent;
    [SerializeField] private GameObject _gameplayContent;

    [Header("Элементы Видео")]
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private Toggle _fullscreenToggle;
    [SerializeField] private Toggle _windowedToggle;
    [SerializeField] private Toggle _vsyncToggle;
    [SerializeField] private Toggle _damageToggle;
    [SerializeField] private Toggle _fpsToggle;

    [Header("Элементы Игры")]
    public TMP_InputField nameInputField;
    public TMP_Text placeholderText;


    private List<Resolution> _filteredResolutions;
    private void Start()
    {
        string savedName = PlayerPrefs.GetString("PlayerName", "Игрок");
        InitResolutions();
        ShowGameplayTab();
        _fullscreenToggle.isOn = Screen.fullScreen;
        placeholderText.text = savedName;
        _windowedToggle.isOn = !Screen.fullScreen;
        _vsyncToggle.isOn = (QualitySettings.vSyncCount != 0);
        _fpsToggle.isOn = (PlayerPrefs.GetInt("ShowFps", 0) == 1);
        _damageToggle.isOn = (PlayerPrefs.GetInt("ShowDamageNumbers", 0) == 1);
    }
    public void ShowVideoTab()
    {
        _videoContent.SetActive(true);
        _audioContent.SetActive(false);
        _gameplayContent.SetActive(false);
    }

    public void ShowAudioTab()
    {
        _videoContent.SetActive(false);
        _audioContent.SetActive(true);
        _gameplayContent.SetActive(false);
    }

    public void ShowGameplayTab()
    {
        _videoContent.SetActive(false);
        _audioContent.SetActive(false);
        _gameplayContent.SetActive(true);
    }

    private void InitResolutions()
    {
        Resolution[] allResolutions = Screen.resolutions;
        _filteredResolutions = new List<Resolution>();

        List<string> options = new List<string>();
        HashSet<string> uniqueResolutionKeys = new HashSet<string>();

        int currentResolutionIndex = 0;


        for (int i = 0; i < allResolutions.Length; i++)
        {

            string resKey = allResolutions[i].width + "x" + allResolutions[i].height;

            if (!uniqueResolutionKeys.Contains(resKey))
            {
                uniqueResolutionKeys.Add(resKey);

                Resolution maxRefreshRateRes = allResolutions[i];
                for (int j = 0; j < allResolutions.Length; j++)
                {
                    if (allResolutions[j].width == allResolutions[i].width &&
                        allResolutions[j].height == allResolutions[i].height)
                    {
                        if (allResolutions[j].refreshRateRatio.value > maxRefreshRateRes.refreshRateRatio.value)
                        {
                            maxRefreshRateRes = allResolutions[j];
                        }
                    }
                }

                _filteredResolutions.Add(maxRefreshRateRes);
                string optionText = maxRefreshRateRes.width + " x " + maxRefreshRateRes.height;
                options.Add(optionText);
            }
        }

        for (int i = 0; i < _filteredResolutions.Count; i++)
        {
            if (_filteredResolutions[i].width == Screen.currentResolution.width &&
                _filteredResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        _resolutionDropdown.ClearOptions();
        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _filteredResolutions[resolutionIndex];
        if (Screen.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.ExclusiveFullScreen);
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.Windowed);
        }
    }

    public void EnableFullscreen()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.fullScreen = true;
    }
    public void EnableWindowedMode()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.fullScreen = false;
    }
    public void SetVSync(bool isEnabled)
    {
        if (isEnabled)
        {
            QualitySettings.vSyncCount = 1; 
        }
        else
        {
            QualitySettings.vSyncCount = 0; 
        }
    }
    public void SetMasterVolume(float volume)
    {
        // Здесь будет управление через AudioMixer.
        // volume будет приходить от Slider (значения от 0.0001f до 1f)
        // Пример перевода в децибелы для микшера: Mathf.Log10(volume) * 20
        Debug.Log($"Общая громкость: {volume}");
    }

    public void SaveName()
    {
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            string enteredName = nameInputField.text;

            PlayerPrefs.SetString("PlayerName", enteredName);
            PlayerPrefs.Save();

            if (placeholderText != null)
            {
                placeholderText.text = enteredName;
            }
        }
    }
    public void SetDamageNumbersVisibility(bool isVisible)
    {
        PlayerPrefs.SetInt("ShowDamageNumbers", isVisible ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void SetFpsVisibility(bool isVisible)
    {
        PlayerPrefs.SetInt("ShowFps", isVisible ? 1 : 0);
        PlayerPrefs.Save();
        FpsCounter fps = FindFirstObjectByType<FpsCounter>();
        fps.FpsOnOff(isVisible ? 1 : 0);
    }
}
