using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TurretLinkVisual : MonoBehaviour
{
    private LineRenderer lr;
    private TurretLink link;

    [SerializeField] Vector3 offset = Vector3.one;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        link = GetComponent<TurretLink>();
        lr.positionCount = 2;
    }

    private void Update()
    {
        if (link == null || !link.HasLink)
        {
            lr.enabled = false;
            return;
        }

        lr.enabled = true;
        lr.SetPosition(0, transform.position + offset);
        lr.SetPosition(1, link.linkedTurret.transform.position + offset);
    }
}