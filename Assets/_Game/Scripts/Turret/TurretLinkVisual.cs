using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TurretLinkVisual : MonoBehaviour
{
    private LineRenderer lr;
    private TurretLink link;
    private bool isDirty; // ilerde pozisyon güncellemesi için -- opsiyonel

    [SerializeField] Vector3 offset = Vector3.one;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        link = GetComponent<TurretLink>();
        lr.positionCount = 2;
        lr.enabled = false;
    }

    private void OnEnable() => link.OnLinkChanged += HandleLinkChanged;
    private void OnDisable() => link.OnLinkChanged -= HandleLinkChanged;

    private void HandleLinkChanged(bool hasLink)
    {
        lr.enabled = hasLink;
        isDirty = hasLink;
    }

    private void LateUpdate()
    {
        if (!lr.enabled) return;
        if (link.LinkedTurret == null) return;

        lr.SetPosition(0, transform.position + offset);
        lr.SetPosition(1, link.LinkedTurret.transform.position + offset);
    }
}