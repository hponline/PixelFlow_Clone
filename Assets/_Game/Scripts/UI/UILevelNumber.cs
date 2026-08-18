using TMPro;
using UnityEngine;

public class UILevelNumber : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelTxt;

    private void Start()
    {
        GameEvent.OnLevelChanged += OnLevelChanged;

        SetText(DataManager.Instance.currentLevel);
    }

    private void OnDisable()
    {
        GameEvent.OnLevelChanged -= OnLevelChanged;
    }

    void OnLevelChanged(int level)
    {
        SetText(level);
    }

    void SetText(int level)
    {
        levelTxt.SetText("Level {0}", level + 1);
    }
}
