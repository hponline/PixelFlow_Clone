using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlatePoolManager : MonoBehaviour
{
    public static PlatePoolManager Instance;

    [SerializeField] List<Plate> plateList;
    [SerializeField] Transform plateContainer;
    [SerializeField] Plate platePrefab;

    [Header("Layout")]
    [SerializeField] Transform firstSlotPoint;
    [SerializeField] Vector3 spacingOffset = new Vector3(0, 0, -2.5f);
    [SerializeField] float slideDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;
    [SerializeField] Ease slideEase = Ease.OutBack;

    [Header("UI")]
    [SerializeField] int maxPlate = 5;
    [SerializeField] int currentPlate;

    public int CurrentPlate => currentPlate;
    public int MaxPlate => maxPlate;
    public int AvailableCount => availablePlates.Count;

    private List<Plate> availablePlates = new();

    private void Awake()
    {
        Instance = this;

        int count = Mathf.Min(maxPlate, plateList.Count);
        for (int i = 0; i < count; i++)
            availablePlates.Add(plateList[i]);

        currentPlate = count;
        RefreshPositions();
    }

    public bool TryGetPlate(out Plate plate)
    {
        if (availablePlates.Count > 0)
        {
            plate = availablePlates[0];
            availablePlates.RemoveAt(0);
            currentPlate--;

            GameEvent.TriggerPlateChanged();
            return true;
        }
        plate = null;
        return false;
    }

    public void ReturnPlate(Plate plate)
    {
        availablePlates.Add(plate);
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

    public void AddPlateSlot()
    {
        Plate plate = Instantiate(platePrefab, plateContainer);
        plate.transform.position = GetBuySpawnPoint();

        plateList.Add(plate);
        availablePlates.Add(plate);
        maxPlate++;
        currentPlate++;

        RefreshPositions();
        GameEvent.TriggerPlateChanged();
    }

    Vector3 GetBuySpawnPoint()
    {
        return firstSlotPoint.position + Vector3.back * 20f;
    }
}