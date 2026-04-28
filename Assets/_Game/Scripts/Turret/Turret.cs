using Lean.Pool;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public enum TurretState { InInventory, InSlot, OnPlate, Despawning }
public enum GridSide { None, Bottom, Top, Left, Right }
[RequireComponent(typeof(Collider))]
public class Turret : MonoBehaviour, IClickable
{
    public event Action<TurretState, TurretState> OnStateChanged;
    event Action<int> OnProjectileFired;

    [Header("References")]
    public TurretSOData turretSO;
    public GameObject projectilePrefab;
    public Transform firePoint;
    //public bool canShoot;

    [Header("UI")]
    [SerializeField] int projectileMagazine;
    public TextMeshProUGUI magazineTxt;
    Collider _collider;

    //public void SetPlate(Plate plate) => currentPlate = plate;

    float animDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;
    Plate currentPlate;
    GridContext gridContext;
    [SerializeField] TurretState currentState;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        UpdateMagazineUI();
    }

    private void Update()
    {
        if (currentState == TurretState.OnPlate)
            TryShoot();
    }
    public void Init(GridContext context, int ammo)
    {
        gridContext = context;
        projectileMagazine = ammo;
        UpdateMagazineUI();
        SetState(TurretState.InInventory);
    }

    public void SetState(TurretState next)
    {
        TurretState prev = currentState;
        currentState = next;

        switch (next)
        {
            case TurretState.InInventory:
                SetClickable(false);
                break;
            case TurretState.InSlot:
                SetClickable(false);
                break;
            case TurretState.OnPlate:
                SetClickable(false);
                break;
            case TurretState.Despawning:
                SetClickable(false);
                TurretDeSpawn();
                break;
        }

        OnStateChanged?.Invoke(prev, next);
    }


    void TryShoot()
    {
        if (projectileMagazine <= 0) return;
        //if (!canShoot) return;

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

        transform.DOKill(true);
        transform.DOPunchScale(Vector3.one * 0.4f, 0.2f, vibrato: 1, elasticity: 0.1f);

        if (projectileMagazine <= 0)
            SetState(TurretState.Despawning);
    }

    #region Grid iþlemleri
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
    #endregion


    void TurretDeSpawn()
    {
        currentPlate.RecallPlate();
        currentPlate = null;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(transform.position + Vector3.forward, animDuration));
        seq.Join(transform.DORotate(new Vector3(0, 180, 0), animDuration));
        seq.Join(transform.DOScale(0, animDuration).SetEase(Ease.InOutSine));
        seq.OnComplete(() => LeanPool.Despawn(this));
    }

    public void SetPlate(Plate plate) => currentPlate = plate;
    public Plate GetPlate() => currentPlate;
    public void SetClickable(bool state) => _collider.enabled = state;

    public void SendToSlot()
    {
        if (!TurretSlotManager.Instance.TryPlaceTurret(this)) return;
        SetState(TurretState.InSlot);
        this.enabled = false;
        //SetClickable(true);
    }

    public void SendToPlate(Plate plate, Transform mountPoint)
    {
        transform.DOMove(TurretManager.Instance.GetSplinePosition(), animDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            SetState(TurretState.OnPlate);
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


    public void OnClick()
    {
        if (currentState != TurretState.InInventory) return;
        if (!TurretManager.Instance.HasFreePlate()) return;

        TurretInventory.Instance.RemoveTurret(gameObject);
        TurretManager.Instance.TurretSendToSpline(this);
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