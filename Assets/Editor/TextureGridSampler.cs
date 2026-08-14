using System;
using UnityEngine;

/// <summary>
/// Editor-only, saf (state tutmayan) sýnýf.
/// Bir texture'ý verilen grid boyutuna göre örnekler, her hücrenin merkez pikselini
/// TileDataSO paletindeki en yakýn sampleColor'a eþleyerek TileType[,] üretir.
/// Runtime koduna baðýmlýlýðý yok, LevelCreatorWindow tarafýndan çaðrýlýr.
/// </summary>
public static class TextureGridSampler
{
    /// <summary>
    /// Texture'ý gridWidth x gridHeight hücreye böler, her hücrenin merkez pikselini
    /// palette içindeki en yakýn renkle eþleþtirir. Eþleþme colorTolerance'ý aþarsa
    /// TileType.None döner.
    /// </summary>
    /// <param name="texture">Read/Write Enabled açýk olmalý.</param>
    /// <param name="palette">TileType.None hariç, karþýlaþtýrýlacak tile paleti.</param>
    /// <param name="colorTolerance">0-255 skalasýnda, RGB mesafe eþiði. Düþük = katý eþleþme.</param>
    public static TileType[,] Sample(Texture2D texture, int gridWidth, int gridHeight, TileDataSO[] palette, float colorTolerance)
    {
        if (texture == null)
            throw new ArgumentNullException(nameof(texture), "Source texture atanmamýþ.");

        if (gridWidth <= 0 || gridHeight <= 0)
            throw new ArgumentException($"Grid boyutu geçersiz: {gridWidth}x{gridHeight}");

        if (palette == null || palette.Length == 0)
            throw new ArgumentException("TileDataSO paleti boþ. TileDatabase.tiles atanmýþ mý kontrol et.");

        if (!texture.isReadable)
            throw new InvalidOperationException(
                $"'{texture.name}' texture'ý okunamýyor. Import Settings'ten 'Read/Write Enabled' seçeneðini aç.");

        var result = new TileType[gridWidth, gridHeight];

        float cellWidth = texture.width / (float)gridWidth;
        float cellHeight = texture.height / (float)gridHeight;

        for (int z = 0; z < gridHeight; z++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int pixelX = Mathf.FloorToInt((x + 0.5f) * cellWidth);
                int pixelY = Mathf.FloorToInt((z + 0.5f) * cellHeight);

                pixelX = Mathf.Clamp(pixelX, 0, texture.width - 1);
                pixelY = Mathf.Clamp(pixelY, 0, texture.height - 1);

                Color sampledColor = texture.GetPixel(pixelX, pixelY);

                // Tamamen saydam piksel = boþ hücre, renk karþýlaþtýrmasýna gerek yok
                if (sampledColor.a < 0.01f)
                {
                    result[x, z] = TileType.None;
                    continue;
                }

                result[x, z] = FindClosestTileType(sampledColor, palette, colorTolerance);
            }
        }

        return result;
    }

    static TileType FindClosestTileType(Color sampledColor, TileDataSO[] palette, float colorTolerance)
    {
        TileType closestType = TileType.None;
        float closestDistance = float.MaxValue;

        foreach (var tileData in palette)
        {
            if (tileData == null || tileData.type == TileType.None) continue;

            float distance = ColorDistance255(sampledColor, tileData.sampleColor);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestType = tileData.type;
            }
        }

        return closestDistance <= colorTolerance ? closestType : TileType.None;
    }

    /// <summary>RGB Euclidean mesafesi, 0-255 kanal skalasýnda.</summary>
    static float ColorDistance255(Color a, Color b)
    {
        float dr = (a.r - b.r) * 255f;
        float dg = (a.g - b.g) * 255f;
        float db = (a.b - b.b) * 255f;
        return Mathf.Sqrt(dr * dr + dg * dg + db * db);
    }
}