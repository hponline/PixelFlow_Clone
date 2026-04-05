using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField] private SplineFollower follower;
    [SerializeField] private Transform mountPoint;
    [SerializeField] private Transform visual;

    PlatePool platePool;
    Turret currentTurret;
    float animDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;
    Vector3 startPos = Vector3.zero;
    Quaternion startRot = Quaternion.identity;

    private void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public void Init(SplineComputer spline, float speed, Turret turret, PlatePool pool)
    {
        platePool = pool;
        gameObject.SetActive(true);
        DOTween.Kill(transform);

        transform.DOMove(TurretManager.Instance.GetSplinePosition(), animDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                visual.transform.localRotation = Quaternion.Euler(0, 90, 90);
                currentTurret = turret;

                follower.spline = spline;
                follower.SetPercent(0);
                follower.onEndReached += OnEnd;
                follower.followSpeed = speed;
                follower.enabled = true;
            });
                turret.SendToPlate(this, mountPoint);
    }


    private void OnEnd(double percent)
    {
        follower.onEndReached -= OnEnd;

        if (currentTurret != null)
        {
            currentTurret.transform.SetParent(null);
            currentTurret.SendToSlot();
            currentTurret = null;
        }

        ReturnToStart();
    }

    public void RecallPlate()
    {
        if (currentTurret != null)
            currentTurret = null;

        ReturnToStart();
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
                visual.transform.rotation = startRot;
                platePool.ReturnPlate(this);
            });
    }
}