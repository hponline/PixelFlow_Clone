using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BoosterReward
{
    public BoosterType type;
    public int amount;
}

[CreateAssetMenu(menuName = "Shop/Pack")]
public class ShopPackData : ScriptableObject
{
    [Tooltip("Play Console'daki Product ID ile birebir ayni. Ornek: pack_small")]
    public string productId;
    public string displayName;
    public string badge; // "BEST VALUE", "POPULAR" veya bos

    [Header("Icerik")]
    public int gold;
    public int lives;
    public List<BoosterReward> boosters = new();
}
