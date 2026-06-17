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
    private int currentRayIndex = 0;

    private void Awake()
    {
        Instance = this;

        foreach (var _ in rayStartPoints)
            rays.Add(new List<GameObject>());
    }

    public void AddTurret(GameObject turretObj)
    {
        rays[currentRayIndex].Add(turretObj);
        RefreshRay(currentRayIndex);
        currentRayIndex = (currentRayIndex + 1) % rays.Count;
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
            if (!ray[i].TryGetComponent<Turret>(out var turret)) continue;

            bool isFirst = i == 0;
            if (!turret.HasLink)
            {
                turret.SetClickable(isFirst);
                continue;
            }
            var (linkedRay, linkedIndex) = GetPosition(turret.LinkedTurret.gameObject);
            bool linkValid = LinkValidator.IsLinkValid(rayIndex, i, linkedRay, linkedIndex);

            turret.SetClickable(isFirst && linkValid);
        }
    }


    public Turret GetNeighbor(GameObject turretObj)
    {
        var (rayIndex, itemIndex) = GetPosition(turretObj);
        if (rayIndex == -1) return null;

        // 8 yön — öncelik sýrasý: sað, sol, arka, ön, çaprazlar
        (int dr, int di)[] directions =
        {
        ( 1,  0), // sað ray
        (-1,  0), // sol ray
        ( 0,  1), // arka (ayný ray, sonraki)
        ( 0, -1), // ön (ayný ray, önceki)
        ( 1,  1), // sað-arka çapraz
        (-1,  1), // sol-arka çapraz
        ( 1, -1), // sað-ön çapraz
        (-1, -1), // sol-ön çapraz
        };

        foreach (var (dr, di) in directions)
        {
            int nr = rayIndex + dr;
            int ni = itemIndex + di;

            if (nr < 0 || nr >= rays.Count) continue;
            if (ni < 0 || ni >= rays[nr].Count) continue;

            var neighbor = rays[nr][ni];
            if (neighbor == null) continue;

            var neighborLink = neighbor.GetComponent<TurretLink>();
            if (neighborLink != null && neighborLink.HasLink) continue; // zaten baðlý, atla

            if (neighbor.TryGetComponent<Turret>(out var turret))
                return turret;
        }

        return null;
    }

    public (int rayIndex, int index) GetPosition(GameObject turretObj)
    {
        for (int r = 0; r < rays.Count; r++)
            for (int i = 0; i < rays[r].Count; i++)
                if (rays[r][i] == turretObj) return (r, i);
        return (-1, -1);
    }
}
