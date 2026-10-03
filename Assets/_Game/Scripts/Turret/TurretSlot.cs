using DG.Tweening;
using UnityEngine;

public class TurretSlot : MonoBehaviour, IClickable
{
    [SerializeField] int slotIndex;
    [SerializeField] Collider _collider;
    public bool isFull = false;

    [SerializeField] float slotJumpPower = 5f;
    [SerializeField] float splineJumpPower = 10f;

    float animDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;
    Turret currentTurret;

    public Turret CurrentTurret => currentTurret;
    public int SlotIndex => slotIndex;

    public void Place(Turret turret, bool isCompacting = false)
    {
        if (isFull) return;
        isFull = true;
        _collider.enabled = false;

        turret.transform.DOKill();
        turret.transform.SetParent(transform);
        currentTurret = turret;

        if (isCompacting)
            PlayCompactingAnimation(turret);
        else
            PlayPlacementAnimation(turret);
    }

    void PlayCompactingAnimation(Turret turret)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(
            turret.transform
                .DOLocalJump(Vector3.zero, slotJumpPower, 1, animDuration)
                .SetEase(Ease.OutCubic));

        seq.Join(
            turret.transform.DOPunchScale(
                turret.transform.localScale * animDuration,
                animDuration * 0.5f,
                vibrato: 1,
                elasticity: 0.5f));

        seq.OnComplete(() => _collider.enabled = true);
    }

    void PlayPlacementAnimation(Turret turret)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(
            turret.transform
                .DOLocalJump(Vector3.zero, splineJumpPower, 1, animDuration)
                .SetEase(Ease.OutCubic)
        );

        seq.Join(
            turret.transform.DOLocalRotate(
                new Vector3(0, 360, 0),
                animDuration
            )
        );

        seq.OnComplete(() => _collider.enabled = true);
    }

    public void ClearState()
    {
        if (currentTurret == null) return;
        isFull = false;
        _collider.enabled = false;

        currentTurret.transform.DOKill();
        currentTurret.transform.SetParent(null);
        currentTurret = null;
    }
    public void ClearForCompact()
    {
        if (currentTurret == null) return;
        isFull = false;
        _collider.enabled = false;
        currentTurret.transform.DOKill();
        currentTurret = null;
    }

    public void OnClick()
    {
        if (!isFull) return;
        if (PlatePoolManager.Instance.CurrentPlate == 0)
        {
            GameEvent.TriggerPlateCountEmpty();
            return;
        }

        Turret turret = currentTurret;

        bool hasLink = turret.HasLink;
        int requiredPlates = hasLink ? 2 : 1;
        if (!TurretManager.Instance.HasFreePlates(requiredPlates)) return;

        if (hasLink)
        {
            TurretSlot linkedSlot = TurretSlotManager.Instance.GetSlotOf(turret.LinkedTurret);
            if (linkedSlot != null)
            {
                linkedSlot.ClearState();
            }
        }

        ClearState();
        TurretSlotManager.Instance.CompactSlots();
        turret.SendToSplineWithLink();
    }
}
