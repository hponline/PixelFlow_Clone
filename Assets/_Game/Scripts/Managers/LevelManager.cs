using Lean.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] TransitionScreen transitionScreen;

    public TileDatabase database;
    public Transform blockContainer;

    public LevelList levelList;
    [SerializeField] int _TestCurrentLevel = 0;
    [SerializeField] int maxLevel = 0;
    public int MaxLevel => maxLevel;

    [SerializeField] Block[,] grid;
    public Grid gridComponent;

    [Header("Play Area (Ortalama ve Ölçekleme)")]
    [SerializeField] Transform playAreaMin;
    [SerializeField] Transform playAreaMax;
    [Tooltip("Block prefablarının tasarlandığı referans hücre boyutu (Grid component'inin şu anki Cell Size'ı, örn. 0.33).")]
    [SerializeField] float referenceCellSize = 0.33f;

    float currentVisualScale = 1f;

    GridContext gridContext;
    int remainingBlocks;

    [Header("Test")]
    [SerializeField] bool Test = false;

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
        if (Test)
            LoadLevel(_TestCurrentLevel);  
        else    
            LoadLevel(DataManager.Instance.currentLevel);

        CheckMaxLevel();
    }

    void CompleteLevel()
    {
        int completedLvl = DataManager.Instance.currentLevel;

        DataManager.Instance.SaveLevel(completedLvl);
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
        transitionScreen.ToggleTransitionScreen();

        LoadLevel(nextLevel);

        GameEvent.TriggerLevelChanged(nextLevel);
        UIManager.Instance.ClosePanel();
        
        DataManager.Instance.SaveLevel(nextLevel);
    }

    public void LoadLevel(int levelIndex)
    {
        var lvl = Mathf.Clamp(levelIndex, 0, levelList.levels.Length - 1);
        var json = levelList.levels[lvl];

        LevelData levelData = JsonUtility.FromJson<LevelData>(json.text);
        GenerateLevel(levelData);
        Debug.Log($"Level yüklendi: {lvl} Şimdiki level: {levelList.levels[lvl].name}");
    }

    void CheckMaxLevel()
    {
        maxLevel = levelList.levels.Length;
    }

    /// <summary>
    /// Play area sınırları içine sığacak şekilde grid'in hücre boyutunu hesaplar,
    /// origin'i level'in merkezi Play Area'nın merkeziyle çakışacak şekilde ayarlar.
    /// Block'ların görsel scale'i için kullanılacak oranı currentVisualScale'e yazar.
    /// </summary>

    void ConfigureGrid(int width, int height)
    {
        if (playAreaMin == null || playAreaMax == null)
        {
            Debug.LogWarning("PlayAreaMin/PlayAreaMax atanmamış, grid ortalama/ölçekleme atlanıyor.");
            currentVisualScale = 1f;
            return;
        }

        float playAreaWidth = playAreaMax.position.x - playAreaMin.position.x;
        float playAreaHeight = playAreaMax.position.z - playAreaMin.position.z;

        float cellSize = Mathf.Min(playAreaWidth / width, playAreaHeight / height);

        float totalGridWidth = width * cellSize;
        float totalGridHeight = height * cellSize;

        float offsetX = (playAreaWidth - totalGridWidth) / 2f;
        float offsetZ = (playAreaHeight - totalGridHeight) / 2f;

        Vector3 startPos = playAreaMin.position + new Vector3(offsetX, 0f, offsetZ);

        Vector3 cellSizeVector = gridComponent.cellSize;
        cellSizeVector.x = cellSize;
        cellSizeVector.z = cellSize;
        gridComponent.cellSize = cellSizeVector;

        gridComponent.transform.position = startPos;

        currentVisualScale = cellSize / referenceCellSize;
    }

    void GenerateLevel(LevelData levelData)
    {
        if (levelData == null || levelData.tiles == null) return;
        int expected = levelData.width * levelData.height;
        if (levelData.tiles.Length != expected) return;

        ConfigureGrid(levelData.width, levelData.height);

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
                spawned.transform.localScale = prefab.transform.localScale * currentVisualScale;
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
    }

    void SpawnTurretsFromData(TurretLinkData[] turretDataArray)
    {
        // id → Turret instance map
        Dictionary<int, Turret> spawnedMap = new Dictionary<int, Turret>();

        foreach (var data in turretDataArray)
        {
            TileType color = (TileType)data.color;
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
}