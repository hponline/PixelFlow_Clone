public interface IAnalyticsService
{
    void LogResourceEarn(string resource, int amount, string source, string sourceId);
    void LogResourceSpend(string resource, int amount, string sink, string sinkId);
    void LogPurchase(string productId, decimal price, string currency, string transactionId);
}