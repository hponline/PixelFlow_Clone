using Lean.Pool;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public enum GridSide { None, Bottom, Top, Left, Right }
[RequireComponent(typeof(Collider))]
public class Turret : MonoBehaviour, IClickable
{
    event Action<int> OnProjectileFired;

    [Header("References")]
    public TurretSOData turretSO;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public bool canShoot;
    [SerializeField] int projectileMagazine;

    [Header("UI")]
    public TextMeshProUGUI magazineTxt;

    public void SetPlate(Plate plate) => currentPlate = plate;

    float animDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;
    Plate currentPlate;
    Collider _collider;
    GridContext gridContext;

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        UpdateMagazineUI();
    }

    private void Update()
    {
        TryShoot();
    }

    void TryShoot()
    {
        if (projectileMagazine <= 0) return;
        if (!canShoot) return;

        Vector2Int gridPos = WorldToGrid(transform.position);
        GridSide side = GetGridSide(gridPos);

        if (side == GridSide.None) return; // Köþede veya grid içinde, ateþ etme

        Block target = FindTargetBlock(gridPos, side);
        if (target == null) return;
        if (target.blockColor != turretSO.TurretColor) return;
        if (target.isShot) return;

        SpawnProjectile(target.transform.position, target);
    }
    void SpawnProjectile(Vector3 targetPos, Block block)
    {
        Shot();
        block.isShot = true;

        var obj = LeanPool.Spawn(projectilePrefab, firePoint.position, Quaternion.identity);
        obj.transform.DOMove(targetPos, turretSO.projectileSpeed)
            .OnComplete(() =>
            {
                LeanPool.Despawn(obj);
                block.DestroyBlock();
            });
    }
    void Shot()
    {
        projectileMagazine--;
        OnProjectileFired?.Invoke(projectileMagazine);

        if (projectileMagazine <= 0)
            TurretDeSpawn();
    }

    GridSide GetGridSide(Vector2Int gridPos)
    {
        bool onBottom = gridPos.y < 0;
        bool onTop = gridPos.y >= gridContext.height;
        bool onLeft = gridPos.x < 0;
        bool onRight = gridPos.x >= gridContext.width;

        // Köþedeyse None döndür, ateþ etme
        if ((onBottom || onTop) && (onLeft || onRight)) return GridSide.None;

        if (onBottom) return GridSide.Bottom;
        if (onTop) return GridSide.Top;
        if (onLeft) return GridSide.Left;
        if (onRight) return GridSide.Right;

        return GridSide.None; // Grid'in içinde, ateþ etme
    }

    Block FindTargetBlock(Vector2Int gridPos, GridSide side)
    {
        switch (side)
        {
            case GridSide.Bottom:
                // Turret'in X hizasýnda, aþaðýdan yukarý tara
                for (int y = 0; y < gridContext.height; y++)
                    if (gridContext.grid[gridPos.x, y] != null) return gridContext.grid[gridPos.x, y];
                break;

            case GridSide.Top:
                // Turret'in X hizasýnda, yukarýdan aþaðý tara
                for (int y = gridContext.height - 1; y >= 0; y--)
                    if (gridContext.grid[gridPos.x, y] != null) return gridContext.grid[gridPos.x, y];
                break;

            case GridSide.Left:
                // Turret'in Z hizasýnda, soldan saða tara
                for (int x = 0; x < gridContext.width; x++)
                    if (gridContext.grid[x, gridPos.y] != null) return gridContext.grid[x, gridPos.y];
                break;

            case GridSide.Right:
                // Turret'in Z hizasýnda, saðdan sola tara
                for (int x = gridContext.width - 1; x >= 0; x--)
                    if (gridContext.grid[x, gridPos.y] != null) return gridContext.grid[x, gridPos.y];
                break;
        }
        return null;
    }

    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3Int cell = LevelManager.Instance.gridComponent.WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.z);
    }

    public void Init(GridContext context, int ammo)
    {
        gridContext = context;
        projectileMagazine = ammo;
        UpdateMagazineUI();
    }


    void TurretDeSpawn()
    {
        currentPlate?.RecallPlate();
        currentPlate = null;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(transform.position, animDuration));
        seq.Join(transform.DORotate(new Vector3(0, 180, 0), animDuration));
        seq.Join(transform.DOScale(0, animDuration).SetEase(Ease.InOutSine));
        seq.OnComplete(() => LeanPool.Despawn(this));
    }
    public void SendToSlot()
    {
        TurretSlotManager.Instance.TryPlaceTurret(this);
        this.enabled = false;
        SetClickable(true);
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
            SetClickable(false);
        });
    }

    void UpdateMagazineCount(int remaining)
    {
        UpdateMagazineUI();
    }

    void UpdateMagazineUI()
    {
        magazineTxt.SetText(projectileMagazine.ToString());
    }

    private void OnEnable()
    {
        OnProjectileFired += UpdateMagazineCount;
    }
    private void OnDisable()
    {
        OnProjectileFired -= UpdateMagazineCount;
    }

    public void SetClickable(bool state)
    {
        _collider.enabled = state;
    }

    void RemoveFirstTurret()
    {
        if (!TurretManager.Instance.HasFreePlate())
        {
            Debug.Log("Boþ plate yok, kuyruk deðiþmedi");
            return;
        }
        TurretInventory.Instance.RemoveTurret(gameObject);
        TurretManager.Instance.TurretSendToSpline(this);
    }

    public void OnClick()
    {
        RemoveFirstTurret();
    }
}