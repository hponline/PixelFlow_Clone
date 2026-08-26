using DG.Tweening;
using Dreamteck.Splines;
using System;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField] public SplineFollower follower;
    [SerializeField] private Transform mountPoint;
    [SerializeField] private Transform visual;

    PlatePoolManager platePool;
    Turret currentTurret;
    Action onMountComplete;

    readonly float animDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;
    Vector3 startPos = Vector3.zero;
    Quaternion startRot = Quaternion.identity;

    private void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public void Init(SplineComputer spline, float speed, Turret turret, PlatePoolManager pool, Action onComplete = null)
    {
        platePool = pool;
        onMountComplete = onComplete;
        gameObject.SetActive(true);
        DOTween.Kill(transform);

        transform.DOMove(TurretManager.Instance.GetSplinePosition(), animDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => StartFollowing(spline, speed));

        turret.SendToPlate(this, mountPoint, () => MountTurret(turret));
    }

    private void MountTurret(Turret turret)
    {
        currentTurret = turret;
        currentTurret.OnStateChanged += OnTurretStateChanged;
        onMountComplete?.Invoke();
        onMountComplete = null;
    }

    private void StartFollowing(SplineComputer spline, float speed)
    {
        visual.localRotation = Quaternion.Euler(0, 90, 90);
        follower.spline = spline;
        follower.SetPercent(0);
        follower.followSpeed = speed;
        follower.onEndReached += OnEnd;
        follower.enabled = true;
    }

    private void OnEnd(double percent)
    {
        follower.onEndReached -= OnEnd;

        if (currentTurret != null)
        {
            currentTurret.OnStateChanged -= OnTurretStateChanged;
            currentTurret.transform.SetParent(null);
            currentTurret.SendToSlot();
            UnsubscribeTurret();
        }

        ReturnToStart();
    }

    public void RecallPlate()
    {
        UnsubscribeTurret();
        ReturnToStart();
    }
    private void UnsubscribeTurret()
    {
        if (currentTurret == null) return;
        currentTurret.OnStateChanged -= OnTurretStateChanged;
        currentTurret = null;
    }

    void ReturnToStart()
    {
        follower.enabled = false;
        follower.onEndReached -= OnEnd;
        follower.spline = null;

        transform.DOMove(startPos, animDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                visual.rotation = startRot;
                platePool.ReturnPlate(this);
            });
    }

    void OnTurretStateChanged(TurretState prev, TurretState next)
    {
        if (next == TurretState.Despawning) return;
        if (currentTurret == null) return;

        UnsubscribeTurret();
        ReturnToStart();
    }
}