using UnityEngine;

[CreateAssetMenu(fileName = "TurretSOData", menuName = "Scriptable Objects/TurretSOData")]
public class TurretSOData : ScriptableObject
{
    public TileType TurretColor;
    public LayerMask blockLayerMask;
    public float projectileSpeed = .3f;
}
