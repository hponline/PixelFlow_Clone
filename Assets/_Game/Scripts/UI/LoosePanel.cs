using TMPro;
using UnityEngine;

public class LoosePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] TextMeshProUGUI loseTxt;

    private void OnEnable()
    {
        SetLevelTxt();
    }

    void SetLevelTxt()
    {
        levelTxt.SetText("Level {0}", LevelManager.Instance.CurrentLevel);
    }
}
