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

    private Dictionary<BoosterType, BoosterSaveData> boosterData = new Dictionary<BoosterType, BoosterSaveData>();

    private void Awake()
    {
        Instance = this;
        Load();
    }

    public bool IsUnlocked(BoosterType type) => GetData(type).isLevelUnlocked;

    public void CheckLevelUnlocks(int currentLevel)
    {
        foreach (var so in allBoosters)
        {
            var data = GetData(so.boosterType);
            if (!data.isLevelUnlocked && currentLevel >= so.unlockLevel)
            {
                data.isLevelUnlocked = true;
                OnBoosterLevelUnlocked?.Invoke(so.boosterType);
            }
        }
        Save();
    }

    public BoosterSaveData GetData(BoosterType type)
    {
        if (!boosterData.ContainsKey(type))
            boosterData[type] = new BoosterSaveData { type = type, count = 0, hasBeenUnlockedOnce = false, isLevelUnlocked = false };

        return boosterData[type];
    }

    public void AddBooster(BoosterType type, int amount)
    {
        var data = GetData(type);
        bool isFirstUnlock = !data.hasBeenUnlockedOnce && data.count == 0 && amount > 0; // sadece ilk açýldýgýnda parlama efekti olacak

        data.count += amount;
        if (isFirstUnlock)
            data.hasBeenUnlockedOnce = true;

        Save();

        OnBoosterCountChanged?.Invoke(type, data.count);
        if (isFirstUnlock)
            OnBoosterFirstUnlocked?.Invoke(type);
    }

    public bool TryUseBooster(BoosterType type)
    {
        var data = GetData(type);
        if (data.count <= 0) return false;

        data.count--;
        Save();
        OnBoosterCountChanged?.Invoke(type, data.count);
        return true;
    }

    private void Save()
    {
        var wrapper = new BoosterSaveWrapper { boosters = new List<BoosterSaveData>(boosterData.Values) };
        PlayerPrefs.SetString(GameTags.Booster.BOOSTER_SAVE_DATA, JsonUtility.ToJson(wrapper));
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey(GameTags.Booster.BOOSTER_SAVE_DATA)) return;

        var wrapper = JsonUtility.FromJson<BoosterSaveWrapper>(PlayerPrefs.GetString(GameTags.Booster.BOOSTER_SAVE_DATA));
        foreach (var b in wrapper.boosters)
            boosterData[b.type] = b;
    }
}