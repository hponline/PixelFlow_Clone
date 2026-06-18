using TMPro;
using UnityEngine;

public class UILevelNumber : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelTxt;

    private void Awake()
    {
        SetText();
    }

    private void OnEnable()
    {
        SetText();
    }

    void SetText()
    {
        levelTxt.SetText("Level {0}", DataManager.Instance.currentLevel);
    }
}
