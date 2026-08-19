using UnityEngine;

[CreateAssetMenu(fileName = "NewCoinDrop", menuName = "Loot/Coin Drop Item")]
public class CoinTypeData : ScriptableObject
{
    public string coinName;
    public int baseValue = 10;
    public Sprite coinSprite;
    public Color color;
}
