using UnityEngine;
using Dreamteck.Splines;
using Lean.Pool;

public class TurretManager : MonoBehaviour
{
    public static TurretManager Instance;

    [SerializeField] SplineComputer spline;

    [Header("Turret")]
    [SerializeField] float speed = 5f;
    [SerializeField] Turret[] turretPrefabs;
    [SerializeField] Transform turretContainer;
    [SerializeField] PlatePool platePool;

    GridContext gridContext;

    void Awake()
    {
        Instance = this;
    }

    public Turret SpawnTurretWithAmmo(ColorType color, int ammo)
    {
        Turret prefab = GetTurretPrefabByColor(color);
        Turret turret = LeanPool.Spawn(prefab, turretContainer);
        turret.Init(gridContext, ammo);
        turret.enabled = false;
        TurretInventory.Instance.AddTurret(turret.gameObject);

        return turret;
    }

    Turret GetTurretPrefabByColor(ColorType color)
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

    public void TurretSendToSpline(Turret turret)
    {
        if (!platePool.TryGetPlate(out Plate plate))
        {
            Debug.Log("Boþ plate yok");
            return;
        }
        turret.enabled = false;
        plate.Init(spline, speed, turret, platePool);
    }

    public bool HasFreePlates(int count)
    {
        return platePool.AvailableCount >= count;
    }

    //public bool HasFreePlate()
    //{
    //    return platePool.HasAvailablePlate();
    //}
}