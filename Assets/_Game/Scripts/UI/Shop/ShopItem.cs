using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] ShopDataSO shopDataSO;

    [SerializeField] TextMeshProUGUI _packName;
    [SerializeField] Image _goldIcon;
    [SerializeField] Image _lifeIcon;
    [SerializeField] TextMeshProUGUI _lifeReward;
    [SerializeField] TextMeshProUGUI _goldReward;
    [SerializeField] TextMeshProUGUI _itemPrice;

    private void Start()
    {
        Setup(shopDataSO);
    }

    void Setup(ShopDataSO shopDataSO)
    {
        _packName.SetText(shopDataSO.packName);
        _goldIcon.sprite = shopDataSO.goldIcon;
        _lifeIcon.sprite = shopDataSO.lifeIcon;
        _lifeReward.SetText("+{0}", shopDataSO.lifeReward);
        _goldReward.text = shopDataSO.goldReward.ToString("N0");
        _itemPrice.SetText("{0:0.00} TRY", shopDataSO.itemPrice);

    }
}
