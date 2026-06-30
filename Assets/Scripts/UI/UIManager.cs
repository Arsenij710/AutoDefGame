using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private EnemySpawner _spawner;
    private bool _isPaused = false;
    private bool _isGameOver = false;
    private string _name;
    private void Awake()
    {
        _spawner = FindFirstObjectByType<EnemySpawner>();
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
        ScoreManager score = FindFirstObjectByType<ScoreManager>(); 
        int highScore = PlayerPrefs.GetInt("Record", 0);
        string highScoreaName = PlayerPrefs.GetString("RecordName", "Игрок");
        int _currentScore = score.GetCurrentScore();
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

        _killedEnemiesText.text = $"Врагов убито: {score.GetEnemyKilledCount()}";
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
        PlayerStats stats = FindFirstObjectByType<PlayerStats>();
        PlayerAttack attack = FindFirstObjectByType<PlayerAttack>();
        currWave.text = $"Текущая волна: {_spawner.GetCurrentWave()}\nУлучшения";
        if (stats != null)
        {
            leftText.text = $"Атака - {stats.Damage}\nХп - {stats.MaxHealth}\nРегенерация Хп - 2\nСкорость атаки - {attack.AttackSpeed}с\nРадиус атаки - {attack.Radius}м";
            rightText.text = $"Шанс крита - 56%\nКрит урон - 120%\nШанс уворота - 15%\nШанс повторной атаки - 3%";
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
