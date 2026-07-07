using UnityEngine;

public class BoosterSelectionManager : MonoBehaviour
{
    public static BoosterSelectionManager Instance { get; private set; }

    [SerializeField] UIIntroController _UIIntroController;

    private BoosterSO activeBoosterSO;
    private BoosterType activeBoosterType;

    public bool IsSelecting => activeBoosterSO != null;

    private void Awake() => Instance = this;

    public void BeginSelection(BoosterSO boosterSO, BoosterType type)
    {
        if (IsSelecting) CancelSelection(); // ayný anda iki booster seçim modunda olamaz

        ShowPanel(true);

        activeBoosterSO = boosterSO;
        activeBoosterType = type;
        activeBoosterSO.OnSelectionStart();
    }

    public void CancelSelection()
    {
        ShowPanel(false);
        activeBoosterSO?.OnSelectionEnd();
        activeBoosterSO = null;
    }

    public bool TrySelectTurret(Turret turret)
    {
        if (!IsSelecting) return false;

        if (!BoosterManager.Instance.TryUseBooster(activeBoosterType))
        {
            CancelSelection();
            return false;
        }

        activeBoosterSO.OnSelectionEnd();

        var context = new BoosterContext { selectedTurret = turret };
        activeBoosterSO.Activate(context);

        activeBoosterSO = null;
        return true;
    }

    void ShowPanel(bool state)
    {
        _UIIntroController.ShowPanel(state);
    }

}