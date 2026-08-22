using System.Collections.Generic;
using UnityEngine;

public enum ArtifactSlotType
{
    Helmet,
    Armor,
    Gauntlets,
    Greaves,
    Weapon,
    Ring
}
public enum ArtifactRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythical
}

[System.Serializable]
public struct ArtifactData
{
    public string artifactName;
    public Sprite icon;
    public ArtifactSlotType slotType;
}
public abstract class ArtifactSet : ScriptableObject
{
    [Header("Settings")]
    public string setName;

    [Header("Artifacts")]
    public List<ArtifactData> setArtifacts = new List<ArtifactData>();

    [Header("Bonuses")]
    [TextArea] public string bonus2PiecesDescription;
    [TextArea] public string bonus4PiecesDescription;
    [TextArea] public string bonus6PiecesDescription;

    public abstract void Apply2PiecesBonus(GameObject player);
    public abstract void Apply4PiecesBonus(GameObject player);
    public abstract void Apply6PiecesBonus(GameObject player);
    public abstract void Remove2PiecesBonus(GameObject player);
    public abstract void Remove4PiecesBonus(GameObject player);
    public abstract void Remove6PiecesBonus(GameObject player);
}
