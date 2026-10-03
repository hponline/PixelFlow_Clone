using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBoosterPanel : MonoBehaviour
{
    [SerializeField] BoosterType boosterType;
    [SerializeField] TextMeshProUGUI amountTxt;
    [SerializeField] TextMeshProUGUI boosterDesp;
    [SerializeField] TextMeshProUGUI boosterAddDesp;
    [SerializeField] BoosterSO boosterSO;

    [SerializeField] Image boosterImage;

    private void OnEnable()
    {
        SetIcon();
        amountTxt.SetText("{0}", boosterSO.price);
    }

    void SetIcon()
    {
        boosterImage.sprite = boosterSO.BoosterIcon;
        boosterDesp.SetText(boosterSO.BoosterDescription);
        boosterAddDesp.SetText(boosterSO.BoosterAddDescription);
    }

    public void BuyBooster()
    {
        if (!CoinManager.Instance.HasEnoughCoins(boosterSO.price)) return;
        CoinManager.Instance.SpendCoins(boosterSO.price);

        BoosterManager.Instance.AddBooster(boosterType, 1);
        UIManager.Instance.ClosePanel();
    }
}
