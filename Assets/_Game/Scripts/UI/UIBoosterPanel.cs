using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBoosterPanel : MonoBehaviour
{
    [SerializeField] BoosterType boosterType;
    [SerializeField] CurrencyType currencyType;
    [SerializeField] TextMeshProUGUI amountTxt;
    [SerializeField] TextMeshProUGUI boosterDesp;
    [SerializeField] TextMeshProUGUI boosterAddDesp;
    [SerializeField] BoosterSO boosterSO;

    [SerializeField] Image boosterImage;

    private void OnEnable()
    {
        Setup();
        amountTxt.SetText("{0}", boosterSO.price);
    }

    void Setup()
    {
        boosterImage.sprite = boosterSO.BoosterIcon;
        boosterDesp.SetText(boosterSO.BoosterDescription);
        boosterAddDesp.SetText(boosterSO.BoosterAddDescription);
    }

    public void BuyBooster()
    {
        if (!CurrencyManager.Instance.Has(currencyType, boosterSO.price)) return;

        CurrencyManager.Instance.TrySpend(currencyType, boosterSO.price);
        BoosterManager.Instance.AddBooster(boosterType, 1);
        UIManager.Instance.ClosePanel();
    }
}
