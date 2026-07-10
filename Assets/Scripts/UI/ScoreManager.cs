using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private TMP_Text _scoreText;
    private int _currentScore = 0;
    private int _enemyKilled = 0;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        _scoreText = GetComponent<TMP_Text>();
        UpdateScoreUI();
    }
    public void AddScore(int points)
    {
        _currentScore += points;
        _enemyKilled++;
        UpdateScoreUI();
    }

    public int GetCurrentScore()
    {
        return _currentScore;
    }
    public int GetEnemyKilledCount()
    {
        return _enemyKilled;
    }
    private void UpdateScoreUI()
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Score: {_currentScore}";
        }
    }
}
