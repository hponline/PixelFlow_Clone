using UnityEngine;

[CreateAssetMenu(fileName = "ShopDataSO", menuName = "Scriptable Objects/ShopDataSO")]
public class ShopDataSO : ScriptableObject
{
    public string packName;
    public Sprite goldIcon;
    public Sprite lifeIcon;
    public int lifeReward;
    public float goldReward;
    public float itemPrice;
}
