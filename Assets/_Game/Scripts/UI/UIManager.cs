using DG.Tweening;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject[] panels;


    [Header("UIPanels")]
    [SerializeField] GameObject settingPanel;
    [SerializeField] GameObject coinPanel;
    [SerializeField] GameObject losePanel;

    private void Awake()
    {
        Instance = this;

    }
    private void OnEnable()
    {
        GameEvent.OnGameLose += HandleLosePanel;
    }
    private void OnDisable()
    {
        GameEvent.OnGameLose -= HandleLosePanel;
    }

    void HandleLosePanel()
    {
        ShowPanel(losePanel);
    }

    public void ShowPanel(GameObject targetPanel)
    {
        foreach (var panel in panels)
            panel.SetActive(false);

        PanelPunchAnimation(targetPanel);
        targetPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
    }

    public void PanelPunchAnimation(GameObject rectTransform)
    {
        rectTransform.transform.DOKill(true);
        rectTransform.transform.localScale = Vector3.one;
        rectTransform.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), GameTags.Animation.DOTWEEN_ANIM_DURATION, 5, 1f);
    }

    #region UIButton

    public void SettingButton()
    {
        ShowPanel(settingPanel);
    }

    public void CoinButton()
    {
        ShowPanel(coinPanel);
    }

    #endregion
}
