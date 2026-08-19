using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }
    private int _totalGold;

    [Header("UI")]
    [SerializeField] private GoldPanelAppear _hudGoldCounter;

    private int _goldEarnedThisRun;
    public int TotalGold => _totalGold;
    public int GoldEarnedThisRun => _goldEarnedThisRun;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadGold();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ResetRunStats()
    {
        _goldEarnedThisRun = 0;
    }
    private void LoadGold() => _totalGold = PlayerPrefs.GetInt("Gold", 0);
    private void SaveGold() => PlayerPrefs.SetInt("Gold", _totalGold);
    public void AddGold(int amount)
    {
        _goldEarnedThisRun += amount;
        _hudGoldCounter.OnGoldChanged(_goldEarnedThisRun);
    }
    public void CommitGold()
    {
        _totalGold += _goldEarnedThisRun;
        _goldEarnedThisRun = 0;
        SaveGold();
    }

    public void DiscardGold()
    {
        _goldEarnedThisRun = 0;
    }

    //public bool TrySpendGold(int price)
    //{
    //    if (_totalGold >= price)
    //    {
    //        _totalGold -= price;
    //        SaveGold();
    //        _hudGoldCounter.OnGoldChanged(_goldEarnedThisRun);
    //        return true;
    //    }
    //    return false;
    //}


    [ContextMenu("Reset Gold Data")]
    public void ResetGold()
    {
        _totalGold = 0;
        _goldEarnedThisRun = 0;
        SaveGold();
    }
}
