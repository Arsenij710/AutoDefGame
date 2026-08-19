using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private GameObject _warningPanel;

    [Header("GamoOver text")]
    [SerializeField] private TextMeshProUGUI _finalScoreText;  
    [SerializeField] private TextMeshProUGUI _killedEnemiesText;
    [SerializeField] private TextMeshProUGUI _goldEarnedText;

    public static UIManager Instance { get; private set; }
    public static bool IsGameOver { get; private set; }


    PlayerStats stats;
    PlayerAttack attack;
    private bool _isPaused = false;
    private string _name;
    private bool _isExitingToMenu = false;

    private void Awake()
    {
        stats = FindFirstObjectByType<PlayerStats>();
        attack = stats.GetComponent<PlayerAttack>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (PlayerPrefs.HasKey("PlayerName"))
        {
            _name = PlayerPrefs.GetString("PlayerName", "Игрок");
            _nameText.text = _name;
        }
    }
    private void Update()
    {
        if (IsGameOver) return;
        if (UpgradeManager.IsUpgradeOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        ShowCursor();
        _isPaused = true;
        _pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        if (GameStatsManager.Instance != null)
        {
            GameStatsManager.Instance.UpdateDisplay();
            GameStatsManager.Instance.StopTimer();
        }
    }

    public void ResumeGame()
    {
        HideCursor();
        _isPaused = false;
        _pauseMenuPanel.SetActive(false); 

        Time.timeScale = 1f;
        if (GameStatsManager.Instance != null)
        {
            GameStatsManager.Instance.StartTimer();
        }
    }
    
    public IEnumerator TriggerGameOver()
    {
        ShowCursor();
        IsGameOver = true;
        float currentTime = 0f;
        _gameOverPanel.SetActive(true);
        ChangeGameoverText();
        GoldManager.Instance.CommitGold();

        CanvasGroup restartCanvasGroup = _gameOverPanel.GetComponent<CanvasGroup>();
        while (currentTime < 1f)
        {
            currentTime += Time.deltaTime;
            restartCanvasGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / 1f);
            yield return null;
        }

        restartCanvasGroup.alpha = 1f;
        Time.timeScale = 0f;
    }
    private void ChangeGameoverText()
    {
        int highScore = PlayerPrefs.GetInt("Record", 0);
        string highScoreaName = PlayerPrefs.GetString("RecordName", "Игрок");
        int _currentScore = ScoreManager.Instance.GetCurrentScore();
        if (_currentScore > highScore)
        {
            PlayerPrefs.SetInt("Record", _currentScore);
            PlayerPrefs.SetString("RecordName", _name);
            PlayerPrefs.Save();
            _finalScoreText.text = $"Новый рекорд!\n{_name} - {_currentScore}";
        }
        else
        {
            _finalScoreText.text = $"Рекорд не побит!\n{highScoreaName} - {highScore}\nВаш текущий счёт: \n{_currentScore}";

        }

        _killedEnemiesText.text = $"Врагов убито: {ScoreManager.Instance.GetEnemyKilledCount()}";

        
        int coinsEarned = GoldManager.Instance.GoldEarnedThisRun;
        _goldEarnedText.text = $"+ {coinsEarned}";
        
    }

    public void QuitToMenu()
    {
        if (IsGameOver)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            _isExitingToMenu = true;
            _warningPanel.SetActive(true);
        }
    }
    public void Restart()
    {
        if (IsGameOver)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            _isExitingToMenu = false;
            _warningPanel.SetActive(true);
        }
    }
    public void ConfirmAction()
    {
        Time.timeScale = 1f;
        GoldManager.Instance.DiscardGold();

        if (_isExitingToMenu)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            HideCursor();
        }
    }

    public void CancelRestart()
    {
        _warningPanel.SetActive(false);
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
