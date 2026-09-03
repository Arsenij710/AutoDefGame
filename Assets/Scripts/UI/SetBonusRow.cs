using TMPro;
using UnityEngine;

public class SetBonusRow : MonoBehaviour
{
    [SerializeField] private TMP_Text _setNameText;
    [SerializeField] private TMP_Text _bonus2Text;
    [SerializeField] private TMP_Text _bonus4Text;
    [SerializeField] private TMP_Text _bonus6Text;

    public void Setup(ArtifactSet set, int count)
    {
        _setNameText.text = $"{set.setName} ({count})";

        SetBonusText(_bonus2Text, set.bonus2PiecesDescription, count >= 2);
        SetBonusText(_bonus4Text, set.bonus4PiecesDescription, count >= 4);
        SetBonusText(_bonus6Text, set.bonus6PiecesDescription, count >= 6);
    }
    private void SetBonusText(TMP_Text textComponent, string description, bool isActive)
    {
        if (textComponent == null) return;

        if (isActive && !string.IsNullOrEmpty(description))
        {
            textComponent.gameObject.SetActive(true);
            textComponent.text = description;
        }
        else
        {
            textComponent.gameObject.SetActive(false);
        }
    }
}
