using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopPackItemUI : MonoBehaviour
{
    [SerializeField] ShopPackData pack;
    [SerializeField] TMP_Text nameText, badgeText, goldText, livesText, plateTxt, MoveHandTxt, shuffleTxt, priceText;
    [SerializeField] Button buyButton;

    private void OnEnable()
    {
        ShopManager.Instance.OnReady += RefreshPrice;
        RefreshPrice();
    }

    private void OnDisable()
    {
        ShopManager.Instance.OnReady -= RefreshPrice;
    }

    void Start()
    {
        nameText.SetText(pack.displayName);
        goldText.SetText(pack.gold.ToString("N0") + " Gold");

        if(livesText != null)
            livesText.SetText($"x{pack.lives}");

        badgeText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(pack.badge));
        badgeText.SetText(pack.badge);

        buyButton.onClick.AddListener(() => ShopManager.Instance.Buy(pack));
    }

    void RefreshPrice()
    {
        var shop = ShopManager.Instance;
        priceText.SetText(shop.IsReady ? shop.GetPrice(pack) : "...");
        buyButton.interactable = shop.IsReady;
    }
}
