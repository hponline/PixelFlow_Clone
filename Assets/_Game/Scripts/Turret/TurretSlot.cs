using DG.Tweening;
using UnityEngine;

public class TurretSlot : MonoBehaviour, IClickable
{
    public bool isFull = false;
    Turret currentTurret;
    float animDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;

    public void Place(Turret turret)
    {
        if (isFull) return;
        isFull = true;
        turret.transform.SetParent(transform);

        Sequence seq = DOTween.Sequence();
        seq.Append(turret.transform.DOLocalMove(Vector3.zero, animDuration));
        seq.Join(turret.transform.DOLocalRotate(new Vector3(0, 360, 0), animDuration));
        seq.OnComplete(() =>
        {
            currentTurret = turret;
        });
    }

    public void Clear()
    {
        if (currentTurret == null) return;
        currentTurret = null;
        isFull = false;
    }

    public void OnClick()
    {
        if (!isFull) return;
        Turret turret = currentTurret;
        Clear();
        TurretManager.Instance.TurretSendToSpline(turret);
    }
}
