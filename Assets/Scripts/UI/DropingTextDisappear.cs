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
            _textMesh.text = $"Крит!!\n{amount.ToString()}";
        }
        else if (isMiss)
        {
            _textMesh.text = $"Уклонение!!!";

        }
        else
        {
            _textMesh.text = amount.ToString();
        }
        _textMesh.color = color;
        _textColor.a = 1f;
        _moveYSpeed = 1.5f;

        _disappearTimer = DISAPPEAR_MAX_TIME; 
        _moveXSpeed = Random.Range(-1f, 1f);
    }
    private void Update()
    {
        _moveYSpeed -= 6f * Time.deltaTime;
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
