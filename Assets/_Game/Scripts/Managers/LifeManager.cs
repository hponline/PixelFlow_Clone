using UnityEngine;
using System;
using NaughtyAttributes;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; }

    [SerializeField] int MaxLife = 5;
    [SerializeField, Tooltip("Saniye cinsinden")]
    double RegenDurationSeconds = 600.0; // 10 dakika

    [SerializeField] int currentLife;
    private DateTime nextLifeTime;
    private DateTime _unlimitedLivesExpiry;
    bool _wasUnlimited;

    public int CurrentLife => currentLife;
    public DateTime NextLifeTime => nextLifeTime;
    public DateTime UnlimitedLivesExpiry => _unlimitedLivesExpiry;
    public bool IsFull => currentLife >= MaxLife;

    public bool HasEnoughLife() => currentLife > 0 || IsUnLimitedLives;
    public bool IsUnLimitedLives => DateTime.UtcNow < _unlimitedLivesExpiry;


    public event Action<int> OnLifeChanged;
    public event Action<DateTime> OnUnLimitedLivesActivited;
    public event Action OnUnlimitedLivesExpired;


    void Awake()
    {
        Instance = this;

        LoadData();
        ApplyOfflineUnlimitedExpiry();
        ApplyOfflineRegen();
    }

    void Start()
    {
        _wasUnlimited = IsUnLimitedLives;

        GameEvent.OnLevelRestart += OnConsumeLife;
    }

    void OnDisable()
    {
        GameEvent.OnLevelRestart -= OnConsumeLife;
    }

    void Update()
    {
        bool isUnlimited = IsUnLimitedLives;

        if (_wasUnlimited && !isUnlimited)
        {
            currentLife = 0;
            nextLifeTime = DateTime.UtcNow.AddSeconds(RegenDurationSeconds);
            _unlimitedLivesExpiry = DateTime.MinValue;
            SaveData();
            OnLifeChanged?.Invoke(currentLife);
            OnUnlimitedLivesExpired?.Invoke();
        }

        _wasUnlimited = isUnlimited;

        if (isUnlimited) return;
        if (currentLife >= MaxLife) return;

        if (DateTime.UtcNow >= nextLifeTime)
        {
            currentLife++;

            // Can hâlâ dolmadıysa bir sonraki periyodu başlat
            if (currentLife < MaxLife)
                nextLifeTime = nextLifeTime.AddSeconds(RegenDurationSeconds);

            SaveData();
            OnLifeChanged?.Invoke(currentLife);
        }
    }

    [Button]
    public void TestAddCurrentLife()
    {
        BuyLife(1);
    }

    // ── Sınırsız Can ──────────────────────────────────────────────────────────
    /// <summary>
    /// Aktifken üstüne alınırsa süre sıfırlanmaz, mevcut expiry'nin üstüne eklenir.
    /// </summary>
    public void ActivateUnlimitedLives(double durationSeconds)
    {
        DateTime baseTime = IsUnLimitedLives ? _unlimitedLivesExpiry : DateTime.UtcNow;
        _unlimitedLivesExpiry = baseTime.AddSeconds(durationSeconds);
        _wasUnlimited = true;
        SaveData();
        OnUnLimitedLivesActivited?.Invoke(_unlimitedLivesExpiry);

    }

    public void BuyLife(int amount)
    {
        currentLife = Mathf.Min(currentLife + amount, 99);

        OnLifeChanged?.Invoke(currentLife);
        SaveData();
    }

    // ── Offline Regen (Oyun Kapalıyken) ───────────────────────────────────────

    /// <summary>
    /// Uygulama başlarken kaydedilmiş nextLifeTime ile UtcNow karşılaştırılır;
    /// geçen her 10 dakika için bir can eklenir (max 5 ile sınırlı).
    /// </summary>
    /// 
    private void ApplyOfflineUnlimitedExpiry()
    {
        if (_unlimitedLivesExpiry == DateTime.MinValue) return;
        if (IsUnLimitedLives) return;

        currentLife = 0;
        nextLifeTime = DateTime.UtcNow.AddSeconds(RegenDurationSeconds);
        _unlimitedLivesExpiry = DateTime.MinValue;
    }

    private void ApplyOfflineRegen()
    {
        if (currentLife >= MaxLife) return;

        DateTime now = DateTime.UtcNow;
        TimeSpan elapsed = now - nextLifeTime;

        if (elapsed.TotalSeconds < 0) return;

        int periodsElapsed = (int)(elapsed.TotalSeconds / RegenDurationSeconds) + 1;
        int livesToAdd = Mathf.Min(periodsElapsed, MaxLife - currentLife);

        currentLife += livesToAdd;
        if (currentLife < MaxLife)
            nextLifeTime = nextLifeTime.AddSeconds(livesToAdd * RegenDurationSeconds);

        SaveData();
        OnLifeChanged?.Invoke(currentLife);
    }

    private void OnConsumeLife()
    {
        if (IsUnLimitedLives) return;
        if (currentLife <= 0) return;

        bool wasAtMax = (currentLife >= MaxLife);
        currentLife--;

        if (wasAtMax && currentLife < MaxLife)
            nextLifeTime = DateTime.UtcNow.AddSeconds(RegenDurationSeconds);

        SaveData();
        OnLifeChanged?.Invoke(currentLife);
    }

    public void ResetLife()
    {
        currentLife = MaxLife;
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(GameTags.PlayerPrefsKeys.CURRENT_LIFE, currentLife);
        PlayerPrefs.SetString(GameTags.PlayerPrefsKeys.NEXT_LIFE_TIME, nextLifeTime.ToBinary().ToString());
        PlayerPrefs.SetString(GameTags.PlayerPrefsKeys.UNLIMITED_LIVES_EXPIRY, _unlimitedLivesExpiry.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        currentLife = PlayerPrefs.GetInt(GameTags.PlayerPrefsKeys.CURRENT_LIFE, MaxLife);

        if (PlayerPrefs.HasKey(GameTags.PlayerPrefsKeys.NEXT_LIFE_TIME) &&
            long.TryParse(PlayerPrefs.GetString(GameTags.PlayerPrefsKeys.NEXT_LIFE_TIME), out long binary))
        {
            nextLifeTime = DateTime.FromBinary(binary);
        }
        else
        {
            // İlk kez açılıyor: can dolu, sayaç anlamsız ama geçerli bir değer ver
            nextLifeTime = DateTime.UtcNow.AddSeconds(RegenDurationSeconds);
        }

        if (PlayerPrefs.HasKey(GameTags.PlayerPrefsKeys.UNLIMITED_LIVES_EXPIRY) &&
            long.TryParse(PlayerPrefs.GetString(GameTags.PlayerPrefsKeys.UNLIMITED_LIVES_EXPIRY), out long unlimitedBinary))
        {
            _unlimitedLivesExpiry = DateTime.FromBinary(unlimitedBinary);
        }
        else
        {
            _unlimitedLivesExpiry = DateTime.MinValue;
        }

        OnLifeChanged?.Invoke(currentLife);
    }
}