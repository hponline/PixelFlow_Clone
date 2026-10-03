using UnityEngine;
using Dreamteck.Splines;
using Lean.Pool;
using System;
using System.Collections.Generic;
using NaughtyAttributes;

public class TurretManager : MonoBehaviour
{
    public static TurretManager Instance;

    [SerializeField] SplineComputer spline;

    [Header("Turret")]
    [SerializeField] float speed = 5f;
    [SerializeField] Turret[] turretPrefabs;
    [SerializeField] Transform turretContainer;
    [SerializeField] float boostedSpeed = 10f;
    [SerializeField] int lowTurretThreshold = 5;

    GridContext gridContext;

    List<Turret> turretLocks = new();

    void Awake()
    {
        Instance = this;

        GameEvent.OnLevelChanged += OnHandleTurretLocks;
        GameEvent.OnLevelCompleted += ClearTurretList;
    }
    private void OnDisable()
    {
        GameEvent.OnLevelChanged -= OnHandleTurretLocks;
        GameEvent.OnLevelCompleted -= ClearTurretList;
    }

    private void Start()
    {
        TurretLocks();
    }

    public Turret SpawnTurretWithAmmo(TileType color, int ammo, TurretUnlockState state)
    {
        Turret prefab = GetTurretPrefabByColor(color);
        Turret turret = LeanPool.Spawn(prefab, turretContainer);

        turret.Init(gridContext, ammo, state);
        if (state == TurretUnlockState.Locked)
            turretLocks.Add(turret);

        turret.enabled = false;
        TurretInventory.Instance.AddTurret(turret.gameObject);

        return turret;
    }

    void OnHandleTurretLocks(int value)
    {
        TurretLocks();
    }

    void TurretLocks()
    {
        foreach (var turret in turretLocks)
        {
            turret.GetComponent<TurretMaterial>().TurretLock();
        }
    }

    void ClearTurretList()
    {
        turretLocks.Clear();
    }

    Turret GetTurretPrefabByColor(TileType color)
    {
        foreach (var prefab in turretPrefabs)
        {
            if (prefab.turretSO.TurretColor == color)            
                return prefab;
        }

        Debug.Log($"Prefab bulunamadý: {color}");
        return turretPrefabs[0];
    }

    public void SetGridContext(GridContext context)
    {
        gridContext = context;
    }

    public Vector3 GetSplinePosition()
    {
        Vector3 splineStartPos = (Vector3)spline.GetPoint(0).position;
        return splineStartPos;
    }

    public void TurretSendToSpline(Turret turret, Action onComplete = null)
    {
        var plateManager = PlatePoolManager.Instance;
        if (!plateManager.TryGetPlate(out Plate plate))
        {
            Debug.Log("Boþ plate yok");
            return;
        }
        turret.enabled = false;

        int totalTurrets = TurretInventory.Instance.CheckTotalTurrets();
        if (totalTurrets <= lowTurretThreshold)
        {
            plate.Init(spline, boostedSpeed, turret, plateManager, onComplete);
            return;
        }
        else
            plate.Init(spline, speed, turret, plateManager, onComplete);
    }

    public bool HasFreePlates(int count)
    {
        return PlatePoolManager.Instance.AvailableCount >= count;
    }

}