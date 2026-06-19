using TMPro;
using UnityEngine;

public class LoosePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] TextMeshProUGUI loseTxt;


    private void OnEnable()
    {
        GameEvent.OnLevelChanged += HandleLevel;

        SetLevelTxt();
    }
    private void OnDisable()
    {
        GameEvent.OnLevelChanged -= HandleLevel;
    }

    void HandleLevel(int level)
    {
        SetLevelTxt();
    }

    void SetLevelTxt()
    {
        levelTxt.SetText("Level {0}", DataManager.Instance.currentLevel);
    }
}
