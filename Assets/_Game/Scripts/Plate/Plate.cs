using DG.Tweening;
using Dreamteck.Splines;
using Lean.Pool;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField] private SplineFollower follower;
    [SerializeField] private Transform mountPoint;

    PlatePool platePool;
    Turret curretTurret;
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

        Vector3 splineStartPos = (Vector3)spline.GetPoint(0).position;
        transform.DOMove(splineStartPos, 1f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {

                follower.spline = spline;
                follower.followSpeed = speed;
                follower.SetPercent(0);
                follower.enabled = true;
                follower.onEndReached += OnEnd;

                MountTurret(turret);
            });
    }

    private void MountTurret(Turret turret) // turret üzerinde mermi varken plate ile beraber dönüyor- bug
    {
        turret.SetPlate(this);
        curretTurret = turret;
        turret.transform.SetParent(mountPoint);
        turret.transform.localPosition = Vector3.zero;
        turret.transform.localRotation = Quaternion.identity;
        turret.enabled = true;
    }

    public void RecallPlate() // Turret mermi bitince plate geri dönüyor
    {
        if (curretTurret != null)
        {
            curretTurret.transform.SetParent(null);
            LeanPool.Despawn(curretTurret);
            curretTurret = null;
        }
        ReturnToStart();
    }

    private void OnEnd(double percent) // plate - spline bitince döner
    {
        ReturnToStart();
    }
    void ReturnToStart() // scene sahnesinde geri dönerken spline üzerinde kalýyor. 
    {
        follower.enabled = false;
        follower.onEndReached -= OnEnd;
        follower.spline = null;

        transform.DOMove(startPos, 1f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                transform.SetLocalPositionAndRotation(startPos, startRot); // baþlangýç pos-rot a geri dönecek
                platePool.ReturnPlate(this);
            });
    }
}