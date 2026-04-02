using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

public class PlatePool : MonoBehaviour
{
    [SerializeField] private Plate[] platePlace; // platePrefab

    //[SerializeField] private Plate platePrefab; // platePrefab
    [SerializeField] private PlateSlotData slotData;
    [SerializeField] private Transform container;

    private Queue<Plate> availablePlates = new();

    private void Awake()
    {
        for (int i = 0; i < slotData.maxPlates; i++)
        {
            //var plate = LeanPool.Spawn(platePlace[i], container); // platePrefab
            //availablePlates.Enqueue(plate);

            availablePlates.Enqueue(platePlace[i]);
        }
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