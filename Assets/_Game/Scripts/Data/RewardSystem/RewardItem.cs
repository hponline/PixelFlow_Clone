using System;

public enum RewardType { Currency, Life, Booster }
public enum RewardSource { LevelWin, Ad, IAP, Daily }

[Serializable]
public struct RewardItem
{
    public RewardType type;
    public CurrencyType currency;   // kind == Currency ise
    public BoosterType booster;     // kind == Booster ise
    public int amount;
}
