/// <summary>
/// LevelCreatorWindow içinde, kullanýcý linkleri elle düzenlerken kullanýlan
/// mutable turret taslaðý. TurretDistributionPlanner tarafýndan üretilir,
/// LevelJsonExporter tarafýndan TurretLinkData'ya çevrilir.
/// </summary>
public class TurretDraft
{
    public int Id;
    public TileType Color;
    public int Ammo;
    public int LinkedTo = -1;

    public TurretDraft(int id, TileType color, int ammo)
    {
        Id = id;
        Color = color;
        Ammo = ammo;
    }
}