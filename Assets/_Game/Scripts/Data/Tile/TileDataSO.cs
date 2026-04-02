using UnityEngine;

[CreateAssetMenu(fileName = "TileSO", menuName = "Scriptable Objects/TileSO")]
public class TileDataSO : ScriptableObject
{
    public TileType type;
    public GameObject prefab;
}
