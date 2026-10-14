using UnityEngine;

public class DebugAnalyticsService : IAnalyticsService
{
    public void LogResourceEarn(string resource, int amount, string source, string sourceId)
        => Debug.Log($"[Analytics] earn: {resource}  price: x{amount} src={source}/{sourceId}");

    public void LogResourceSpend(string resource, int amount, string sink, string sinkId)
        => Debug.Log($"[Analytics] spend: {resource}  price: x{amount} sink={sink}/{sinkId}");

    public void LogPurchase(string productId, decimal price, string currency, string transactionId)
        => Debug.Log($"[Analytics] purchase: {productId} price:  {price} Currency Conversion: {currency} tx={transactionId}");
}