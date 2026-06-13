using DG.Tweening;
using UnityEngine;

public class TurretSlot : MonoBehaviour, IClickable
{
    [SerializeField] int slotIndex;
    [SerializeField] Collider _collider;
    public bool isFull = false;

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

        Sequence seq = DOTween.Sequence();

        if (isCompacting)
        {
            seq.Append(turret.transform.DOLocalMove(Vector3.zero, animDuration).SetEase(Ease.OutCubic));
            seq.Join(turret.transform.DOPunchScale(turret.transform.localScale * animDuration, animDuration * 0.5f, vibrato: 1, elasticity: 0.5f));
        }
        else
        {
            seq.Append(turret.transform.DOLocalMove(Vector3.zero, animDuration));
            seq.Join(turret.transform.DOLocalRotate(new Vector3(0, 360, 0), animDuration));
        }

        seq.OnComplete(() =>
        {
            _collider.enabled = true;
        });
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

    public void OnClick()
    {
        if (!isFull) return;

        Turret turret = currentTurret;
        var link = turret.GetComponent<TurretLink>();
        bool hasLink = link != null && link.HasLink;

        int requiredPlates = hasLink ? 2 : 1;
        if (!TurretManager.Instance.HasFreePlates(requiredPlates)) return;

        // Linked turret'i slottan çýkar
        if (hasLink)
        {
            TurretSlot linkedSlot = TurretSlotManager.Instance.GetSlotOf(link.LinkedTurret);
            linkedSlot?.ClearState();
        }

        ClearState();
        TurretSlotManager.Instance.CompactSlots(slotIndex);
        turret.SendToSplineWithLink();
    }
}
