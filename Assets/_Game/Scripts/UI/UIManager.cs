using DG.Tweening;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] UIIntroController _UIIntroController;

    public GameObject[] panels;


    [Header("UIPanels")]
    [SerializeField] GameObject settingPanel;
    [SerializeField] GameObject coinPanel;
    [SerializeField] GameObject losePanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject boosterPlatePanel;
    [SerializeField] GameObject boosterPanel2;
    [SerializeField] GameObject boosterPanel3;
    [SerializeField] GameObject boosterButton;


    private void Awake()
    {
        Instance = this;

    }
    private void OnEnable()
    {
        GameEvent.OnGameLose += HandleLosePanel;
        GameEvent.OnLevelLose += HandleLosePanel;
        GameEvent.OnGameRestart += ClosePanel;
        GameEvent.OnLevelCompleted += HandleWinPanel;


    }
    private void OnDisable()
    {
        GameEvent.OnGameLose -= HandleLosePanel;
        GameEvent.OnLevelLose -= HandleLosePanel;
        GameEvent.OnGameRestart -= ClosePanel;
        GameEvent.OnLevelCompleted -= HandleWinPanel;
    }

    void HandleWinPanel()
    {
        ShowPanel(winPanel);
    }

    void HandleLosePanel()
    {
        if (losePanel.activeInHierarchy) return;
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

        _UIIntroController.ShowPanel(true);
    }

    public void PanelPunchAnimation(GameObject rectTransform)
    {
        rectTransform.transform.DOKill(true);
        rectTransform.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), GameTags.Animation.DOTWEEN_ANIM_DURATION, 5, 1f);
    }

    #region UIButton

    public void BoosterPlateButton()
    {
        ShowPanel(boosterPlatePanel);
    }

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
