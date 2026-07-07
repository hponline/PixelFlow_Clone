using TMPro;
using UnityEngine;

public class UIBoosterPanel : MonoBehaviour
{
    [SerializeField] BoosterType boosterType;
    [SerializeField] TextMeshProUGUI amountTxt;
    [SerializeField] BoosterSO boosterSO;

    private void OnEnable()
    {
        amountTxt.SetText("{0}", boosterSO.price);
    }

    public void BuyBooster()
    {
        if (!CoinManager.Instance.HasEnoughCoins(boosterSO.price)) return;
        CoinManager.Instance.SpendCoins(boosterSO.price);

        BoosterManager.Instance.AddBooster(boosterType, 1);
        UIManager.Instance.ClosePanel();
    }
}
