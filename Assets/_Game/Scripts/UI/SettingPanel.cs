using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] GameObject restartBtn;
    [SerializeField] GameObject mainMenuBtn;

    [SerializeField] GameObject privacyButton;
    [SerializeField] GameObject contactButton;

    public void LinkButton(string link)
    {
        Application.OpenURL(link);
    }

    public void PrivacyButton()
    {
        UIManager.Instance.PanelPunchAnimation(privacyButton);
    }
    public void ContactButton()
    {
        UIManager.Instance.PanelPunchAnimation(contactButton);
    }

    public void RestartButton()
    {
        UIManager.Instance.PanelPunchAnimation(restartBtn);
    }

    public void HapticButton()
    {

    }
}
