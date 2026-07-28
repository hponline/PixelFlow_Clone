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

    [SerializeField] Block[,] grid;
    public Grid gridComponent;
    public int turretMaxAmmo;

    [Header("Turret Links")]
    [Range(0f, 1f)]
    [SerializeField] float linkChance = 0.5f;

    GridContext gridContext;
    Dictionary<int, int> tileCounts;
    int remainingBlocks;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Block.OnBlockDestroyed += HandleBlockDestroyed;
        GameEvent.OnSlotFull += HandleSlotsFull;
    }
    private void OnDisable()
    {
        Block.OnBlockDestroyed -= HandleBlockDestroyed;
        GameEvent.OnSlotFull -= HandleSlotsFull;
    }

    void Start()
    {
        //LoadLevel(DataManager.Instance.currentLevel);  
        LoadLevel(currentLevel);  
    }

    void CompleteLevel()
    {
        int nextLvl = PlayerPrefs.GetInt(GameTags.PlayerPrefsKeys.CURRENT_LEVEL, 0) +1;
        DataManager.Instance.SaveLevel(nextLvl);

        Debug.Log("Level Win");
        GameEvent.TriggerLevelCompleted();
    }

    void LoseLevel()
    {
        Debug.Log("Level Lose — slotlar dolu");
        GameEvent.TriggerLevelLose();
    }

    void HandleSlotsFull()
    {
        LoseLevel();
    }

    void HandleBlockDestroyed(Block block)
    {
        grid[block.gridX, block.gridZ] = null;
        remainingBlocks--;

        if (remainingBlocks <= 0)
            CompleteLevel();
    }

    public void NextLevel()
    {
        int nextLevel = DataManager.Instance.NextLevel;
        LoadLevel(nextLevel);

        GameEvent.TriggerLevelChanged(nextLevel);
        UIManager.Instance.ClosePanel();

        DataManager.Instance.SaveGame();
        Debug.Log("Level değişti Şimdiki level: " + nextLevel);
    }

    public void LoadLevel(int levelIndex)
    {
        var lvl = Mathf.Clamp(levelIndex, 0, levelList.levels.Length - 1);
        var json = levelList.levels[lvl];

        LevelData levelData = JsonUtility.FromJson<LevelData>(json.text);

        tileCounts = CountTiles(levelData.tiles);
        GetTotalAmmo();
        GenerateLevel(levelData);

        Debug.Log("Level yüklendi: " + lvl);
    }

    //void ClearLevel() // Her level yüklediginde sahne reseti
    //{
    //    if (grid != null)
    //    {
    //        for (int y = 0; y < grid.GetLength(1); y++)
    //        {
    //            for (int x = 0; x < grid.GetLength(0); x++)
    //            {
    //                if (grid[x, y] != null)
    //                    LeanPool.Despawn(grid[x, y].gameObject);
    //            }
    //        }
    //    }
    //    grid = null;

    //    TurretManager.Instance.ClearTurrets(); // turret'lar için aynı mantık, ayrı method gerekir
    //}

    void GenerateLevel(LevelData levelData)
    {
        if (levelData == null || levelData.tiles == null) return;
        int expected = levelData.width * levelData.height;
        if (levelData.tiles.Length != expected) return;

        grid = new Block[levelData.width, levelData.height];
        remainingBlocks = 0;

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
                remainingBlocks++;
            }
        }
        gridContext = new GridContext
        {
            grid = grid,
            width = levelData.width,
            height = levelData.height,
        };

        TurretManager.Instance.SetGridContext(gridContext);

        if (levelData.turrets != null && levelData.turrets.Length > 0)
            SpawnTurretsFromData(levelData.turrets);
        else
            DistributeTurrets();
    }

    void SpawnTurretsFromData(TurretLinkData[] turretDataArray)
    {
        // id → Turret instance map
        Dictionary<int, Turret> spawnedMap = new Dictionary<int, Turret>();

        foreach (var data in turretDataArray)
        {
            ColorType color = (ColorType)data.color;
            Turret turret = TurretManager.Instance.SpawnTurretWithAmmo(color, data.ammo);
            spawnedMap[data.id] = turret;
        }

        // Link pass — her iki taraf spawn olduktan sonra
        foreach (var data in turretDataArray)
        {
            if (data.linkedTo == -1) continue;
            if (!spawnedMap.TryGetValue(data.id, out var turret)) continue;
            if (!spawnedMap.TryGetValue(data.linkedTo, out var target)) continue;

            var link = turret.GetComponent<TurretLink>();
            if (link == null || link.HasLink) continue;

            link.Link(target);
        }
    }

    void DistributeTurrets() // Mermi dağıtım
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
        foreach (var turret in turrets)
        {
            var link = turret.GetComponent<TurretLink>();
            if (link == null || link.HasLink) continue;

            Turret neighbor = TurretInventory.Instance.GetNeighbor(turret.gameObject);
            if (neighbor == null) continue;

            var neighborLink = neighbor.GetComponent<TurretLink>();
            if (neighborLink == null || neighborLink.HasLink) continue;
            if (UnityEngine.Random.value <= linkChance)
                link.Link(neighbor);
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