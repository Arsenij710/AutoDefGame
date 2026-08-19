using System.Collections;
using TMPro;
using UnityEngine;

public class GoldPanelAppear : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Animation")]
    [SerializeField] private float _showDuration = 2f;
    [SerializeField] private float _fadeSpeed = 2f;

    private Coroutine _fadeCoroutine;
    private void Awake()
    {
        _canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        if (GoldManager.Instance != null)
        {
            _goldText.text = GoldManager.Instance.TotalGold.ToString();
        }
    }
    public void OnGoldChanged(int newTotalGold)
    {
        _goldText.text = newTotalGold.ToString();

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(ShowAndHideRoutine());
    }
    private IEnumerator ShowAndHideRoutine()
    {
        while (_canvasGroup.alpha < 1f)
        {
            _canvasGroup.alpha += _fadeSpeed * Time.deltaTime;
            yield return null;
        }
        _canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(_showDuration);

        while (_canvasGroup.alpha > 0f)
        {
            _canvasGroup.alpha -= _fadeSpeed * Time.deltaTime;
            yield return null;
        }
        _canvasGroup.alpha = 0f;
    }
}
