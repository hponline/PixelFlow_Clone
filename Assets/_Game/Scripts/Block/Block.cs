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

    [SerializeField, Range(0, 1)] float punchForce = 0.2f;

    public void DestroyBlock()
    {
        transform.DOPunchScale(Vector3.one * punchForce, GameTags.Animation.DOTWEEN_BLOCK_DEAD_DURATION).SetEase(Ease.InBack).OnComplete(() =>
        {
            OnBlockDestroyed?.Invoke(this);
            isShot = false;
            Lean.Pool.LeanPool.Despawn(gameObject);
        });
    }
}
