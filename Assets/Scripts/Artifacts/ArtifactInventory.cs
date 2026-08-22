using System.Collections.Generic;
using UnityEngine;

public class ArtifactInventory : MonoBehaviour
{
    public static ArtifactInventory Instance;

    public int maxSlots = 30;
    [SerializeField] private GameObject _playerGameObject;

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
    }
    public bool AddArtifact(RuntimeArtifact artifact)
    {
        if (items.Count >= maxSlots) return false;
        items.Add(artifact);
        InventoryUI.Instance?.UpdateInventoryUI();
        return true;
    }
    public void EquipArtifact(RuntimeArtifact artifact)
    {
        ArtifactSlotType targetSlot = artifact.data.slotType;

        if (equipmentSlots[targetSlot] != null)
        {
            UnequipArtifact(targetSlot);
        }

        equipmentSlots[targetSlot] = artifact;
        items.Remove(artifact);

        RecalculateSets();
        InventoryUI.Instance?.UpdateInventoryUI();
    }
    public void UnequipArtifact(ArtifactSlotType slot)
    {
        RuntimeArtifact artifactToRemove = equipmentSlots[slot];
        if (artifactToRemove != null && items.Count < maxSlots)
        {
            items.Add(artifactToRemove);
            equipmentSlots[slot] = null;

            RecalculateSets();
            InventoryUI.Instance?.UpdateInventoryUI();
        }
    }
    private void RecalculateSets()
    {
        _setCounts.Clear();
        foreach (var kvp in equipmentSlots)
        {
            if (kvp.Value?.artifactSet == null) continue;

            if (_setCounts.ContainsKey(kvp.Value.artifactSet))
                _setCounts[kvp.Value.artifactSet]++;
            else
                _setCounts.Add(kvp.Value.artifactSet, 1);
        }
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
}
