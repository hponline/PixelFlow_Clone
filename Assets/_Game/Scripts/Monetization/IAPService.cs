using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

// Unity IAP 4.x (IDetailedStoreListener). Paket surumun 5.x ise StoreController API'si
// farkli; sadece bu sinifi degistirmen yeterli, geri kalan kod etkilenmez.
public class IAPService : IDetailedStoreListener
{
    IStoreController _controller;

    public bool IsReady => _controller != null;
    public event Action<string> OnPurchaseSucceeded; // productId
    public event Action<string, string> OnPurchaseFailed; // productId, reason

    public void Initialize(IEnumerable<string> consumableProductIds)
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var id in consumableProductIds)
            builder.AddProduct(id, ProductType.Consumable);
        UnityPurchasing.Initialize(this, builder);
    }

    public void Buy(string productId)
    {
        if (!IsReady) { OnPurchaseFailed?.Invoke(productId, "store_not_ready"); return; }
        _controller.InitiatePurchase(productId);
    }

    // Fiyati magazadan al. TRY degerini koda yazma: bolgeye gore degisir.
    public string GetLocalizedPrice(string productId)
    {
        var p = _controller?.products.WithID(productId);
        return p != null ? p.metadata.localizedPriceString : "--";
    }

    // ---- IStoreListener ----
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        => _controller = controller;

    public void OnInitializeFailed(InitializationFailureReason error)
        => Debug.LogError($"IAP init failed: {error}");

    public void OnInitializeFailed(InitializationFailureReason error, string message)
        => Debug.LogError($"IAP init failed: {error} {message}");

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // Odul verme senkron ve lokal oldugu icin Complete donmek guvenli.
        // Sunucu dogrulamasi eklersen Pending don, dogrulayinca ConfirmPendingPurchase cagir.
        OnPurchaseSucceeded?.Invoke(args.purchasedProduct.definition.id);
        return PurchaseProcessingResult.Complete;
    }

    void IDetailedStoreListener.OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError("Satýn alma baþarýsýz");
        throw new NotImplementedException();
    }

    void IStoreListener.OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("Satýn alma baþarýsýz");
        throw new NotImplementedException();
    }

    //public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    //    => OnPurchaseFailed?.Invoke(product.definition.id, reason.ToString());

    //public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
    //    => OnPurchaseFailed?.Invoke(product.definition.id, description.reason.ToString());
}
