using System;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Grid Setup")]
    [SerializeField] private Transform _gridContainer;
    [SerializeField] private InventorySlot _slotPrefab;

    [Header("Equipped Slots")]
    [SerializeField] private InventorySlot _helmetSlot;
    [SerializeField] private InventorySlot _armorSlot;
    [SerializeField] private InventorySlot _guantletsSlot;
    [SerializeField] private InventorySlot _greavesSlot;
    [SerializeField] private InventorySlot _weaponSlot;
    [SerializeField] private InventorySlot _ringSlot;
    private InventorySlot[] _uiSlots;
    private int _maxCapacity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _maxCapacity = ArtifactInventory.Instance.maxSlots;
        _uiSlots = new InventorySlot[_maxCapacity];
        for (int i = 0; i < _maxCapacity; i++)
        {
            _uiSlots[i] = Instantiate(_slotPrefab, _gridContainer);
            _uiSlots[i].ClearSlot();
        }
    }

    public void UpdateInventoryUI()
    {
        var currentItems = ArtifactInventory.Instance.items;

        for (int i = 0; i < _maxCapacity; i++)
        {
            if (_uiSlots[i] == null) continue;

            if (i < currentItems.Count)
                _uiSlots[i].Setup(currentItems[i]);
            else
                _uiSlots[i].ClearSlot();
        }
        UpdateEquipmentUI();
    }
    private void UpdateEquipmentUI()
    {
        var equipped = ArtifactInventory.Instance.equipmentSlots;

        equipped.TryGetValue(ArtifactSlotType.Helmet, out var helmet);
        equipped.TryGetValue(ArtifactSlotType.Armor, out var armor);
        equipped.TryGetValue(ArtifactSlotType.Gauntlets, out var guantlets);
        equipped.TryGetValue(ArtifactSlotType.Greaves, out var greaves);
        equipped.TryGetValue(ArtifactSlotType.Weapon, out var weapon);
        equipped.TryGetValue(ArtifactSlotType.Ring, out var ring);

        _helmetSlot.Setup(helmet);
        _armorSlot.Setup(armor);
        _guantletsSlot.Setup(guantlets);
        _greavesSlot.Setup(greaves);
        _weaponSlot.Setup(weapon);
        _ringSlot.Setup(ring);
    }
}
