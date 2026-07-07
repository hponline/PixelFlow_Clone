using UnityEngine;

[CreateAssetMenu(fileName = "PlateBoosterSO", menuName = "Scriptable Objects/PlateBoosterSO")]
public class PlateBoosterSO : BoosterSO
{
    public override void Activate(BoosterContext context)
    {
        PlatePoolManager.Instance.AddPlateSlot();
    }
}
