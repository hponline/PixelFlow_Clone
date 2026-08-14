using System.Collections.Generic;

/// <summary>
/// TileType[,] grid içindeki renkleri sayar. None hariç tutulur.
/// Saf fonksiyon, state tutmaz.
/// </summary>
public static class TileCounter
{
    public static Dictionary<TileType, int> Count(TileType[,] grid)
    {
        var counts = new Dictionary<TileType, int>();

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                TileType type = grid[x, z];
                if (type == TileType.None) continue;

                counts.TryGetValue(type, out int current);
                counts[type] = current + 1;
            }
        }

        return counts;
    }
}