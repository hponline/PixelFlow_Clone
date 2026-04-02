using UnityEngine;

public class Projectile : MonoBehaviour
{
    Block blockType;

    public float speed = 20f;

    public void Init(Block target)
    {
        blockType = target;
    }

    private void Update()
    {
        if (blockType == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, blockType.transform.position, speed * Time.deltaTime);
    }
}
