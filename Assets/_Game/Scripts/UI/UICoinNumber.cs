using TMPro;
using UnityEngine;

public class UICoinNumber : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinTxt;

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
}
