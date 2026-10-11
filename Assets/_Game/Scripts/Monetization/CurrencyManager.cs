using NaughtyAttributes;
using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public event Action<CurrencyType, int> OnCurrencyChanged;

    const string SaveKey = "currencySaveData";

    [Serializable]
    class Data
    {
        public int[] balances = new int[Enum.GetValues(typeof(CurrencyType)).Length];
    }

    Data _data;
    ISaveService _save;


    void Awake()
    {
        Instance = this;
        _save = new PlayerPrefsSaveService();

        Load();
    }

    public int Get(CurrencyType type) => _data.balances[(int)type];
    public bool Has(CurrencyType type, int amount) => Get(type) >= amount;

    public void Add(CurrencyType type, int amount)
    {
        if (amount <= 0) return;
        _data.balances[(int)type] += amount;
        Commit(type);
    }

    public bool TrySpend(CurrencyType type, int amount)
    {
        if (amount <= 0 || !Has(type, amount)) return false;
        _data.balances[(int)type] -= amount;
        Commit(type);
        return true;
    }

    void Commit(CurrencyType type)
    {
        _save.Save(SaveKey, _data);
        OnCurrencyChanged?.Invoke(type, Get(type));
    }

    //// ---- Gold ----
    //public void AddGold(int amount)
    //{
    //    if (amount <= 0) return;
    //    _data.gold += amount;
    //    Save();
    //    OnGoldChanged?.Invoke(_data.gold);
    //}

    //public bool TrySpendGold(int amount)
    //{
    //    if (amount <= 0 || _data.gold < amount) return false;
    //    _data.gold -= amount;
    //    Save();
    //    OnGoldChanged?.Invoke(_data.gold);
    //    return true;
    //}

    //// ---- Lives ----
    //public void AddLives(int amount)
    //{
    //    if (amount <= 0) return;
    //    _data.lives += amount;
    //    Save();
    //    OnLivesChanged?.Invoke(_data.lives);
    //}

    //public bool TryConsumeLife()
    //{
    //    if (_data.lives <= 0) return false;
    //    _data.lives--;
    //    Save();
    //    OnLivesChanged?.Invoke(_data.lives);
    //    return true;
    //}

    //// ---- Boosters ----
    //public void AddBooster(BoosterType t, int amount)
    //{
    //    if (amount <= 0) return;
    //    _data.boosters[(int)t] += amount;
    //    Save();
    //    OnBoosterChanged?.Invoke(t, _data.boosters[(int)t]);
    //}

    //public bool TryConsumeBooster(BoosterType t)
    //{
    //    if (_data.boosters[(int)t] <= 0) return false;
    //    _data.boosters[(int)t]--;
    //    Save();
    //    OnBoosterChanged?.Invoke(t, _data.boosters[(int)t]);
    //    return true;
    //}

    void Load()
    {
        _data = _save.Load<Data>(SaveKey);
        
        int count = Enum.GetValues(typeof(CurrencyType)).Length;
        if (_data.balances == null)
            _data.balances = new int[count];
        else if (_data.balances.Length != count)
            Array.Resize(ref _data.balances, count); // Yeni eleman eklenince bozulmamasý için

        var json = PlayerPrefs.GetString(SaveKey, null);
        _data = string.IsNullOrEmpty(json) ? new Data() : JsonUtility.FromJson<Data>(json);
    }

    [Button]
    void DebugAddCoin() => Add(CurrencyType.Coin, 5000);
}
