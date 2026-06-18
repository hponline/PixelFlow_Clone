using TMPro;
using UnityEngine;

public class UICoinNumber : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinTxt;

    private void Awake()
    {
        coinTxt.SetText("{0}", DataManager.Instance.currentCoin);
    }

    private void OnEnable()
    {
        CoinManager.Instance.OnCoinChanged += UpdateCoin;
    }
    private void OnDisable()
    {
        CoinManager.Instance.OnCoinChanged -= UpdateCoin;
    }

    void UpdateCoin(int amount)
    {
        coinTxt.SetText("{0}", DataManager.Instance.currentCoin);
        UIManager.Instance.PanelPunchAnimation(this.gameObject);
    }
}
