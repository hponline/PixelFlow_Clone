using UnityEngine;

public class Block : MonoBehaviour
{
    public ColorType blockColor;
    public bool isShot = false;

    //static readonly ColorType[] values =
    //(ColorType[])System.Enum.GetValues(typeof(ColorType));

    //void Awake()
    //{
    //    blockColor = values[Random.Range(0, values.Length)];
    //}

    public void DestroyBlock()
    {
        Destroy(gameObject);
    }
}
