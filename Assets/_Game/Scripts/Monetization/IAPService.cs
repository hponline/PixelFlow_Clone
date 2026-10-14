using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

// Unity IAP 4.x (IDetailedStoreListener). Paket surumun 5.x ise StoreController API'si
public class IAPService : IDetailedStoreListener
{
    IStoreController _controller;

    public bool IsReady => _controller != null;
    public event Action<string, decimal, string, string> OnPurchaseSucceeded; // productId
    public event Action<string, string> OnPurchaseFailed; // productId, reason
    public event Action OnReady;

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
    {
        _controller = controller;
        OnReady?.Invoke();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
        => Debug.LogError($"IAP init failed: {error}");

    public void OnInitializeFailed(InitializationFailureReason error, string message)
        => Debug.LogError($"IAP init failed: {error} {message}");

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var p = args.purchasedProduct;
        OnPurchaseSucceeded?.Invoke(
            p.definition.id,
            p.metadata.localizedPrice,
            p.metadata.isoCurrencyCode,
            p.transactionID);

        return PurchaseProcessingResult.Complete;
    }

    void IDetailedStoreListener.OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        OnPurchaseFailed?.Invoke(product?.definition.id, failureDescription.reason.ToString());

        Debug.Log("Satýn alma baþarýsýz1");
    }

    void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        OnPurchaseFailed?.Invoke(product?.definition.id, failureReason.ToString());
        Debug.Log("Satýn alma baþarýsýz2");
    }
}
