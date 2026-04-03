using UnityEngine;
using Dreamteck.Splines;
using NaughtyAttributes;
using Lean.Pool;

public class SplineManager : MonoBehaviour
{
    public static SplineManager Instance;

    public SplineComputer spline;

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

    [Button]
    public void SpawnTurret() // Turret input a baðlanacak -- Üret diyince pooldan çekiyor bug!
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

}