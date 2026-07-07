using UnityEngine;

[CreateAssetMenu(fileName = "MoveHandBoosterSO", menuName = "Scriptable Objects/MoveHandBoosterSO")]
public class MoveHandBoosterSO : BoosterSO
{
    public override bool RequiresSelection => true;

    public override void OnSelectionStart()
    {
        TurretInventory.Instance.SetAllHighlighted(true);
        TurretInventory.Instance.ShowPanel(false);
    }
    public override void OnSelectionEnd()
    {
        TurretInventory.Instance.SetAllHighlighted(false);
        TurretInventory.Instance.ShowPanel(true);
    }
    public override void Activate(BoosterContext context)
    {
        if (context.selectedTurret == null) return;
        context.selectedTurret.SendLinkTurret();
    }
}
