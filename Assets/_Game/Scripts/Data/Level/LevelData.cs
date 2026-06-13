
[System.Serializable]
public class TurretLinkData
{
    public int id;
    public int color;       // ColorType enum cast
    public int ammo;
    public int linkedTo;    // -1 = no link
}
[System.Serializable]
public class LevelData
{
    public int width;
    public int height;
    public int[] tiles;
    public TurretLinkData[] turrets;
}