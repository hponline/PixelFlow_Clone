using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public int currentLevel {  get; private set; }
    public int currentCoin;
    public int currentLife;

    private void Awake()
    {
        Instance = this;

        Setup();
    }

    void Setup()
    {
        currentLevel = PlayerPrefs.GetInt(GameTags.PlayerPrefsKeys.SAVED_LEVEL, 1);
        currentCoin = PlayerPrefs.GetInt(GameTags.PlayerPrefsKeys.PLAYER_COIN, 0);
        currentLife = PlayerPrefs.GetInt(GameTags.PlayerPrefsKeys.CURRENT_LIFE, 5);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt(GameTags.PlayerPrefsKeys.SAVED_LEVEL, currentLevel);
        PlayerPrefs.SetInt(GameTags.PlayerPrefsKeys.PLAYER_COIN, currentCoin);
        PlayerPrefs.SetInt(GameTags.PlayerPrefsKeys.CURRENT_LIFE, currentLife);

        PlayerPrefs.Save();
    }
}
