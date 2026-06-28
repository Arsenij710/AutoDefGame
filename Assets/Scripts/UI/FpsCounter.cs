using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class FpsCounter : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private TMP_Text _fpsText; 
    [SerializeField] private float _updateInterval = 0.5f; 

    private float _accumulatedTime = 0f;
    private int _frameCount = 0;
    private void Start()
    {
        int showFps = PlayerPrefs.GetInt("ShowFps", 0);
        Debug.Log(showFps);
        FpsOnOff(showFps);
    }
    private void Update()
    {
        _accumulatedTime += Time.deltaTime;
        _frameCount++;

        if (_accumulatedTime >= _updateInterval)
        {
            int fps = Mathf.RoundToInt(_frameCount / _accumulatedTime);

            _fpsText.text = $"FPS: {fps}";

            _accumulatedTime = 0f;
            _frameCount = 0;
        }
    }
    public void FpsOnOff(int flag)
    {
        if (flag == 1)
        {
            enabled = true;
            _fpsText = GetComponent<TMP_Text>();
        }
        else
        {
            enabled = false;
            if (_fpsText != null) _fpsText.text = "";
        }
    }
}
