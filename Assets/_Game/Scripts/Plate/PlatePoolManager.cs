using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class PlatePoolManager : MonoBehaviour
{
    [SerializeField] List<Plate> plateList;
    [SerializeField] Transform plateContainer;
    [SerializeField] PlateSO plateSO;
    [SerializeField] Plate platePrefab;

    [Header("Layout")]
    [SerializeField] Transform firstSlotPoint;
    [SerializeField] Vector3 spacingOffset = new Vector3(0, 0, -2.5f);
    [SerializeField] float slideDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;
    [SerializeField] Ease slideEase = Ease.OutBack;

    [Header("UI")]
    [SerializeField] TextMeshPro plateCounterTxt;
    [SerializeField] int maxPlate = 5;
    [SerializeField] int currentPlate;

    public int AvailableCount => availablePlates.Count;

    private List<Plate> availablePlates = new();

    private void Awake()
    {
        int count = Mathf.Min(maxPlate, plateList.Count);
        for (int i = 0; i < count; i++)
            availablePlates.Add(plateList[i]);

        currentPlate = count;
        RefreshPositions();
    }

    private void OnEnable() => GameEvent.OnPlateCountChanged += HandlePlateCount;
    private void OnDisable() => GameEvent.OnPlateCountChanged -= HandlePlateCount;

    public bool TryGetPlate(out Plate plate)
    {
        if (availablePlates.Count > 0)
        {
            plate = availablePlates[0];
            availablePlates.RemoveAt(0);
            currentPlate--;
            //RefreshPositions();
            GameEvent.TriggerPlateChanged();
            return true;
        }
        plate = null;
        return false;
    }

    public void ReturnPlate(Plate plate)
    {
        availablePlates.Add(plate); // sona eklenir, queue mantýðý
        currentPlate++;
        RefreshPositions();
        GameEvent.TriggerPlateChanged();
    }

    void RefreshPositions()
    {
        for (int i = 0; i < availablePlates.Count; i++)
        {
            Transform t = availablePlates[i].transform;
            Vector3 targetPos = firstSlotPoint.position + spacingOffset * i;

            t.DOKill();
            t.DOMove(targetPos, slideDuration).SetEase(slideEase);
        }
    }

    [Button]
    public void AddPlate()
    {
        currentPlate = Mathf.Min(currentPlate + 1, maxPlate);
        GameEvent.TriggerPlateChanged();
    }

    public void BuyPlate()
    {
        if (!CoinManager.Instance.HasEnoughCoins(plateSO.platePrice)) return;

        CoinManager.Instance.SpendCoins(plateSO.platePrice);

        Plate plate = Instantiate(platePrefab, plateContainer);
        plate.transform.position = GetBuySpawnPoint(); // ekranýn altýndan baþlar

        plateList.Add(plate);
        availablePlates.Add(plate);
        maxPlate++;
        currentPlate++;

        RefreshPositions(); // bu plate de hedefine uçar
        GameEvent.TriggerPlateChanged();
    }

    Vector3 GetBuySpawnPoint()
    {
        // Ekranýn altýndan baþlangýç noktasý — Inspector'dan ayarlanabilir bir Transform daha doðru olur
        return firstSlotPoint.position + Vector3.down * 5f;
    }

    void HandlePlateCount() => ShowPlateCount();

    void ShowPlateCount()
    {
        plateCounterTxt.SetText("{0}/{1}", currentPlate, maxPlate);
    }
}