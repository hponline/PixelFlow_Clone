using System.Collections.Generic;


[System.Serializable]
public class BoosterSaveData
{
    public BoosterType type;
    public int count;
    public bool hasBeenUnlockedOnce; // ilk kazaným flag'i
    public bool isLevelUnlocked;
}

[System.Serializable]
public class BoosterSaveWrapper
{
    public List<BoosterSaveData> boosters = new List<BoosterSaveData>();
}
