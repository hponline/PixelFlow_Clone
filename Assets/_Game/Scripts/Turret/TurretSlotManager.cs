using UnityEngine;

public class TurretSlotManager : MonoBehaviour
{
    public static TurretSlotManager Instance;
    public TurretSlot[] turretSlots;

    private void Awake()
    {
        Instance = this;
    }

    public bool TryPlaceTurret(Turret turret)
    {
        foreach (var slot in turretSlots)
        {
            if (!slot.isFull)
            {
                slot.Place(turret);
                return true;
            }
        }
        return false;
    }
}
