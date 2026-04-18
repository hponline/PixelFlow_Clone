using System.Collections.Generic;
using UnityEngine;

public class PlatePool : MonoBehaviour
{
    [SerializeField] private Plate[] platePlace;
    [SerializeField] private PlateSlotData slotData;
    [SerializeField] private Transform container;

    private Queue<Plate> availablePlates = new();

    private void Awake()
    {
        int count = Mathf.Min(slotData.maxPlates, platePlace.Length);
        for (int i = 0; i < count; i++)
            availablePlates.Enqueue(platePlace[i]);
    }

    public bool HasAvailablePlate()
    {
        return availablePlates.Count > 0;
    }

    public bool TryGetPlate(out Plate plate)
    {
        if (availablePlates.Count > 0)
        {
            plate = availablePlates.Dequeue();
            return true;
        }
        plate = null;
        return false;
    }

    public void ReturnPlate(Plate plate)
    {
        availablePlates.Enqueue(plate);
    }
}