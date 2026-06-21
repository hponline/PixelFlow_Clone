using UnityEngine;

public class PlateArrange : MonoBehaviour
{
    [SerializeField] Transform[] items;
    [SerializeField] float spacing = .5f;
    [SerializeField] Transform offset;

    private void Start()
    {
        //Arrange();
    }

    void Arrange()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].localPosition = offset.position + Vector3.left * (i * -spacing);
        }
    }

    private void Update()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].localPosition = offset.position + Vector3.right * (i * spacing);
        }
    }
}
