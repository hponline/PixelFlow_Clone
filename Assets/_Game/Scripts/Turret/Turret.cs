using Lean.Pool;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public class Turret : MonoBehaviour
{
    [Header("References")]
    public TurretSOData turretSO;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("UI")]
    public TextMeshProUGUI magazineTxt;

    Plate currentPlate;
    public void SetPlate(Plate plate) => currentPlate = plate;

    [SerializeField] int projectileMagazine;
    float animDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;

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

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(transform.position + new Vector3(0, 0, 2), animDuration));
        seq.Join(transform.DORotate(new Vector3(0, 360, 0), animDuration));
        seq.Join(transform.DOScale(0, animDuration).SetEase(Ease.InOutSine));
        seq.OnComplete(() => Destroy(gameObject));
    }

    public void SendToSlot() // Animasyon yapýlacak
    {
        TurretSlotManager.Instance.TryPlaceTurret(this);
    }

    public void SendToPlate(Plate plate, Transform mountPoint)
    {
        transform.DOMove(TurretManager.Instance.GetSplinePosition(), animDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            SetPlate(plate);
            
            transform.SetParent(mountPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            this.enabled = true;
        });
    }

    void Shot()
    {
        projectileMagazine--;
        OnProjectileFired?.Invoke(projectileMagazine);

        if (projectileMagazine <= 0)        
            TurretDeSpawn();
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