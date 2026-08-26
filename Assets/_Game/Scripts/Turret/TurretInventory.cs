using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class TurretInventory : MonoBehaviour
{
    public static TurretInventory Instance;

    [SerializeField] UIIntroController uIIntroController;

    [SerializeField] float slideDuration = GameTags.Animation.DOTWEEN_ANIM_DURATION;
    [SerializeField] private Ease slideEaseType = Ease.OutBack;
    [SerializeField] private Vector3 spacingOffset = new Vector3(0, 0, -2.5f);
    [SerializeField] private Transform[] rayStartPoints;

    private List<List<GameObject>> rays = new();
    private List<List<Turret>> turretCache;
    private List<List<TurretHighlight>> highlightCache;
    private int currentRayIndex = 0;


    private void Awake()
    {
        Instance = this;

        foreach (var _ in rayStartPoints)
            rays.Add(new List<GameObject>());

    }
    private void Start()
    {
        CacheComponents();

    }

    public int CheckTotalTurrets()
    {
        int total = 0;

        for (int i = 0; i < rays.Count; i++)
        {
            for (int j = 0; j < rays[i].Count; j++)
            {
                total += 1;
            }
        }

        return total;
    }


    void CacheComponents()
    {
        turretCache = new List<List<Turret>>();
        highlightCache = new List<List<TurretHighlight>>();

        foreach (var ray in rays)
        {
            var turretRow = new List<Turret>();
            var highlightRow = new List<TurretHighlight>();

            foreach (var turretObj in ray)
            {
                turretRow.Add(turretObj.GetComponent<Turret>());
                highlightRow.Add(turretObj.GetComponentInChildren<TurretHighlight>());
            }

            turretCache.Add(turretRow);
            highlightCache.Add(highlightRow);
        };
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
                RestoreDefaultClickable();
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
        RefreshLinkedPartnerRays(rayIndex);
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
            bool linkedIsFirst = linkedIndex == 0;
            bool linkedBehind = linkedRay == rayIndex && linkedIndex == i + 1;
            bool linkValid = LinkValidator.IsLinkValid(rayIndex, i, linkedRay, linkedIndex);

            bool canClick =
                isFirst &&
                linkValid &&
                (linkedIsFirst || linkedBehind);

            turret.SetClickable(canClick);
        }
    }

    void RefreshLinkedPartnerRays(int rayIndex)
    {
        var ray = rays[rayIndex];
        HashSet<int> partnerRays = null;

        for (int i = 0; i < ray.Count; i++)
        {
            if (!ray[i].TryGetComponent<Turret>(out var turret)) continue;
            if (!turret.HasLink) continue;

            var (linkedRay, _) = GetPosition(turret.LinkedTurret.gameObject);
            if (linkedRay == -1 || linkedRay == rayIndex) continue;

            partnerRays ??= new HashSet<int>();
            partnerRays.Add(linkedRay);
        }

        if (partnerRays == null) return;

        foreach (var r in partnerRays)
            UpdateClickable(r);
    }

    public Turret GetNeighbor(GameObject turretObj)
    {
        var (rayIndex, itemIndex) = GetPosition(turretObj);
        if (rayIndex == -1) return null;

        // 8 yön — öncelik sýrasý: sað, sol, arka, ön, çaprazlar
        (int row, int column)[] directions =
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

        foreach (var (row, column) in directions)
        {
            int nr = rayIndex + row;
            int ni = itemIndex + column;

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

    public void SetAllHighlighted(bool active)
    {
        for (int i = 0; i < turretCache.Count; i++)
        {
            for (int j = 0; j < turretCache[i].Count; j++)
            {
                turretCache[i][j]?.SetClickable(active);
                highlightCache[i][j]?.SetHighlighted(active);
            }
        }

        if (!active)
            RestoreDefaultClickable();

        Debug.Log("Highlighted" + active);
    }

    public void ShowPanel(bool state)
    {
        uIIntroController.ShowPanel(state);
    }

    void RestoreDefaultClickable()
    {
        for (int i = 0; i < rays.Count; i++)
        {
            UpdateClickable(i);
        }
    }

    void Shuffle(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public void ShuffleButton()
    {
        List<GameObject> allObjects = new List<GameObject>();

        foreach (var ray in rays)
        {
            allObjects.AddRange(ray);
        }

        Shuffle(allObjects);

        int index = 0;

        for (int i = 0; i < rays.Count; i++)
        {
            int count = rays[i].Count;
            rays[i].Clear();

            for (int j = 0; j < count; j++)
            {
                rays[i].Add(allObjects[index]);
                index++;
            }

            RefreshRay(i);
        }
    }
}
