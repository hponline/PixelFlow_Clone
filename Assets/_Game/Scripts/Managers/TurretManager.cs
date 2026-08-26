using UnityEngine;
using Dreamteck.Splines;
using Lean.Pool;
using System;

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

    //public event Action<int> OnTurretCountChanged;

    void Awake()
    {
        Instance = this;
    }

    public Turret SpawnTurretWithAmmo(TileType color, int ammo)
    {
        Turret prefab = GetTurretPrefabByColor(color);
        Turret turret = LeanPool.Spawn(prefab, turretContainer);
        turret.Init(gridContext, ammo);
        turret.enabled = false;
        TurretInventory.Instance.AddTurret(turret.gameObject);

        int total = TurretInventory.Instance.CheckTotalTurrets();
        //OnTurretCountChanged?.Invoke(total);

        return turret;
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
        if (!PlatePoolManager.Instance.TryGetPlate(out Plate plate))
        {
            Debug.Log("Boþ plate yok");
            return;
        }
        turret.enabled = false;

        int totalA = TurretInventory.Instance.CheckTotalTurrets();
        if (totalA <= lowTurretThreshold)
        {
            plate.Init(spline, boostedSpeed, turret, PlatePoolManager.Instance, onComplete);
            return;
        }
        else
            plate.Init(spline, speed, turret, PlatePoolManager.Instance, onComplete);
    }

    public bool HasFreePlates(int count)
    {
        return PlatePoolManager.Instance.AvailableCount >= count;
    }

}