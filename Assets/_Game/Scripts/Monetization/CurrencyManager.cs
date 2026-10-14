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

    void Load()
    {
        _data = _save.Load<Data>(SaveKey) ?? new Data();
        
        int count = Enum.GetValues(typeof(CurrencyType)).Length;
        if (_data.balances == null)
            _data.balances = new int[count];
        else if (_data.balances.Length != count)
            Array.Resize(ref _data.balances, count); // Yeni eleman eklenince bozulmamasý için
    }

    [Button]
    void DebugAddCoin() => Add(CurrencyType.Coin, 5000);
}
