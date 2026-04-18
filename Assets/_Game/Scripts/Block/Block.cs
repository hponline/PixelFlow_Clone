using DG.Tweening;
using UnityEngine;

public class Block : MonoBehaviour
{
    public ColorType blockColor;
    public bool isShot = false;
    public int ring;

    public int gridX;
    public int gridZ;

    public void DestroyBlock()
    {
        transform.DOScale(0, GameTags.Animation.DOTWEEN_ANIM_DURATION).SetEase(Ease.InBack).OnComplete(() =>
        {
            Lean.Pool.LeanPool.Despawn(gameObject);
            LevelManager.Instance.grid[gridX, gridZ] = null;
        });
    }
}
