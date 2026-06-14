using DG.Tweening;
using Lean.Pool;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Turret))]
public class TurretShooter : MonoBehaviour
{
    [SerializeField] Turret turret;
    GridContext gridContext;

    private void Awake()
    {
        turret = GetComponent<Turret>();
    }

    public void Init(GridContext context) => gridContext = context;


    public void TryShoot()
    {
        if (turret.projectileMagazine <= 0) return;

        Vector2Int gridPos = WorldToGrid(transform.position);
        GridSide side = GetGridSide(gridPos);
        if (side == GridSide.None) return; // Köþede veya grid içinde, ateþ etme

        Block target = FindTargetBlock(gridPos, side);
        if (target == null) return;
        if (target.blockColor != turret.turretSO.TurretColor) return;
        if (target.isShot) return;

        SpawnProjectile(target.transform.position, target);
    }

    void SpawnProjectile(Vector3 targetPos, Block block)
    {
        turret.Shot();
        block.isShot = true;
        var obj = LeanPool.Spawn(turret.projectilePrefab, turret.firePoint.position, Quaternion.identity);
        obj.transform.DOMove(targetPos, turret.turretSO.projectileSpeed)
            .OnComplete(() =>
            {
                LeanPool.Despawn(obj);
                block.DestroyBlock();
            });
    }

    #region Grid iþlemleri
    GridSide GetGridSide(Vector2Int gridPos)
    {
        bool onBottom = gridPos.y < 0;
        bool onTop = gridPos.y >= gridContext.height;
        bool onLeft = gridPos.x < 0;
        bool onRight = gridPos.x >= gridContext.width;

        // Köþedeyse None döndür, ateþ etme
        if ((onBottom || onTop) && (onLeft || onRight)) return GridSide.None;

        if (onBottom) return GridSide.Bottom;
        if (onTop) return GridSide.Top;
        if (onLeft) return GridSide.Left;
        if (onRight) return GridSide.Right;

        return GridSide.None; // Grid'in içinde, ateþ etme
    }

    Block FindTargetBlock(Vector2Int gridPos, GridSide side)
    {
        switch (side)
        {
            case GridSide.Bottom:
                // Turret'in X hizasýnda, aþaðýdan yukarý tara
                for (int y = 0; y < gridContext.height; y++)
                    if (gridContext.grid[gridPos.x, y] != null) return gridContext.grid[gridPos.x, y];
                break;

            case GridSide.Top:
                // Turret'in X hizasýnda, yukarýdan aþaðý tara
                for (int y = gridContext.height - 1; y >= 0; y--)
                    if (gridContext.grid[gridPos.x, y] != null) return gridContext.grid[gridPos.x, y];
                break;

            case GridSide.Left:
                // Turret'in Z hizasýnda, soldan saða tara
                for (int x = 0; x < gridContext.width; x++)
                    if (gridContext.grid[x, gridPos.y] != null) return gridContext.grid[x, gridPos.y];
                break;

            case GridSide.Right:
                // Turret'in Z hizasýnda, saðdan sola tara
                for (int x = gridContext.width - 1; x >= 0; x--)
                    if (gridContext.grid[x, gridPos.y] != null) return gridContext.grid[x, gridPos.y];
                break;
        }
        return null;
    }

    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3Int cell = LevelManager.Instance.gridComponent.WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.z);
    }

    #endregion
}
