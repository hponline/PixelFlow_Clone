using TMPro;
using UnityEngine;

public class UILevelNumber : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelTxt;

    private void Start()
    {
        GameEvent.OnLevelChanged += OnLevelChanged;

        SetText();
    }

    private void OnDisable()
    {
        GameEvent.OnLevelChanged -= OnLevelChanged;
    }

    void OnLevelChanged(int level)
    {
        SetText();
    }

    void SetText()
    {
        levelTxt.SetText("Level {0}", DataManager.Instance.currentLevel);
    }
}
