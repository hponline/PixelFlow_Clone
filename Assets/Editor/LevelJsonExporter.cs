using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// TileType[,] grid ve TurretDraft listesini runtime'ýn okuduðu LevelData JSON
/// formatýna çevirip diske yazar. Editor-only.
/// </summary>
public static class LevelJsonExporter
{
    /// <param name="grid">TextureGridSampler çýktýsý.</param>
    /// <param name="turrets">TurretDistributionPlanner çýktýsý, linkleri kullanýcý tarafýndan düzenlenmiþ.</param>
    /// <param name="folderPath">Örn. "Assets/_Game/Levels".</param>
    /// <param name="fileName">Uzantýsýz dosya adý, örn. "level_01".</param>
    /// <returns>Yazýlan dosyanýn tam yolu.</returns>
    public static string Export(TileType[,] grid, List<TurretDraft> turrets, string folderPath, string fileName)
    {
        if (grid == null)
            throw new ArgumentNullException(nameof(grid), "Grid boþ, önce TextureGridSampler.Sample çaðrýlmalý.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Dosya adý boþ olamaz.");

        LevelData levelData = BuildLevelData(grid, turrets);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fullPath = Path.Combine(folderPath, fileName + ".json");
        string json = JsonUtility.ToJson(levelData, true);

        File.WriteAllText(fullPath, json);
        AssetDatabase.Refresh();

        return fullPath;
    }

    static LevelData BuildLevelData(TileType[,] grid, List<TurretDraft> turrets)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int[] tiles = new int[width * height];

        // LevelManager.GenerateLevel ile ayný indexleme: index = y * width + x
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = z * width + x;
                tiles[index] = (int)grid[x, z];
            }
        }

        TurretLinkData[] turretData;
        if (turrets == null || turrets.Count == 0)
        {
            turretData = Array.Empty<TurretLinkData>();
        }
        else
        {
            turretData = new TurretLinkData[turrets.Count];
            for (int i = 0; i < turrets.Count; i++)
            {
                var draft = turrets[i];
                turretData[i] = new TurretLinkData
                {
                    id = draft.Id,
                    color = (int)draft.Color, // TileType ve ColorType deðerleri birebir eþleþiyor
                    ammo = draft.Ammo,
                    linkedTo = draft.LinkedTo
                };
            }
        }

        return new LevelData
        {
            width = width,
            height = height,
            tiles = tiles,
            turrets = turretData
        };
    }
}