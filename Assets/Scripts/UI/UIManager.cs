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
    [SerializeField] private TMP_Text _name;

    [Header("Stats text")]
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;

    private bool _isPaused = false;
    private bool _isGameOver = false;
    private void Awake()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            string savedName = PlayerPrefs.GetString("PlayerName", "Игрок");
            _name.text = savedName;
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
        _isPaused = true;
        _pauseMenuPanel.SetActive(true);
        UpdatePlayerStatsUI();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        _pauseMenuPanel.SetActive(false); 

        Time.timeScale = 1f;
    }
    public IEnumerator TriggerGameOver()
    {
        _isGameOver = true;
        float currentTime = 0f;
        _gameOverPanel.SetActive(true);
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

    public void QuitToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu"); 
    }
    public void Resatrt()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    private void UpdatePlayerStatsUI()
    {
        PlayerStats stats = FindFirstObjectByType<PlayerStats>();
        PlayerAttack attack = FindFirstObjectByType<PlayerAttack>();

        if (stats != null)
        {
            leftText.text = $"Атака - {stats.Damage}\nХп - {stats.MaxHealth}\nРегенерация Хп - 2\nСкорость атаки - {attack.AttackSpeed}с\nРадиус атаки - {attack.Radius}м";
            rightText.text = $"Шанс крита - 56%\nКрит урон - 120%\nШанс уворота - 15%\nШанс повторной атаки - 3%";
        }
        
    }
}
