using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance;

    [Header("Pause text")]
    public TextMeshProUGUI _gameText;
    public TextMeshProUGUI _playerText;

    private float _elapsedTime;
    private bool _isTimerRunning = true;
    private int _totalDamageDealt = 0;
    private int _maxSingleHitDamage = 0;
    private int _totalDamageReceived = 0;
    private struct DamageEvent
    {
        public int timeStamp;
        public int damage;
    }
    private Queue<DamageEvent> _damageHistory = new Queue<DamageEvent>();
    private int _dpsWindowSeconds = 3;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _elapsedTime = 0f;
        _isTimerRunning = true;
        _totalDamageDealt = 0;
        _maxSingleHitDamage = 0;
        _totalDamageReceived = 0;
        _damageHistory.Clear();
    }
    private void Update()
    {
        if (_isTimerRunning)
        {
            _elapsedTime += Time.deltaTime;
        }
    }
    public void AddDamage(int damageAmount)
    {
        if (damageAmount > 0)
        {
            _totalDamageDealt += damageAmount;
            if (damageAmount > _maxSingleHitDamage)
            {
                _maxSingleHitDamage = damageAmount;
            }

            DamageEvent newEvent = new DamageEvent
            {
                timeStamp = Mathf.FloorToInt(_elapsedTime),
                damage = damageAmount
            };
            _damageHistory.Enqueue(newEvent);
        }
    }
    private void ClearOldDamageEvents()
    {
        int currentTime = Mathf.FloorToInt(_elapsedTime);
        while (_damageHistory.Count > 0 && currentTime - _damageHistory.Peek().timeStamp >= _dpsWindowSeconds)
        {
            _damageHistory.Dequeue();
        }
    }
    private int CalculateCurrentDps()
    {
        if (_damageHistory.Count == 0) return 0;

        int sum = 0;
        foreach (var damageEvent in _damageHistory)
        {
            sum += damageEvent.damage;
        }

        return Mathf.RoundToInt((float)sum / _dpsWindowSeconds);
    }
    public void AddReceivedDamage(int damageAmount)
    {
        if (damageAmount > 0)
        {
            _totalDamageReceived += damageAmount;
        }
    }
    public void UpdateDisplay()
    {
        string timeText = GetFormattedTime();
        string enemyKilled = ScoreManager.Instance.GetEnemyKilledCount().ToString("N0").Replace(",", " ");
        string goldEarned = GoldManager.Instance.GoldEarnedThisRun.ToString("N0").Replace(",", " ");
        _gameText.text = $"{timeText}\n{enemyKilled}\nсобрано артов\n{goldEarned}";

        string damageDealt = _totalDamageDealt.ToString("N0").Replace(",", " ");
        string maxDamageDealt = _maxSingleHitDamage.ToString("N0").Replace(",", " ");
        string damageReceive = _totalDamageReceived.ToString("N0").Replace(",", " ");
        ClearOldDamageEvents();
        string dps = CalculateCurrentDps().ToString("N0").Replace(",", " ");
        _playerText.text = $"{damageDealt}\n{maxDamageDealt}\n{damageReceive}\n{dps}";
            
        
    }
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(_elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60f);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer() => _isTimerRunning = false;
    public void StartTimer() => _isTimerRunning = true;
}
