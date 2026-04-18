using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDataSO", menuName = "Scriptable Objects/ProjectileDataSO")]
public class ProjectileData : ScriptableObject
{
    public GameObject prefab;
    public int preloadCount;
}
