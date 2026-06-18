using UnityEngine;
using System;
using NaughtyAttributes;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public event Action<int> OnCoinChanged;
    public event Action OnCoinsSpent;

    public bool HasEnoughCoins(float amount) => DataManager.Instance.currentCoin >= amount;

    public int CurrentCoins => DataManager.Instance.currentCoin;

    public 

    void Awake()
    {
        Instance = this;
    }

    public void AddCoins(int amount)
    {
        DataManager.Instance.currentCoin += amount;
        DataManager.Instance.SaveGame();
        OnCoinChanged?.Invoke(CurrentCoins);
    }

    [Button]
    public void TestAddCoin()
    {
        DataManager.Instance.currentCoin += 5000;
        DataManager.Instance.SaveGame();
        OnCoinChanged?.Invoke(CurrentCoins);
    }


    public bool SpendCoins(int amount)
    {
        if (!HasEnoughCoins(amount)) return false;

        DataManager.Instance.currentCoin -= amount;
        DataManager.Instance.SaveGame();
        OnCoinChanged?.Invoke(CurrentCoins);
        OnCoinsSpent?.Invoke();
        return true;
    }

    public void ResetCoin()
    {
        DataManager.Instance.currentCoin = 0;
        DataManager.Instance.SaveGame();
    }
}