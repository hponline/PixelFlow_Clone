using UnityEngine;

[CreateAssetMenu(fileName = "TileSO", menuName = "Scriptable Objects/TileSO")]
public class TileDataSO : ScriptableObject
{
    public TileType type;
    public GameObject prefab;

    [Header("Level Creator Tool")]
    [Tooltip("Level Creator Tool'un texture'daki pikselleri bu tile'a eþlemek için kullandýðý referans renk.")]
    public Color sampleColor = Color.white;
}
