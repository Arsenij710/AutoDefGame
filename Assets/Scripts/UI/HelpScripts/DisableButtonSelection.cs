using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisableButtonSelection : MonoBehaviour, IPointerUpHandler
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_button != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
