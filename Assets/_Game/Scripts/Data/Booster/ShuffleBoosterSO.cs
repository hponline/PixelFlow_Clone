using UnityEngine;

[CreateAssetMenu(fileName = "ShuffleBoosterSO", menuName = "Scriptable Objects/ShuffleBoosterSO")]
public class ShuffleBoosterSO : BoosterSO
{
    public override void Activate(BoosterContext context)
    {
        TurretInventory.Instance.ShuffleButton();
    }
}
