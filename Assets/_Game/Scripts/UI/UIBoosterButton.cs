using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBoosterButton : MonoBehaviour
{
    [SerializeField] private BoosterType boosterType;
    [SerializeField] GameObject boosterBuyPanel;
    [SerializeField] BoosterSO boosterSO;

    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private TextMeshProUGUI lockTxtLvlInfo;

    [SerializeField] private GameObject particle;
    [SerializeField] private TextMeshProUGUI boosterCount;
    [SerializeField] private Image boosterActive;
    [SerializeField] private Image boosterPassive;
    [SerializeField] private GameObject panelPunchAnim;
    [SerializeField] Button activeButton;
    [SerializeField] Button passiveButton;

    private void Start()
    {
        activeButton.onClick.AddListener(ButtonPunchAnim);
        passiveButton.onClick.AddListener(ButtonPunchAnim);
    }

    private void OnEnable()
    {
        if (BoosterManager.Instance == null)
        {
            StartCoroutine(SubscribeWhenReady());
            return;
        }
        Subscribe();
    }

    private void OnDisable()
    {
        BoosterManager.Instance.OnBoosterCountChanged -= HandleCountChanged;
        BoosterManager.Instance.OnBoosterFirstUnlocked -= HandleUnlockEvent;
        BoosterManager.Instance.OnBoosterLevelUnlocked -= HandleUnlockEvent;
    }

    IEnumerator SubscribeWhenReady()
    {
        yield return new WaitUntil(() => BoosterManager.Instance != null);
        Subscribe();
    }

    private void Subscribe()
    {
        BoosterManager.Instance.OnBoosterCountChanged += HandleCountChanged;
        BoosterManager.Instance.OnBoosterFirstUnlocked += HandleUnlockEvent;
        BoosterManager.Instance.OnBoosterLevelUnlocked += HandleUnlockEvent;
        RefreshVisual();
    }

    private void HandleCountChanged(BoosterType type, int count)
    {
        if (type != boosterType) return;
        particle?.SetActive(false);
        RefreshVisual();
    }

    private void HandleUnlockEvent(BoosterType type)
    {
        if (type != boosterType) return;
        particle?.SetActive(true);
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        var boosterData = BoosterManager.Instance;
        bool isUnlocked = boosterData.IsUnlocked(boosterType);
        bool hasBooster = boosterData.Has(boosterType);

        lockedOverlay?.SetActive(!isUnlocked);
        lockTxtLvlInfo.SetText("Lv {0}", boosterSO.unlockLevel);
        boosterActive?.gameObject.SetActive(isUnlocked && hasBooster);
        boosterPassive?.gameObject.SetActive(isUnlocked && !hasBooster);
        boosterCount?.gameObject.SetActive(isUnlocked && hasBooster);

        if (isUnlocked && hasBooster)
            boosterCount.SetText("{0}", boosterData.GetCount(boosterType));
    }

    void ButtonPunchAnim()
    {
        UIManager.Instance.PanelPunchAnimation(panelPunchAnim);
    }

    public void OnClick()
    {
        if (!BoosterManager.Instance.IsUnlocked(boosterType))
        {
            Debug.Log("Booster açýk deðil");
            return;
        }

        int count = BoosterManager.Instance.GetCount(boosterType);
        if (count <= 0)
        {
            ShowBuyPanel();
            return;
        }

        if (boosterSO.RequiresSelection)
        {
            BoosterSelectionManager.Instance.BeginSelection(boosterSO, boosterType);
        }

        else
        {
            Debug.Log("UseBooster: " + boosterType);
            if (!BoosterManager.Instance.TryUseBooster(boosterType)) return;
            boosterSO.Activate(new BoosterContext());
            // plate ve shuffle için
        }
    }

    void ShowBuyPanel()
    {
        UIManager.Instance.ShowPanel(boosterBuyPanel);
    }
}