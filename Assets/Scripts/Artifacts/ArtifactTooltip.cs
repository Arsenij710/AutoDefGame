using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactTooltip : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button _actionButton;
    [SerializeField] private TMP_Text _actionButtonText;
    public static ArtifactTooltip Instance { get; private set; }

    [SerializeField] private RectTransform _panel;
    [Header("ArtStats")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _artifactNameText;
    [SerializeField] private TextMeshProUGUI _elementText;
    [SerializeField] private TextMeshProUGUI _mainStatsText;
    [SerializeField] private TextMeshProUGUI _mainStatsValue;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private Image _rarity;
    [SerializeField] private TextMeshProUGUI _subStatsText;
    [SerializeField] private TextMeshProUGUI _artifactSetText;
    [SerializeField] private TextMeshProUGUI _setBonusText2;
    [SerializeField] private TextMeshProUGUI _setBonusText4;
    [SerializeField] private TextMeshProUGUI _setBonusText6;
    [SerializeField] private Image _setActiveImage2;
    [SerializeField] private Image _setActiveImage4;
    [SerializeField] private Image _setActiveImage6;
    [SerializeField] private Sprite _ActiveSet;
    [SerializeField] private Sprite _NotActiveSet;


    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private Vector2 _offset = new Vector2(2f, 1f);
    [SerializeField] private Vector2 _padding = new Vector2(10f, 10f);
    private bool _isPinned = false;
    private bool _justPinned;
    private bool _isEquippedState;
    public bool IsPinned => _isPinned;
    private bool _waitingForMouseMovement;
    private Vector3 _startMousePosition;
    private RuntimeArtifact _currentArtifact;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        HideTooltip();
        _actionButton.onClick.RemoveAllListeners();
        _actionButton.onClick.AddListener(OnEquipButtonClick);
    }

    private void Update()
    {
        if (_waitingForMouseMovement)
        {
            if (Vector3.Distance(Input.mousePosition, _startMousePosition) > 2f)
            {
                _waitingForMouseMovement = false;
            }
        }

        if (!_isPinned) return;

        if (_justPinned)
        {
            _justPinned = false;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideTooltip();
            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(_panel, Input.mousePosition, _mainCanvas.worldCamera))
            {
                HideTooltip();
            }
        }
    }
    public void ShowPreview(RuntimeArtifact artifact, RectTransform slotRect, bool isEquipped)
    {
        if (_waitingForMouseMovement || _isPinned) return;
        if (slotRect == null || !slotRect.gameObject.activeInHierarchy) return;

        _currentArtifact = artifact;
        _isEquippedState = isEquipped;

        _panel.gameObject.SetActive(true);

        UpdateUI(_currentArtifact);
        UpdatePosition(slotRect);
    }
    public void PinTooltip(RuntimeArtifact artifact, RectTransform slotRect,bool isEquipped)
    {
        _currentArtifact = artifact;
        _isPinned = true;
        _justPinned = true;
        _isEquippedState = isEquipped;

        _panel.gameObject.SetActive(true);

        UpdateUI(_currentArtifact);
        UpdatePosition(slotRect);
    }
    public void HideTooltip()
    {
        _isPinned = false;
        _currentArtifact = null;
        _panel.gameObject.SetActive(false);
    }
    
    private void UpdatePosition(RectTransform slotRect)
    {
        Vector3[] slotCorners = new Vector3[4];
        slotRect.GetWorldCorners(slotCorners);

        Camera uiCamera = _mainCanvas.worldCamera;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, slotCorners[2]);
        RectTransform canvasRect = _mainCanvas.transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, uiCamera, out Vector2 localPoint))
        {
            _panel.anchoredPosition = localPoint + _offset;
        }

        Canvas.ForceUpdateCanvases();
        ClampToScreen();
    }
    private void ClampToScreen()
    {
        Vector3[] corners = new Vector3[4];
        _panel.GetWorldCorners(corners);

        Camera uiCamera = _mainCanvas.worldCamera;
        Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[0]);
        Vector2 topRight = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[2]);

        Vector2 shift = Vector2.zero;

        if (topRight.x > Screen.width - _padding.x)
            shift.x -= (topRight.x - (Screen.width - _padding.x));

        if (bottomLeft.x < _padding.x)
            shift.x += (_padding.x - bottomLeft.x);

        if (bottomLeft.y < _padding.y)
            shift.y += (_padding.y - bottomLeft.y);

        if (topRight.y > Screen.height - _padding.y)
            shift.y -= (topRight.y - (Screen.height - _padding.y));

        _panel.anchoredPosition += shift / _mainCanvas.scaleFactor;
    }

    public void OnEquipButtonClick()
    {
        if (_currentArtifact == null) return;

        if (_isEquippedState)
        {
            ArtifactInventory.Instance.UnequipArtifact(_currentArtifact.data.slotType);
        }
        else
        {
            ArtifactInventory.Instance.EquipArtifact(_currentArtifact);
        }

        HideTooltip();
    }
    public void UpdateUI(RuntimeArtifact artifact)
    {
        if (_actionButtonText != null)
        {
            _actionButtonText.text = _isEquippedState ? "Снять" : "Надеть";
        }

        _artifactNameText.text = artifact.data.artifactName;
        _rarity.color = StatUtils.GetRarityColor(artifact.rarity);
        Debug.Log(artifact.rarity);
        Debug.Log(_rarity.color.r);
        _elementText.text = StatUtils.GetElementName(artifact.data.slotType);
        _mainStatsText.text = $"{StatUtils.GetStatName(artifact.mainStat.type)}";
        _mainStatsValue.text = $"{StatUtils.FormatStatForUI(artifact.mainStat)}";
        _icon.sprite = artifact.data.icon;
        _level.text = $"Уровень {artifact.level}";

        List<ArtifactStat> sbStats = artifact.subStats;
        string resSubStats = "";
        for (int i = 0; i < sbStats.Count; i++) 
        {
            string calcValue = StatUtils.FormatStatForUI(sbStats[i]);
            resSubStats += $"* {StatUtils.GetStatName(sbStats[i].type)} + {calcValue}\n";
        }
        _subStatsText.text = resSubStats;
        _artifactSetText.text = $"{artifact.artifactSet.setName}";

        int currentCount = ArtifactInventory.Instance.GetEquippedCount(artifact.artifactSet);
        _setActiveImage2.sprite = currentCount >= 2 ? _ActiveSet : _NotActiveSet;
        _setActiveImage4.sprite = currentCount >= 4 ? _ActiveSet : _NotActiveSet;
        _setActiveImage6.sprite = currentCount >= 6 ? _ActiveSet : _NotActiveSet;

        _setBonusText2.text = artifact.artifactSet.bonus2PiecesDescription;
        _setBonusText4.text = artifact.artifactSet.bonus4PiecesDescription;
        _setBonusText6.text = artifact.artifactSet.bonus6PiecesDescription;

    }
    public void OnInventoryOpened()
    {
        HideTooltip();
        _waitingForMouseMovement = true;
        _startMousePosition = Input.mousePosition;
    }
}
