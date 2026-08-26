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
    [SerializeField] Button button;

    private void Start()
    {
        button.onClick.AddListener(ButtonPunchAnim);     
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
        BoosterManager.Instance.OnBoosterFirstUnlocked -= HandleFirstUnlock;
    }

    IEnumerator SubscribeWhenReady()
    {
        yield return new WaitUntil(() => BoosterManager.Instance != null);
        Subscribe();
    }

    private void Subscribe()
    {
        BoosterManager.Instance.OnBoosterCountChanged += HandleCountChanged;
        BoosterManager.Instance.OnBoosterFirstUnlocked += HandleFirstUnlock;
        RefreshVisual(BoosterManager.Instance.GetData(boosterType).count);
    }

    private void HandleCountChanged(BoosterType type, int count)
    {
        if (type != boosterType) return;
        RefreshVisual(count);
    }

    private void HandleFirstUnlock(BoosterType type)
    {
        if (type != boosterType) return;
        particle?.SetActive(true);
    }

    private void RefreshVisual(int count)
    {
        bool isUnlocked = BoosterManager.Instance.IsUnlocked(boosterType);
        bool hasBooster = count > 0;

        Debug.LogWarning("unlock1: " + isUnlocked);
        lockedOverlay?.SetActive(!isUnlocked);
        Debug.LogWarning("unlock2: " + isUnlocked);
        lockTxtLvlInfo.SetText("Lv {0}", boosterSO.unlockLevel);
        Debug.LogWarning("unlock3: " + isUnlocked);
        boosterActive?.gameObject.SetActive(isUnlocked && hasBooster);
        boosterPassive?.gameObject.SetActive(isUnlocked && !hasBooster);
        boosterCount?.gameObject.SetActive(isUnlocked && hasBooster);

        if (isUnlocked && hasBooster)
            boosterCount.SetText("{0}", count);
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

        int count = BoosterManager.Instance.GetData(boosterType).count;
        if (count <= 0)
        {
            ShowPanel();
            return;
        }

        if(boosterSO.RequiresSelection)
        {
            BoosterSelectionManager.Instance.BeginSelection(boosterSO, boosterType);
        }

        else
        {
            Debug.Log("UseBooster: " + boosterType);
            if (!BoosterManager.Instance.TryUseBooster(boosterType)) return;
            boosterSO.Activate(new BoosterContext());
        }
    }

    void ShowPanel()
    {
        UIManager.Instance.ShowPanel(boosterBuyPanel);
    }
}