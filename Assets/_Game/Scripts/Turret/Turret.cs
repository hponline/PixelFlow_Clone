using Lean.Pool;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

[RequireComponent(typeof(Collider))]
public class Turret : MonoBehaviour, IClickable
{
    public event Action<TurretState, TurretState> OnStateChanged;

    [Header("References")]
    public TurretSOData turretSO;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("UI")]
    public int projectileMagazine;
    [SerializeField] TextMeshPro magazineTxt;
    [SerializeField, Range(0, 1)] float turretAttackAnimPunch = 0.4f;

    float animDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;
    float jumpAnimDuration = GameTags.Animation.DOTWEEN_JUMP_ANIM_DURATION;
    Plate currentPlate;
    GridContext gridContext;
    Collider _collider;
    TurretLink turretLink;
    TurretShooter turretShooter;
    TurretMaterial turretMaterial;
    TurretAnimController turretAnimController;
    Vector3 defaultScale;

    [SerializeField] TurretState currentState;
    public void SetPlate(Plate plate) => currentPlate = plate;
    public void SetClickable(bool state) => _collider.enabled = state;
    public bool HasLink => turretLink != null && turretLink.HasLink;
    public Turret LinkedTurret => turretLink?.LinkedTurret;
    public TurretState CurrentState => currentState;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        turretLink = GetComponent<TurretLink>();
        turretShooter = GetComponent<TurretShooter>();
        turretMaterial = GetComponent<TurretMaterial>();
        turretAnimController = GetComponent<TurretAnimController>();

        defaultScale = transform.localScale;

        UpdateMagazineUI();
    }

    private void OnEnable()
    {
        GameEvent.OnTurretFired += UpdateMagazineUI;
    }
    private void OnDisable()
    {
        GameEvent.OnTurretFired -= UpdateMagazineUI;
    }

    private void Update()
    {
        if (currentState == TurretState.OnPlate)
            turretShooter.TryShoot();
    }

    public void Init(GridContext context, int ammo, TurretUnlockState state)
    {
        gridContext = context;
        turretShooter.Init(gridContext);
        turretMaterial.SetState(state);
        projectileMagazine = ammo;
        UpdateMagazineUI();
        turretLink.Clear();
        SetState(TurretState.InInventory);
    }

    public void SetState(TurretState next)
    {
        TurretState prev = currentState;
        currentState = next;

        switch (next)
        {
            case TurretState.InInventory:
                SetClickable(true);
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

    public void Shot()
    {
        projectileMagazine--;
        GameEvent.TriggerTurretFired();

        turretAnimController.Attack();

        transform.DOKill(true);
        transform.DOPunchScale(Vector3.one * turretAttackAnimPunch, 0.2f, vibrato: 1, elasticity: 0.1f);

        if (projectileMagazine <= 0)
            TryDeSpawn();
    }

    void TryDeSpawn()
    {
        if (turretLink == null || !turretLink.HasLink)
        {
            SetState(TurretState.Despawning);
            return;
        }

        if (turretLink.LinkedTurret.projectileMagazine <= 0 || turretLink.WaitingForDespawn)
        {
            turretLink.LinkedTurret.SetState(TurretState.Despawning);
            SetState(TurretState.Despawning);
            return;
        }
        turretLink.SetWaitingForDespawn(true);
    }

    void TurretDeSpawn()
    {
        currentPlate?.RecallPlate();
        currentPlate = null;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(transform.position + Vector3.forward, animDuration));
        seq.Join(transform.DORotate(new Vector3(0, 180, 0), animDuration));
        seq.Join(transform.DOScale(0, animDuration).SetEase(Ease.InOutSine));
        seq.OnComplete(() => LeanPool.Despawn(this));
    }

    public void SendToSlot()
    {
        if (!TurretSlotManager.Instance.TryPlaceTurret(this)) return;
        SetState(TurretState.InSlot);
        this.enabled = false;
    }

    public void SendToPlate(Plate plate, Transform mountPoint, Action onComplete = null)
    {
        transform.DOJump(TurretManager.Instance.GetSplinePosition(), 3f, 1, jumpAnimDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            SetState(TurretState.OnPlate);
            SetPlate(plate);
            transform.SetParent(mountPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            this.enabled = true;
            SetClickable(false);

            onComplete?.Invoke();
        });
    }

    void UpdateMagazineUI()
    {
        magazineTxt.SetText("{0}", projectileMagazine);
    }


    #region Spline a yollama

    // Slot >>>> Spline
    public void SendToSplineWithLink()
    {
        if (!CanSendToSpline(out Turret linked)) return;
        SendToSplineCore(linked);
    }

    // Inventory >>>> Spline // Booster için
    public void SendLinkTurret()
    {
        if (!CanSendToSpline(out Turret linked)) return;

        TurretInventory.Instance.RemoveTurret(gameObject);
        if (linked != null)
            TurretInventory.Instance.RemoveTurret(linked.gameObject);

        SendToSplineCore(linked);
    }

    void SendToSplineCore(Turret linked)
    {
        TurretManager.Instance.TurretSendToSpline(this, onComplete: () =>
        {
            if (linked == null) return;
            TurretManager.Instance.TurretSendToSpline(linked);
        });
    }

    private bool CanSendToSpline(out Turret linked)
    {
        bool hasLink = turretLink != null && turretLink.HasLink;
        int requiredPlates = hasLink ? 2 : 1;
        linked = hasLink ? turretLink.LinkedTurret : null;
        return TurretManager.Instance.HasFreePlates(requiredPlates);
    }

    #endregion

    void TurretSelectAnim()
    {
        transform.DOKill();
        transform.localScale = defaultScale;

        transform.DOPunchScale(Vector3.one * 0.3f, animDuration)
            .OnComplete(() =>
            {
                transform.localScale = defaultScale;
            });
    }
    public void OnClick()
    {
        TurretSelectAnim();
        if (PlatePoolManager.Instance.CurrentPlate == 0)
        {
            GameEvent.TriggerPlateCountEmpty();
            return;
        }

        if (BoosterSelectionManager.Instance.IsSelecting)
        {
            if (currentState == TurretState.InInventory)
                BoosterSelectionManager.Instance.TrySelectTurret(this);

            return;
        }

        if (currentState != TurretState.InInventory) return;

        SendLinkTurret();
    }
}