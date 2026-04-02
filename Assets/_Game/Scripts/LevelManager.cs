using NaughtyAttributes;
using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public TileDatabase database;
    public float tileSize = 0.3f;

    public Transform blockContainer;
    public Transform gridPosition;

    public LevelList levelList;
    [SerializeField] int currentLevel = 0;
    [SerializeField] int levelWidth = 5;
    [SerializeField] int levelHeight = 5;


    void Start()
    {
        LoadLevel(currentLevel);

        //GenerateLevel(RandomGenerateLevel(levelWidth, levelHeight));
    }

    [Button]
    public void GenerateLevelButton()
    {
        GenerateLevel(RandomGenerateLevel(levelWidth, levelHeight));
    }

    public void LoadLevel(int levelIndex)
    {
        var lvl = Mathf.Clamp(levelIndex, 0, levelList.levels.Length);
        var json = levelList.levels[lvl];

        LevelData levelData = JsonUtility.FromJson<LevelData>(json.text);

        GenerateLevel(levelData);
    }

    void GenerateLevel(LevelData levelData)
    {
        if (levelData == null || levelData.tiles == null) return;

        int expected = levelData.width * levelData.height;
        if (levelData.tiles.Length != expected)
        {
            Debug.LogError($"Tile eþleþmiyor. total tile: {levelData.tiles.Length} olmasý gereken tile: {expected}");
            return;
        }

        for (int y = 0; y < levelData.height; y++)
        {
            for (int x = 0; x < levelData.width; x++)
            {
                int index = y * levelData.width + x;

                int value = levelData.tiles[index];

                if (!Enum.IsDefined(typeof(TileType), value))
                {
                    Debug.Log("Tile eþleþmiyor");
                    continue;
                }

                TileType type = (TileType)value;

                GameObject prefab = database.GetPrefab(type);

                Vector3 pos = gridPosition.position + new Vector3(x * tileSize, 0, y * tileSize);

                Instantiate(prefab, pos, Quaternion.identity, blockContainer);
            }
        }
    }

    // Random GenerateLevel
    LevelData RandomGenerateLevel(int width, int height)
    {
        LevelData data = new LevelData();
        data.width = width;
        data.height = height;
        data.tiles = new int[width * height];

        for (int i = 0; i < data.tiles.Length; i++)
        {
            data.tiles[i] = UnityEngine.Random.Range(0, 3); // Block Enum/Prefab
        }

        return data;
    }
}