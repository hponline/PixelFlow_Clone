using DG.Tweening;
using Dreamteck.Splines;
using Lean.Pool;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField] private SplineFollower follower;
    [SerializeField] private Transform mountPoint;

    PlatePool platePool;
    Turret currentTurret;
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
        transform.DOMove(splineStartPos, 0.4f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                transform.localRotation = Quaternion.Euler(0, 90, 0);
                transform.localScale = Vector3.one;

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
        currentTurret = turret;
        turret.transform.SetParent(mountPoint);
        turret.transform.localPosition = Vector3.zero;
        turret.transform.localRotation = Quaternion.identity;
        turret.enabled = true;
    }

    private void OnEnd(double percent)
    {
        follower.onEndReached -= OnEnd;

        if(currentTurret != null)
        {
            currentTurret.transform.SetParent(null);
            //LeanPool.Despawn(currentTurret); // þimdilik despawn - turret için ayrý yer yapýlacak
            currentTurret.SendToSlot();
            currentTurret = null;
        }

        ReturnToStart();
    }

    public void RecallPlate()
    {
        if (currentTurret != null)
        {
            //currentTurret.transform.SetParent(null);
            //currentTurret.SendToSlot();
            //LeanPool.Despawn(currentTurret);  // Turret Slota gönder
            currentTurret = null;
        }
        ReturnToStart();
    }

    void ReturnToStart()
    {
        follower.enabled = false;
        follower.onEndReached -= OnEnd;
        follower.spline = null;

        transform.DOMove(startPos, 0.4f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                transform.rotation = startRot;
                platePool.ReturnPlate(this);
            });
    }
}