using UnityEngine;

[CreateAssetMenu(fileName = "TurretSOData", menuName = "Scriptable Objects/TurretSOData")]
public class TurretSOData : ScriptableObject
{
    public TileType TurretColor;
    public LayerMask blockLayerMask;
    [Range(0, 1)] public float projectileSpeed = .4f;
}
