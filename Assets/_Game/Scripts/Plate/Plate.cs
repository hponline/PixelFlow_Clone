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

    Vector3 startLocalPos;
    public Quaternion startLocalRot;
    Quaternion visualStartLocalRot;
    Vector3 visualFollowEuler = new Vector3(90, 90, 0);

    Tween moveTween;
    bool isReturning;

    private void Awake()
    {
        transform.GetLocalPositionAndRotation(out startLocalPos, out startLocalRot);
        visualStartLocalRot = visual.localRotation;
    }

    public void Init(SplineComputer spline, float speed, Turret turret, PlatePoolManager pool, Action onComplete = null)
    {
        KillTween();
        isReturning = false;

        platePool = pool;
        onMountComplete = onComplete;
        gameObject.SetActive(true);

        moveTween = transform.DOLocalMove(TurretManager.Instance.GetSplinePosition(), animDuration)
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
        visual.localRotation = Quaternion.Euler(visualFollowEuler);
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

    void ResetPose()
    {
        transform.SetLocalPositionAndRotation(startLocalPos, startLocalRot);
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
        if (isReturning) return;
        isReturning = true;

        follower.enabled = false;
        follower.onEndReached -= OnEnd;
        follower.spline = null;

        KillTween();

        moveTween = DOTween.Sequence()
            .Join(transform.DOLocalMove(startLocalPos, animDuration).SetEase(Ease.InOutQuad))
            .Join(transform.DOLocalRotateQuaternion(startLocalRot, animDuration).SetEase(Ease.InQuad))
            .SetLink(gameObject)
                    .OnComplete(() =>
                    {
                        ResetPose();
                        visual.localRotation = visualStartLocalRot;
                        platePool.ReturnPlate(this);
                    });
    }

    void KillTween()
    {
        moveTween?.Kill();
        moveTween = null;
    }

    void OnDisable()
    {
        KillTween();
        if (follower != null) follower.onEndReached -= OnEnd;
        UnsubscribeTurret();
    }

    void OnTurretStateChanged(TurretState prev, TurretState next)
    {
        if (next == TurretState.Despawning) return;
        if (currentTurret == null) return;

        UnsubscribeTurret();
        ReturnToStart();
    }
}