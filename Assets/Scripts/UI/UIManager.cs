using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _name;


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

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        _pauseMenuPanel.SetActive(false); 

        Time.timeScale = 1f;
    }
    public void TriggerGameOver()
    {
        _isGameOver = true;
        _gameOverPanel.SetActive(true);
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
}
