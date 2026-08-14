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

    GridContext gridContext;

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

        return turret;
    }

    Turret GetTurretPrefabByColor(TileType color)
    {
        foreach (var prefab in turretPrefabs)
        {
            if (prefab.turretSO.TurretColor == color)            
                return prefab;
        }

        Debug.Log($"Prefab bulunamadı: {color}");
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
            Debug.Log("Boş plate yok");
            return;
        }
        turret.enabled = false;
        plate.Init(spline, speed, turret, PlatePoolManager.Instance, onComplete);
    }

    public bool HasFreePlates(int count)
    {
        return PlatePoolManager.Instance.AvailableCount >= count;
    }

}