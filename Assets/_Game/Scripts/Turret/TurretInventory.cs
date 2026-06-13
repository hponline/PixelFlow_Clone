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

            var link = ray[i].GetComponent<TurretLink>();
            if (link == null || !link.HasLink)
            {
                turret.SetClickable(i == 0);
                continue;
            }

            bool isFirst = i == 0;
            bool linkValid = LinkValidator.IsLinkValid(
                ray[i],
                link.LinkedTurret.gameObject,
                rays);

            turret.SetClickable(isFirst && linkValid);
        }
    }


    public Turret GetNeighbor(GameObject turretObj)
    {
        var (rayIndex, index) = GetPosition(turretObj);
        if (rayIndex == -1) return null;

        // Arkasý — ayný ray, bir sonraki index
        if (index + 1 < rays[rayIndex].Count)
            if (rays[rayIndex][index + 1].TryGetComponent<Turret>(out var behind))
                return behind;

        // Yaný — komþu ray, ayný index
        int neighborRay = (rayIndex + 1) % rays.Count;
        if (index < rays[neighborRay].Count)
            if (rays[neighborRay][index].TryGetComponent<Turret>(out var beside))
                return beside;

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
