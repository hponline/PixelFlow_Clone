using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBoosterButton : MonoBehaviour
{
    [SerializeField] private BoosterType boosterType;
    [SerializeField] GameObject boosterBuyPanel;
    [SerializeField] BoosterSO boosterSO;

    [SerializeField] private GameObject particle;
    [SerializeField] private TextMeshProUGUI boosterCount;
    [SerializeField] private Image boosterActive;
    [SerializeField] private Image boosterPassive;
    [SerializeField] private GameObject panelPunchAnim;
    [SerializeField] Button button;

    private void Start()
    {
        BoosterManager.Instance.OnBoosterCountChanged += HandleCountChanged;
        BoosterManager.Instance.OnBoosterFirstUnlocked += HandleFirstUnlock;

        button.onClick.AddListener(ButtonPunchAnim);

        RefreshVisual(BoosterManager.Instance.GetData(boosterType).count);        
    }

    private void OnDisable()
    {
        BoosterManager.Instance.OnBoosterCountChanged -= HandleCountChanged;
        BoosterManager.Instance.OnBoosterFirstUnlocked -= HandleFirstUnlock;
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
        bool hasBooster = count > 0;

        boosterActive?.gameObject.SetActive(hasBooster);
        boosterPassive?.gameObject.SetActive(!hasBooster);
        boosterCount?.gameObject.SetActive(hasBooster);

        if (hasBooster)
            boosterCount.SetText("{0}", count);
    }

    void ButtonPunchAnim()
    {
        UIManager.Instance.PanelPunchAnimation(panelPunchAnim);
    }

    public void OnClick()
    {
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