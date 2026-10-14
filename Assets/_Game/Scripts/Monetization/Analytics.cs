public static class Analytics
{
    public static IAnalyticsService Service { get; private set; } = new DebugAnalyticsService();
    public static void SetService(IAnalyticsService service) => Service = service;
    // Analytics.SetService(new FirebaseAnalyticsService()); firebase Init callback içinde yap
}
