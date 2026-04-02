using UnityEngine;

[CreateAssetMenu(fileName = "TurretSOData", menuName = "Scriptable Objects/TurretSOData")]
public class TurretSOData : ScriptableObject
{
    public ColorType TurretColor;
    public LayerMask blockLayerMask;
    public int magazine = 20;
    public float projectileSpeed = .3f;
    public float raycastRange = 10;
}
