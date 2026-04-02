using Dreamteck.Splines;
using Lean.Pool;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public class Turret : MonoBehaviour
{
    public SplineFollower follower;

    Plate currentPlate;
    public void SetPlate(Plate plate) => currentPlate = plate;

    public TurretSOData turretSO;

    public GameObject projectilePrefab;
    public Transform firePoint;

    [SerializeField] int projectileMagazine;

    [Header("UI")]
    public TextMeshProUGUI magazineTxt;

    event Action<int> OnProjectileFired;

    //    static readonly ColorType[] values =
    //(ColorType[])System.Enum.GetValues(typeof(ColorType));

    private void Awake()
    {
        //turretColor = values[Random.Range(0, values.Length)];
        projectileMagazine = turretSO.magazine;
        UpdateMagazineUI();
    }

    private void Update()
    {
        TryShoot();
    }

    void TryShoot()
    {
        if (projectileMagazine <= 0) return;

        RaycastHit hit;
        if (!Physics.Raycast(firePoint.position, firePoint.forward, out hit, turretSO.raycastRange, turretSO.blockLayerMask)) return;

        Block block = hit.collider.GetComponent<Block>();

        if (block.isShot || block.blockColor != turretSO.TurretColor) return;

        SpawnProjectile(hit.point, block);
    }

    void SpawnProjectile(Vector3 targetPos, Block block) // Prefab yerine trail renderer olabilir
    {
        Shot();
        block.isShot = true;

        var obj = LeanPool.Spawn(projectilePrefab, firePoint.position, Quaternion.identity);
        obj.transform.DOMove(targetPos, turretSO.projectileSpeed)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                LeanPool.Despawn(obj);
                block.DestroyBlock();
            });
    }

    void TurretDeSpawn()
    {
        currentPlate?.RecallPlate();
        currentPlate = null;
        // Animasyon ile Despawn
        // Reset turret
        LeanPool.Despawn(this);
    }

    void Shot()
    {
        projectileMagazine--;
        OnProjectileFired?.Invoke(projectileMagazine);

        if (projectileMagazine <= 0)
        {
            TurretDeSpawn();
        }
    }

    void UpdateMagazineCount(int remaining)
    {
        UpdateMagazineUI();
    }

    void UpdateMagazineUI()
    {
        magazineTxt.SetText(projectileMagazine.ToString());
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(firePoint.position, firePoint.forward * turretSO.raycastRange);
    }

    private void OnEnable()
    {
        OnProjectileFired += UpdateMagazineCount;
    }
    private void OnDisable()
    {
        OnProjectileFired -= UpdateMagazineCount;
    }
}