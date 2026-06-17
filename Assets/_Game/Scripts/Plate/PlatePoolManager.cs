using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlatePoolManager : MonoBehaviour
{
    [SerializeField] Plate[] platePlace;
    [SerializeField] Transform container;

    [Header("UI")]
    [SerializeField] TextMeshPro plateCounterTxt;
    [SerializeField] int maxPlate = 5;
    [SerializeField] int currentPlate;

    public int AvailableCount => availablePlates.Count;

    Queue<Plate> availablePlates = new();

    public Action OnChangedPlateCount;

    private void Awake()
    {
        int count = Mathf.Min(maxPlate, platePlace.Length);
        for (int i = 0; i < count; i++)
            availablePlates.Enqueue(platePlace[i]);

        currentPlate = count;
    }
    private void OnEnable()
    {
        OnChangedPlateCount += HandlePlateCount;
    }
    private void OnDisable()
    {
        OnChangedPlateCount -= HandlePlateCount;
    }

    public bool TryGetPlate(out Plate plate)
    {
        if (availablePlates.Count > 0)
        {
            plate = availablePlates.Dequeue();
            currentPlate--;
            OnChangedPlateCount?.Invoke();
            return true;
        }
        plate = null;
        return false;
    }

    public void ReturnPlate(Plate plate)
    {
        availablePlates.Enqueue(plate);
        currentPlate++;
        OnChangedPlateCount?.Invoke();
    }

    [Button]
    public void BuyPlate()
    {
        currentPlate = Mathf.Min(currentPlate + +1);
        OnChangedPlateCount?.Invoke();
    }

    void HandlePlateCount()
    {
        ShowPlateCount();
    }
    void ShowPlateCount()
    {
        plateCounterTxt.SetText("{0}/{1}", currentPlate, maxPlate);
    }
}