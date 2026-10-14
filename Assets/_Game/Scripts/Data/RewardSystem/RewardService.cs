using System.Collections.Generic;
using System;
using UnityEngine;

public class RewardService : MonoBehaviour, IAnalyticsService
{
    public static RewardService Instance { get; private set; }

    public event Action<IReadOnlyList<RewardItem>, RewardSource> OnRewardGranted;

    void Awake() => Instance = this;

    public void Grant(IReadOnlyList<RewardItem> items, RewardSource source, string sourceId)
    {
        foreach (var item in items)
        {
            if (item.amount <= 0) continue;

            switch (item.type)
            {
                case RewardType.Currency:
                    CurrencyManager.Instance.Add(item.currency, item.amount);
                    break;
                case RewardType.Life:
                    LifeManager.Instance.Add(item.amount);
                    break;
                case RewardType.Booster:
                    BoosterManager.Instance.AddBooster(item.booster, item.amount);
                    break;
            }

            var name = item.type switch
            {
                RewardType.Currency => item.currency.ToString(),
                RewardType.Booster => item.booster.ToString(),
                _ => "Life"
            };
            // Analytics: resource earn eventi (item, amount, source, sourceId)
            LogResourceEarn(name, item.amount, source.ToString(), sourceId);

        }
        OnRewardGranted?.Invoke(items, source);
    }

    public void LogResourceEarn(string resource, int amount, string source, string sourceId)
    {
        Debug.Log($"[Analytics] earn {resource} x{amount} src={source}/{sourceId}");
    }

    public void LogResourceSpend(string resource, int amount, string sink, string sinkId)
    {
        Debug.Log($"[Analytics] spend {resource} x{amount} sink={sink}/{sinkId}");
    }

    public void LogPurchase(string productId, decimal price, string currency, string transactionId)
    {
        Debug.Log($"[Analytics] purchase {productId} {price} {currency} tx={transactionId}");
    }
}