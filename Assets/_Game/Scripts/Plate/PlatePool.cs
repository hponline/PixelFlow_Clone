using System.Collections.Generic;
using UnityEngine;

public class PlatePool : MonoBehaviour
{
    [SerializeField] Plate[] platePlace;
    [SerializeField] int maxPlate = 5;
    [SerializeField] Transform container;

    Queue<Plate> availablePlates = new();

    private void Awake()
    {
        int count = Mathf.Min(maxPlate, platePlace.Length);
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