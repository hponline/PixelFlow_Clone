using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] List<ShopPackData> packs;
    public IReadOnlyList<ShopPackData> Packs => packs;

    readonly IAPService _iap = new();
    Dictionary<string, ShopPackData> _byId;

    void Awake()
    {
        Instance = this;

        _byId = packs.ToDictionary(p => p.productId);
        _iap.OnPurchaseSucceeded += Grant;
        _iap.OnPurchaseFailed += (id, reason) => Debug.LogWarning($"Purchase failed {id}: {reason}");
        _iap.Initialize(_byId.Keys);
    }

    public string GetPrice(ShopPackData pack) => _iap.GetLocalizedPrice(pack.productId);
    public void Buy(ShopPackData pack) => _iap.Buy(pack.productId);

    void Grant(string productId)
    {
        if (!_byId.TryGetValue(productId, out var pack)) return;

        var wallet = CurrencyManager.Instance;
        var life = LifeManager.Instance;

        wallet.Add(CurrencyType.Coin, pack.gold);
        life.Add(pack.lives);
        //foreach (var b in pack.boosters)
            //wallet.Add(b.type, b.amount); // booster

        // Analytics: purchase eventi buradan loglanir (Firebase "purchase").
    }
}
