using TMPro;
using UnityEngine;

public class UICoinNumber : MonoBehaviour
{
    [SerializeField] UIIntroController _UIIntroController;

    [SerializeField] TextMeshProUGUI coinTxt;
    [SerializeField] GameObject shopPanel;

    private void Start()
    {
        coinTxt.SetText("{0}", CurrencyManager.Instance.Get(CurrencyType.Coin));
        
        CurrencyManager.Instance.OnCurrencyChanged += UpdateCoin;
    }

    private void OnDisable()
    {
        CurrencyManager.Instance.OnCurrencyChanged -= UpdateCoin;
    }

    void UpdateCoin(CurrencyType type, int amount)
    {
        if (type != CurrencyType.Coin) return;

        coinTxt.SetText("{0}", CurrencyManager.Instance.Get(CurrencyType.Coin));
        UIManager.Instance.PanelPunchAnimation(this.gameObject);
    }

    public void ShowPanel()
    {
        _UIIntroController.ShowPanel(false);
        UIManager.Instance.PanelPunchAnimation(this.gameObject);
        UIManager.Instance.ShowPanel(shopPanel);        
    }

}
