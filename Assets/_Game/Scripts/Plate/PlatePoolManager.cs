using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlatePoolManager : MonoBehaviour
{
    [SerializeField] Plate[] platePlace;
    [SerializeField] Transform container;
    [SerializeField] PlateSO plateSO;

    [Header("UI")]
    [SerializeField] TextMeshPro plateCounterTxt;
    [SerializeField] int maxPlate = 5;
    [SerializeField] int currentPlate;

    public int AvailableCount => availablePlates.Count;

    Queue<Plate> availablePlates = new();

    private void Awake()
    {
        int count = Mathf.Min(maxPlate, platePlace.Length);
        for (int i = 0; i < count; i++)
            availablePlates.Enqueue(platePlace[i]);

        currentPlate = count;
    }
    private void OnEnable()
    {
        GameEvent.OnPlateCountChanged += HandlePlateCount;
    }
    private void OnDisable()
    {
        GameEvent.OnPlateCountChanged -= HandlePlateCount;
    }

    public bool TryGetPlate(out Plate plate)
    {
        if (availablePlates.Count > 0)
        {
            plate = availablePlates.Dequeue();
            currentPlate--;
            GameEvent.TriggerPlateChanged();
            return true;
        }
        plate = null;
        return false;
    }

    public void ReturnPlate(Plate plate)
    {
        availablePlates.Enqueue(plate);
        currentPlate++;
        GameEvent.TriggerPlateChanged();
    }

    [Button]
    public void AddPlate()
    {
        currentPlate = Mathf.Min(currentPlate + +1);
        GameEvent.TriggerPlateChanged();
    }

    public void BuyPlate()
    {
        if (!CoinManager.Instance.HasEnoughCoins(plateSO.platePrice)) return;

        CoinManager.Instance.SpendCoins(plateSO.platePrice);

        // LeanPool ile plate önceden oluþtur sonra kullanýcýya ver

        maxPlate++;
        currentPlate++;
        GameEvent.TriggerPlateChanged();
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