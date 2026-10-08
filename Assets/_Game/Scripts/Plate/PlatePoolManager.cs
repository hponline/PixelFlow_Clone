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
    [SerializeField] int baseMaxPlate = 5;
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

    private void OnValidate()
    {
        RefreshPositions();
    }

    private void Awake()
    {
        Instance = this;

        int count = Mathf.Min(baseMaxPlate, plateList.Count);
        for (int i = 0; i < count; i++)
            availablePlates.Add(plateList[i]);

        currentPlate = count;
        RefreshPositions();

        GameEvent.OnLevelChanged += OnHandleResetPlate;
    }
    private void OnDisable()
    {
        GameEvent.OnLevelChanged -= OnHandleResetPlate;
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
        AddPlateSlot();
    }

    public void AddPlateSlot()
    {
        Plate plate = Instantiate(platePrefab, plateContainer);
        plate.transform.position = GetBuySpawnPoint();
        plate.transform.rotation = plate.startLocalRot;
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

    [Button]
    void ResetPlate()
    {
        while (plateList.Count > baseMaxPlate)
        {
            Plate extra = plateList[^1];
            plateList.RemoveAt(plateList.Count -1);
            availablePlates.Remove(extra);
            Destroy(extra.gameObject);
        }

        maxPlate = baseMaxPlate;
        availablePlates.Clear();
        for (int i = 0; i < baseMaxPlate; i++)
            availablePlates.Add(plateList[i]);

        currentPlate = baseMaxPlate;
        RefreshPositions();
        GameEvent.TriggerPlateChanged();
    }

    void OnHandleResetPlate(int value)
    {
        ResetPlate();
    }
}