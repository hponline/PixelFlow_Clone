using TMPro;
using UnityEngine;

public class WinPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] TextMeshProUGUI rewardTxt;
    [SerializeField] TextMeshProUGUI iconRewardTxt;

    private void OnEnable()
    {
        HandleLevelChanged();
    }

    void HandleLevelChanged()
    {
        levelTxt.SetText("Level {0}", DataManager.Instance.currentLevel + 1);
    }

    public void NextLevelButton()
    {
        //BoosterManager.Instance.CheckLevelUnlocks(DataManager.Instance.NextLevel);
        Debug.LogWarning("Sonraki lvl: " + DataManager.Instance.NextLevel);
        LevelManager.Instance.NextLevel();
    }

    public void DoubleRewardButton()
    {
        // x2 reklam reward button
    }
}
