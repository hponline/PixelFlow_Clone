using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public event Action<int> OnGoldChanged;
    public event Action<int> OnLivesChanged;
    public event Action<BoosterType, int> OnBoosterChanged;

    const string SaveKey = "wallet_v1";

    [Serializable]
    class Data
    {
        public int gold;
        public int lives = 5;
        public int[] boosters = new int[Enum.GetValues(typeof(BoosterType)).Length];
    }

    Data _d;

    public int Gold => _d.gold;
    public int Lives => _d.lives;
    public int GetBooster(BoosterType t) => _d.boosters[(int)t];

    void Awake()
    {
        Instance = this;

        Load();
    }

    // ---- Gold ----
    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        _d.gold += amount;
        Save();
        OnGoldChanged?.Invoke(_d.gold);
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0 || _d.gold < amount) return false;
        _d.gold -= amount;
        Save();
        OnGoldChanged?.Invoke(_d.gold);
        return true;
    }

    // ---- Lives ----
    public void AddLives(int amount)
    {
        if (amount <= 0) return;
        _d.lives += amount;
        Save();
        OnLivesChanged?.Invoke(_d.lives);
    }

    public bool TryConsumeLife()
    {
        if (_d.lives <= 0) return false;
        _d.lives--;
        Save();
        OnLivesChanged?.Invoke(_d.lives);
        return true;
    }

    // ---- Boosters ----
    public void AddBooster(BoosterType t, int amount)
    {
        if (amount <= 0) return;
        _d.boosters[(int)t] += amount;
        Save();
        OnBoosterChanged?.Invoke(t, _d.boosters[(int)t]);
    }

    public bool TryConsumeBooster(BoosterType t)
    {
        if (_d.boosters[(int)t] <= 0) return false;
        _d.boosters[(int)t]--;
        Save();
        OnBoosterChanged?.Invoke(t, _d.boosters[(int)t]);
        return true;
    }

    // ---- Persistence ----
    // Prototip icin PlayerPrefs. Sonra ISaveService arkasina tasi (dosya / cloud save).
    void Load()
    {
        var json = PlayerPrefs.GetString(SaveKey, null);
        _d = string.IsNullOrEmpty(json) ? new Data() : JsonUtility.FromJson<Data>(json);
    }

    void Save()
    {
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(_d));
        PlayerPrefs.Save();
    }
}
