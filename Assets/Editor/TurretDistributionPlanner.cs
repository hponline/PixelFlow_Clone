using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Renk baþýna blok sayýsýndan turret taslak listesi üretir.
/// Linkleme yapmaz (id/linkedTo = -1 olarak býrakýr) — linkleme
/// LevelCreatorWindow'da kullanýcý tarafýndan elle yapýlýr.
/// Saf fonksiyon, state tutmaz.
/// </summary>
public static class TurretDistributionPlanner
{
    /// <param name="tileCounts">TileCounter.Count çýktýsý — renk baþýna blok sayýsý.</param>
    /// <param name="bulletCountPresets">Kullanýcýnýn tool'da tanýmladýðý mermi miktarý seçenekleri (örn. 5, 10, 20, 30).</param>
    public static List<TurretDraft> Plan(Dictionary<TileType, int> tileCounts, IReadOnlyList<int> bulletCountPresets)
    {
        var result = new List<TurretDraft>();

        if (bulletCountPresets == null || bulletCountPresets.Count == 0)
            return result;

        // Büyükten küçüðe, greedy tüketim için sýrala
        var presetsDescending = bulletCountPresets
            .Where(p => p > 0)
            .Distinct()
            .OrderByDescending(p => p)
            .ToList();

        if (presetsDescending.Count == 0)
            return result;

        int nextId = 0;

        var orderedCounts = tileCounts
            .OrderByDescending(entry => entry.Key)
            .ThenBy(entry => entry.Key);

        foreach (var entry in orderedCounts)
        {
            TileType color = entry.Key;
            int remaining = entry.Value;


            while (remaining > 0)
            {
                int ammo = FindLargestFittingPreset(presetsDescending, remaining);

                // Hiçbir preset kalan miktara sýðmýyorsa (remaining < smallestPreset),
                // kalaný olduðu gibi son bir turret'a ata — toplam ammo blok sayýsýna
                // tam eþit kalmalý (win condition bu deðiþmezliðe dayanýyor).
                if (ammo <= 0)
                    ammo = remaining;

                result.Add(new TurretDraft(nextId, color, ammo));
                nextId++;
                remaining -= ammo;
            }
        }

        return result;
    }

    static int FindLargestFittingPreset(List<int> presetsDescending, int remaining)
    {
        foreach (var preset in presetsDescending)
        {
            if (preset <= remaining)
                return preset;
        }
        return -1;
    }
}