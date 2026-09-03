using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class DropingTextDisappear : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    private float _disappearTimer;
    private Color _textColor;

    private IObjectPool<DropingTextDisappear> _myPool;

    private const float DISAPPEAR_MAX_TIME = 0.5f; 
    private float _moveYSpeed = 1.5f;
    private float _moveXSpeed;
    private void Awake()
    {
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void Setup(float amount, IObjectPool<DropingTextDisappear> pool, Color color, bool isCrit=false, bool isMiss=false)
    {
        _myPool = pool;
        if (isCrit)
        {
            _textMesh.text = $"Крит!!\n{amount.ToString("0.#")}";
        }
        else if (isMiss)
        {
            _textMesh.text = $"Уклонение!!!";

        }
        else
        {
            _textMesh.text = amount.ToString("0.#");
        }
        _textMesh.color = color;
        _textColor.a = 1f;
        _moveYSpeed = Random.Range(1.5f, 3f);
        _moveXSpeed = Random.Range(-3.5f, 3.5f);

        _disappearTimer = DISAPPEAR_MAX_TIME; 
    }
    private void Update()
    {
        _moveYSpeed -= 5f * Time.deltaTime;
        transform.position += new Vector3(_moveXSpeed, _moveYSpeed, 0) * Time.deltaTime;
        _disappearTimer -= Time.deltaTime;

        if (_disappearTimer <= 0)
        {
            _textColor.a -= 5f * Time.deltaTime;
            _textMesh.color = _textColor;

            if (_textColor.a <= 0)
            {
                _myPool?.Release(this);
            }
        }
    }
}
