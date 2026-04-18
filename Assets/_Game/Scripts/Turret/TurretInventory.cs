using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class TurretInventory : MonoBehaviour
{
    public static TurretInventory Instance;

    [SerializeField] float slideDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;
    [SerializeField] private Ease slideEaseType = Ease.OutBack;
    [SerializeField] private Vector3 spacingOffset = new Vector3(0, 0, -2.5f);
    [SerializeField] private Transform[] rayStartPoints;

    private List<List<GameObject>> rays = new();

    private void Awake()
    {
        Instance = this;

        foreach (var _ in rayStartPoints)
            rays.Add(new List<GameObject>());
    }

    public void AddTurret(GameObject turretObj)
    {
        int randomRay = Random.Range(0, rays.Count); // Raylara rastgele deðil sýra ile ekle
        rays[randomRay].Add(turretObj);
        RefreshRay(randomRay);
    }

    public void RemoveTurret(GameObject turretObj)
    {
        for (int i = 0; i < rays.Count; i++)
        {
            if (rays[i].Remove(turretObj))
            {
                RefreshRay(i);
                UpdateClickable(i);
                return;
            }
        }
    }

    void RefreshRay(int rayIndex)
    {
        var ray = rays[rayIndex];
        Transform startPoint = rayStartPoints[rayIndex];

        for (int i = 0; i < ray.Count; i++)
        {
            Transform t = ray[i].transform;
            Vector3 targetPos = startPoint.position + spacingOffset * i;

            t.DOKill();
            t.DOMove(targetPos, slideDuration).SetEase(slideEaseType);
            t.DORotateQuaternion(startPoint.rotation, slideDuration);
        }

        UpdateClickable(rayIndex);
    }

    void UpdateClickable(int rayIndex)
    {
        var ray = rays[rayIndex];
        for (int i = 0; i < ray.Count; i++)
        {
            if (ray[i].TryGetComponent<Turret>(out var t))
                t.SetClickable(i == 0);
        }
    }
}
