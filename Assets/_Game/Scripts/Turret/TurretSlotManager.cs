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

    public void CompactSlots(int fromIndex, int secondIndex = -1)
    {
        isCompacting = true;

        // Ýki boþluk varsa küçük index'ten büyüðe sýrala
        List<int> emptyIndices = new List<int> { fromIndex };
        if (secondIndex >= 0) emptyIndices.Add(secondIndex);
        emptyIndices.Sort();

        // Her boþluk için sola kaydýr
        foreach (int emptyIndex in emptyIndices)
        {
            for (int i = emptyIndex; i < turretSlots.Length - 1; i++)
            {
                if (!turretSlots[i + 1].isFull) break;

                Turret t = turretSlots[i + 1].CurrentTurret;
                turretSlots[i + 1].ClearState();
                turretSlots[i].Place(t, isCompacting: true);
            }
        }

        isCompacting = false;
    }

    public TurretSlot GetSlotOf(Turret turret)
    {
        foreach (var slot in turretSlots)
        {
            if (slot.CurrentTurret == turret) return slot;
        }
        return null;
    }
}
