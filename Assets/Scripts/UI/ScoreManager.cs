using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private TMP_Text _scoreText;
    private int _currentScore = 0;
    private int _enemyKilled = 0;
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
