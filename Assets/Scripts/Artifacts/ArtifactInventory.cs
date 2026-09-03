using System.Collections.Generic;
using UnityEngine;

public class ArtifactInventory : MonoBehaviour
{
    public static ArtifactInventory Instance;

    public int TotalArtifactsCollected { get; private set; } = 0;

    public int maxSlots = 30;
    [SerializeField] private GameObject _playerGameObject;
    private PlayerStats _stats;

    public List<RuntimeArtifact> items = new List<RuntimeArtifact>();
    public Dictionary<ArtifactSlotType, RuntimeArtifact> equipmentSlots = new Dictionary<ArtifactSlotType, RuntimeArtifact>();
    private Dictionary<ArtifactSet, int> _setCounts = new Dictionary<ArtifactSet, int>();
    private void Awake()
    {
        Instance = this;
        foreach (ArtifactSlotType slot in System.Enum.GetValues(typeof(ArtifactSlotType)))
        {
            equipmentSlots.Add(slot, null);
        }
        _stats = _playerGameObject.GetComponent<PlayerStats>();
    }
    public bool AddArtifact(RuntimeArtifact artifact)
    {
        if (items.Count >= maxSlots) return false;
        items.Add(artifact);
        TotalArtifactsCollected++;
        InventoryUI.Instance?.UpdateInventoryUI();
        return true;
    }
    public void EquipArtifact(RuntimeArtifact artifact)
    {
        ArtifactSlotType targetSlot = artifact.data.slotType;
        items.Remove(artifact);

        if (equipmentSlots[targetSlot] != null)
        {
            UnequipArtifact(targetSlot);
        }

        equipmentSlots[targetSlot] = artifact;

        _stats.RecalculateArtifactStats();
        RecalculateSets();
        _stats.NotifyStatsChanged();

        InventoryUI.Instance?.UpdateInventoryUI();
    }
    public void UnequipArtifact(ArtifactSlotType slot)
    {
        RuntimeArtifact artifactToRemove = equipmentSlots[slot];
        if (artifactToRemove != null && items.Count < maxSlots)
        {
            items.Add(artifactToRemove);
            equipmentSlots[slot] = null;

            _stats.RecalculateArtifactStats();
            RecalculateSets();
            _stats.NotifyStatsChanged();

            InventoryUI.Instance?.UpdateInventoryUI();
        }
    }
    private void RecalculateSets()
    {
        foreach (var kvp in _setCounts)
        {
            ArtifactSet set = kvp.Key;
            int count = kvp.Value;

            if (count >= 2) set.Remove2PiecesBonus(_playerGameObject);
            if (count >= 4) set.Remove4PiecesBonus(_playerGameObject);
            if (count >= 6) set.Remove6PiecesBonus(_playerGameObject);
        }

        _setCounts.Clear();
        foreach (var kvp in equipmentSlots)
        {
            if (kvp.Value?.artifactSet == null) continue;

            if (_setCounts.ContainsKey(kvp.Value.artifactSet))
                _setCounts[kvp.Value.artifactSet]++;
            else
                _setCounts.Add(kvp.Value.artifactSet, 1);
        }

        foreach (var kvp in _setCounts)
        {
            ArtifactSet set = kvp.Key;
            int count = kvp.Value;

            if (count >= 2) set.Apply2PiecesBonus(_playerGameObject);
            if (count >= 4) set.Apply4PiecesBonus(_playerGameObject);
            if (count >= 6) set.Apply6PiecesBonus(_playerGameObject);
        }
        InventoryUI.Instance.RefreshPanel(_setCounts);
    }

    public int GetEquippedCount(ArtifactSet set)
    {
        if (set == null) return 0;

        if (_setCounts.TryGetValue(set, out int count))
        {
            return count;
        }

        return 0;
    }
    public void RemoveArtifact(RuntimeArtifact artifact)
    {
        if (artifact == null) return;

        if (IsEquipped(artifact))
        {
            UnequipArtifact(artifact.data.slotType);
        }

        if (items.Contains(artifact))
        {
            items.Remove(artifact);
        }

        InventoryUI.Instance?.UpdateInventoryUI();
        RecalculateSets();
    }
    private bool IsEquipped(RuntimeArtifact artifact)
    {
        foreach (var kvp in equipmentSlots)
        {
            if (kvp.Value == artifact) return true;
        }
        return false;
    }
}
