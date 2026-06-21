using System;
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

        GameEvent.TriggerSlotFull();
        return false;
    }

    public void CompactSlots()
    {
        isCompacting = true;

        int writeIndex = 0;

        for (int readIndex = 0; readIndex < turretSlots.Length; readIndex++)
        {
            if (!turretSlots[readIndex].isFull) continue;

            if (readIndex != writeIndex)
            {
                Turret t = turretSlots[readIndex].CurrentTurret;
                turretSlots[readIndex].ClearForCompact();
                turretSlots[writeIndex].Place(t, isCompacting: true);
            }

            writeIndex++;
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
