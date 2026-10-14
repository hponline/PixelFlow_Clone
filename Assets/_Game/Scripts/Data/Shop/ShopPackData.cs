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

    public IReadOnlyList<RewardItem> GetRewards()
    {
        var list = new List<RewardItem>();
        if (gold > 0)
            list.Add(new RewardItem { type = RewardType.Currency, currency = CurrencyType.Coin, amount = gold });
        if (lives > 0)
            list.Add(new RewardItem { type = RewardType.Life, amount = lives });
        foreach (var booster in boosters)
            list.Add(new RewardItem { type = RewardType.Booster, booster = booster.type, amount = booster.amount });
        return list;
    }
}