using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] TrailRenderer trailRenderer;

    private void OnEnable()
    {
        trailRenderer.Clear();
        trailRenderer.enabled = true;
    }
    private void OnDisable()
    {
        trailRenderer.Clear();
        trailRenderer.enabled = false;
    }
}
