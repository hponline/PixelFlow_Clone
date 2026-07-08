using UnityEngine;

public class UIShopPanel : MonoBehaviour
{
    [SerializeField] UIIntroController _UIIntroController;

    public void OnClick()
    {
        _UIIntroController.ShowPanel(true);
    }
}
