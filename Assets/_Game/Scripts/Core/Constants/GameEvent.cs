using System;
public static class GameEvent
{
    public static event Action OnGameRestart;
    public static event Action OnGameLose;
    public static event Action<int> OnLevelChanged;
    public static event Action<int> OnLivesChanged;
    public static event Action<int> OnCoinChanged;
    public static event Action OnCoinsSpend;

    public static event Action OnTurretFired;

    public static event Action OnLevelCompleted;
    public static event Action OnLevelLose;

    public static event Action OnSlotFull;
    public static event Action OnPlateCountChanged;


    public static void TriggerGameRestart() => OnGameRestart?.Invoke();
    public static void TriggerGameLose() => OnGameLose?.Invoke();

    public static void TriggerLevelCompleted() => OnLevelCompleted?.Invoke();
    public static void TriggerLevelLose() => OnLevelLose?.Invoke();
    public static void TriggerSlotFull() => OnSlotFull?.Invoke();
    public static void TriggerPlateChanged() => OnPlateCountChanged?.Invoke();

    public static void TriggerLevelChanged(int level) => OnLevelChanged?.Invoke(level);
    public static void TriggerLivesChanged(int lives) => OnLivesChanged?.Invoke(lives);
    public static void TriggerCoinChanged(int coin) => OnCoinChanged?.Invoke(coin);
    public static void TriggerCoinSpendChanged() => OnCoinsSpend?.Invoke();

    public static void TriggerTurretFired() => OnTurretFired?.Invoke();


}
