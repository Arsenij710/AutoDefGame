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

    [Header("Stats text")]
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI currWave;

    [Header("GamoOver text")]
    [SerializeField] private TextMeshProUGUI _finalScoreText;  
    [SerializeField] private TextMeshProUGUI _killedEnemiesText;

    public static UIManager Instance { get; private set; }


    PlayerStats stats;
    PlayerAttack attack;
    private bool _isPaused = false;
    private bool _isGameOver = false;
    private string _name;
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
        if (_isGameOver) return;
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
        UpdatePlayerStatsUI();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        HideCursor();
        _isPaused = false;
        _pauseMenuPanel.SetActive(false); 

        Time.timeScale = 1f;
    }
    public IEnumerator TriggerGameOver()
    {
        ShowCursor();
        _isGameOver = true;
        float currentTime = 0f;
        _gameOverPanel.SetActive(true);
        ChangeGameoverText();

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
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu"); 
    }
    public void Resatrt()
    {
        HideCursor();
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    private void UpdatePlayerStatsUI()
    {
        currWave.text = $"Текущая волна: {EnemySpawner.Instance.GetCurrentWave()}\nУлучшения";
        if (stats != null)
        {
            leftText.text = $"Атака - {attack.Damage}\nХп - {(int)stats.MaxHealth}\nРегенерация Хп - {stats.HpRegenPercent * 100}%\nСкорость атаки - {attack.AttackSpeed}с\nРадиус атаки - {attack.Radius}м";
            rightText.text = $"Шанс крита - {attack.CritChance}%\nКрит урон - {attack.CritDamage * 100}%\nШанс уворота - {stats.TotalDodgeChance}%\nШанс повторной атаки - {attack.TotalDoubleStrikeChance}%";
        }
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
