using System.Collections.Generic;
using UnityEngine;

public class TurretSlotManager : MonoBehaviour
{
    public static TurretSlotManager Instance;
    public TurretSlot[] turretSlots;
    bool isCompacting = false;

    private void Awake() => Instance = this;

    public bool TryPlaceTurret(Turret turret)
    {
        if (isCompacting) return false;

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

    public void CompactSlots(int fromIndex)
    {
        isCompacting = true;

        for (int i = fromIndex; i < turretSlots.Length - 1; i++)
        {
            // Bir sonraki slot boþsa kaydýracak bir þey yok
            if (!turretSlots[i + 1].isFull) break;

            Turret turret = turretSlots[i + 1].CurrentTurret;
            turretSlots[i + 1].ClearState();
            turretSlots[i].Place(turret, isCompacting: true);
        }

        isCompacting = false;
    }    
}
