using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisableButtonSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private Button _button;
    private bool _waitingForMouseMove;
    private Vector3 _startMousePosition;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    private void OnEnable()
    {
        _waitingForMouseMove = true;
        _startMousePosition = Input.mousePosition;

        ResetState();
    }
    private void Update()
    {
        if (_waitingForMouseMove)
        {
            if (Vector3.Distance(Input.mousePosition, _startMousePosition) > 2f)
            {
                _waitingForMouseMove = false;
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_waitingForMouseMove)
        {
            ResetState();
            return;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ResetState();
    }
    private void ResetState()
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (_button != null)
        {
            _button.OnPointerExit(new PointerEventData(EventSystem.current));
        }
    }
}
