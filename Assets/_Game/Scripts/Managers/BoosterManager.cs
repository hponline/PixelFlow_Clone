using System.Collections.Generic;
using System;
using UnityEngine;


public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance { get; private set; }

    [SerializeField] List<BoosterSO> allBoosters;

    public event Action<BoosterType, int> OnBoosterCountChanged;
    public event Action<BoosterType> OnBoosterFirstUnlocked;
    public event Action<BoosterType> OnBoosterLevelUnlocked;

    readonly Dictionary<BoosterType, BoosterSaveData> boosterData = new Dictionary<BoosterType, BoosterSaveData>();
    ISaveService _save;

    [Header("PlayerPrefs")]
    const string KEYS_BOOSTERS = "boosters";

    private void Awake()
    {
        Instance = this;

        _save = new PlayerPrefsSaveService();
        Load();
    }

    public int GetCount(BoosterType type) => boosterData.TryGetValue(type, out var data) ? data.count : 0;
    public bool IsUnlocked(BoosterType type) => boosterData.TryGetValue(type, out var data) && data.isLevelUnlocked;
    public bool Has(BoosterType type) => GetCount(type) > 0;


    public void CheckLevelUnlocks(int currentLevel)
    {
        bool changed = false;
        foreach (var so in allBoosters)
        {
            var data = GetOrCreate(so.boosterType);
            if (data.isLevelUnlocked || currentLevel < so.unlockLevel) continue;

            data.isLevelUnlocked = true;
            if (data.count <= 0)
                data.count = so.startCount;

            changed = true;
            OnBoosterCountChanged?.Invoke(so.boosterType, data.count);
            OnBoosterLevelUnlocked?.Invoke(so.boosterType);
            Debug.LogWarning("Booster Unlock: " + so.boosterType.ToString());
        }

        if (changed)
            Save();
    }

    public void AddBooster(BoosterType type, int amount)
    {
        var data = GetOrCreate(type);
        bool isFirstUnlock = !data.hasBeenUnlockedOnce; // sadece ilk açýldýgýnda parlama efekti olacak

        data.count += amount;
        data.hasBeenUnlockedOnce = true;

        OnBoosterCountChanged?.Invoke(type, data.count);
        Save();
        if (isFirstUnlock)
            OnBoosterFirstUnlocked?.Invoke(type);
    }

    public bool TryUseBooster(BoosterType type)
    {
        if (!boosterData.TryGetValue(type, out var data) || data.count <= 0) return false;

        data.count--;
        Save();
        OnBoosterCountChanged?.Invoke(type, data.count);
        return true;
    }

    BoosterSaveData GetOrCreate(BoosterType type)
    {
        if (!boosterData.TryGetValue(type, out var data))
            boosterData[type] = data = new BoosterSaveData { type = type };
        return data;
    }

    void Save()
    {
        _save.Save(KEYS_BOOSTERS, new BoosterSaveWrapper
        {
            boosters = new List<BoosterSaveData>(boosterData.Values)
        });
    }

    void Load()
    {
        var wrapper = _save.Load<BoosterSaveWrapper>(KEYS_BOOSTERS);
        if (wrapper.boosters == null || wrapper.boosters.Count == 0) return;

        foreach (var b in wrapper.boosters)
            boosterData[b.type] = b;
    }
}