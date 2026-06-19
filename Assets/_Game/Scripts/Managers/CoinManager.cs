using UnityEngine;
using NaughtyAttributes;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public bool HasEnoughCoins(float amount) => DataManager.Instance.currentCoin >= amount;

    public int CurrentCoins => DataManager.Instance.currentCoin;


    void Awake()
    {
        Instance = this;
    }

    public void AddCoins(int amount)
    {
        DataManager.Instance.currentCoin += amount;
        DataManager.Instance.SaveGame();
        GameEvent.TriggerCoinChanged(CurrentCoins);        
    }

    [Button]
    public void TestAddCoin()
    {
        DataManager.Instance.currentCoin += 5000;
        DataManager.Instance.SaveGame();
        GameEvent.TriggerCoinChanged(CurrentCoins);
    }


    public bool SpendCoins(int amount)
    {
        if (!HasEnoughCoins(amount)) return false;

        DataManager.Instance.currentCoin -= amount;
        DataManager.Instance.SaveGame();
        GameEvent.TriggerCoinChanged(CurrentCoins);
        GameEvent.TriggerCoinSpendChanged();

        return true;
    }

    public void ResetCoin()
    {
        DataManager.Instance.currentCoin = 0;
        DataManager.Instance.SaveGame();
    }
}