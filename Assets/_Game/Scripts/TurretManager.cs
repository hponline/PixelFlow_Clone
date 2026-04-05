using UnityEngine;
using Dreamteck.Splines;
using NaughtyAttributes;
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

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnTurret();
    }

    public Vector3 GetSplinePosition()
    {
        Vector3 splineStartPos = (Vector3)spline.GetPoint(0).position;
        return splineStartPos;
    }

    [Button]
    public void SpawnTurret()
    {
        OnTurretSelected(Random.Range(0, turretPrefabs.Length));
    }

    public void OnTurretSelected(int index)
    {
        if (!platePool.TryGetPlate(out Plate plate))
        {
            Debug.Log("Boþ Plate Yok");
            return;
        }

        Turret turret = LeanPool.Spawn(turretPrefabs[index], turretContainer);
        turret.enabled = false;

        plate.Init(spline, speed, turret, platePool);
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
}