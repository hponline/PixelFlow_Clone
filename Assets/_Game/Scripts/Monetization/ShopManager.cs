using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] List<ShopPackData> packs;
    Dictionary<string, ShopPackData> _byId;
    readonly IAPService _iap = new();
    public IReadOnlyList<ShopPackData> Packs => packs;
    public event Action OnReady;
    public event Action<bool> OnPurchasingChanged;

    public bool IsReady => _iap.IsReady;
    public bool IsPurchasing { get; private set; }

    void Awake()
    {
        Instance = this;

        _byId = packs.ToDictionary(p => p.productId);
        _iap.OnPurchaseSucceeded += HandleSucceeded;
        _iap.OnPurchaseFailed += HandleFailed;
        _iap.OnReady += HandleReady;
        _iap.Initialize(_byId.Keys);
    }

    private void OnDestroy()
    {
        _iap.OnPurchaseSucceeded -= HandleSucceeded;
        _iap.OnPurchaseFailed -= HandleFailed;
        _iap.OnReady -= HandleReady;
    }

    void HandleSucceeded(string id, decimal price, string currency, string transactionId)
    {
        if (Grant(id))
            Analytics.Service.LogPurchase(id, price, currency, transactionId);

        SetPurchasing(false);
    }

    void HandleFailed(string id, string reason)
    {
        Debug.LogWarning($"Purchase failed {id}: {reason}");
        SetPurchasing(false);
    }

    void HandleReady() => OnReady?.Invoke();

    public string GetPrice(ShopPackData pack) => _iap.GetLocalizedPrice(pack.productId);
    public void Buy(ShopPackData pack)
    {
        if (IsPurchasing || !_iap.IsReady) return;
        SetPurchasing(true);
        _iap.Buy(pack.productId);
    }

    bool Grant(string productId)
    {
        if (!_byId.TryGetValue(productId, out var pack))
        {
            Debug.LogError($"Satýn alýnan ürün katalogda yok: {productId}");
            return false;
        }

        RewardService.Instance.Grant(pack.GetRewards(), RewardSource.IAP, productId);
        // Analytics: purchase eventi buradan loglanir (Firebase "purchase").
        return true;
    }

    void SetPurchasing(bool value)
    {
        if (IsPurchasing == value) return;
        IsPurchasing = value;
        OnPurchasingChanged?.Invoke(value);
    }
}
