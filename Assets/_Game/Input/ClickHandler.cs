using UnityEngine;
using Lean.Touch;

public class ClickHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] LayerMask clickMask;

    void OnEnable()
    {
        LeanTouch.OnFingerTap += OnFingerTap;
    }
    void OnDisable()
    {
        LeanTouch.OnFingerTap -= OnFingerTap;
    }

    void OnFingerTap(LeanFinger finger)
    {
        if (finger.IsOverGui) return; // UI üzerindeyse 3D raycast atma — built-in
        Ray ray = finger.GetRay(mainCamera);

        bool hitSometing = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, clickMask);

        if(BoosterSelectionManager.Instance.IsSelecting)
        {
            if(hitSometing && hit.collider.TryGetComponent<Turret>(out var turret) && turret.CurrentState == TurretState.InInventory)
            {
                turret.OnClick();
            }
            else
            {
                BoosterSelectionManager.Instance.CancelSelection();
            }
        }

        if(hitSometing)
            hit.collider.GetComponent<IClickable>()?.OnClick();
    }
}