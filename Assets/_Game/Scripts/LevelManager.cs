using Lean.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public TileDatabase database;
    public Transform blockContainer;

    public LevelData levelData;
    public LevelList levelList;
    [SerializeField] int currentLevel = 0;

    public int CurrentLevel => currentLevel;
    public Block[,] grid;
    public Grid gridComponent;
    public int turretMaxAmmo;

    [Header("Turret Links")]
    [Range(0f, 1f)]
    [SerializeField] float linkChance = 0.5f;

    GridContext gridContext;
    Dictionary<int, int> tileCounts;

    private void Awake() => Instance = this;

    void Start()
    {
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int levelIndex)
    {
        var lvl = Mathf.Clamp(levelIndex, 0, levelList.levels.Length - 1);
        var json = levelList.levels[lvl];

        LevelData levelData = JsonUtility.FromJson<LevelData>(json.text);

        tileCounts = CountTiles(levelData.tiles);
        GetTotalAmmo();
        GenerateLevel(levelData);
    }


    void GenerateLevel(LevelData levelData)
    {
        if (levelData == null || levelData.tiles == null) return;
        int expected = levelData.width * levelData.height;
        if (levelData.tiles.Length != expected) return;

        grid = new Block[levelData.width, levelData.height];

        for (int y = 0; y < levelData.height; y++)
        {
            for (int x = 0; x < levelData.width; x++)
            {
                int index = y * levelData.width + x;
                int value = levelData.tiles[index];

                if (!Enum.IsDefined(typeof(TileType), value))
                {
                    grid[x, y] = null;
                    continue;
                }

                TileType type = (TileType)value;
                if (type == TileType.None)
                {
                    grid[x, y] = null;
                    continue;
                }

                Vector3 pos = gridComponent.GetCellCenterWorld(new Vector3Int(x, 0, y));
                GameObject prefab = database.GetPrefab(type);
                GameObject spawned = LeanPool.Spawn(prefab, pos, Quaternion.identity, blockContainer);

                if (!spawned.TryGetComponent<Block>(out var _block)) continue;

                _block.ring = Mathf.Min(Mathf.Min(x, y), Mathf.Min(levelData.width - 1 - x, levelData.height - 1 - y));
                _block.gridX = x;
                _block.gridZ = y;
                grid[x, y] = _block;
            }
        }
        gridContext = new GridContext
        {
            grid = grid,
            width = levelData.width,
            height = levelData.height,
        };

        TurretManager.Instance.SetGridContext(gridContext);
        DistributeTurrets();
    }

    void DistributeTurrets() // Mermi daðýtým
    {
        List<Turret> spawnedTurrets = new List<Turret>();

        foreach (var entry in tileCounts)
        {
            if (entry.Key == (int)ColorType.None) continue;

            ColorType color = (ColorType)entry.Key;
            int remaining = entry.Value;
            while (remaining > 0)
            {
                int ammo = UnityEngine.Random.Range(1, Mathf.Min(40, remaining) + 1);
                remaining -= ammo;

                Turret turret = TurretManager.Instance.SpawnTurretWithAmmo(color, ammo);
                spawnedTurrets.Add(turret);
            }
        }

        LinkTurrets(spawnedTurrets);
    }

    void LinkTurrets(List<Turret> turrets)
    {
        ShuffleTurrets(turrets);

        for (int i = 0; i + 1 < turrets.Count; i += 2)
        {
            if (UnityEngine.Random.value <= linkChance)
                turrets[i].GetComponent<TurretLink>().Link(turrets[i + 1]);
        }
    }

    void ShuffleTurrets(List<Turret> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    Dictionary<int, int> CountTiles(int[] tiles)
    {
        Dictionary<int, int> counts = new Dictionary<int, int>();
        foreach (int tile in tiles)
        {
            if (!counts.ContainsKey(tile))
                counts[tile] = 0;

            counts[tile]++;
        }
        return counts;
    }

    void GetTotalAmmo()
    {
        turretMaxAmmo = 0;
        foreach (var tile in tileCounts)
        {
            if (tile.Key == (int)ColorType.None) continue;

            turretMaxAmmo += tile.Value;
        }
    }
}