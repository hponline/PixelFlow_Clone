using DG.Tweening;
using System;
using UnityEngine;

public class Block : MonoBehaviour
{
    public static event Action<Block> OnBlockDestroyed;

    public TileType blockColor;
    public bool isShot = false;
    public int ring;

    public int gridX;
    public int gridZ;

    public void DestroyBlock()
    {
        transform.DOScale(0, GameTags.Animation.DOTWEEN_BLOCK_DEAD_DURATION).SetEase(Ease.InBack).OnComplete(() =>
        {
            OnBlockDestroyed?.Invoke(this);
            Lean.Pool.LeanPool.Despawn(gameObject);
        });
    }
}
