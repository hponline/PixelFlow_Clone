using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public int currentLevel {  get; private set; }
    public int currentCoin;

    public int NextLevel => currentLevel + 1;

    private void Awake()
    {
        Instance = this;

        Setup();
    }

    void Setup()
    {
        currentLevel = PlayerPrefs.GetInt(GameTags.PlayerPrefsKeys.CURRENT_LEVEL, 0);
        currentCoin = PlayerPrefs.GetInt(GameTags.PlayerPrefsKeys.PLAYER_COIN, 0);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt(GameTags.PlayerPrefsKeys.CURRENT_LEVEL, currentLevel);
        PlayerPrefs.SetInt(GameTags.PlayerPrefsKeys.PLAYER_COIN, currentCoin);

        PlayerPrefs.Save();
    }

    public void SaveLevel(int level)
    {
        currentLevel = level;
        PlayerPrefs.SetInt(GameTags.PlayerPrefsKeys.CURRENT_LEVEL, currentLevel);
        PlayerPrefs.Save();
    }
}
