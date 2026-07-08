using TMPro;
using UnityEngine;

public class UICoinNumber : MonoBehaviour
{
    [SerializeField] UIIntroController _UIIntroController;

    [SerializeField] TextMeshProUGUI coinTxt;
    [SerializeField] GameObject shopPanel;

    private void Start()
    {
        coinTxt.SetText("{0}", DataManager.Instance.currentCoin);
        
        GameEvent.OnCoinChanged += UpdateCoin;
    }

    private void OnDisable()
    {
        GameEvent.OnCoinChanged -= UpdateCoin;
    }

    void UpdateCoin(int amount)
    {
        coinTxt.SetText("{0}", DataManager.Instance.currentCoin);
        UIManager.Instance.PanelPunchAnimation(this.gameObject);
    }

    public void ShowPanel()
    {
        _UIIntroController.ShowPanel(false);
        UIManager.Instance.PanelPunchAnimation(this.gameObject);
        UIManager.Instance.ShowPanel(shopPanel);        
    }

}
