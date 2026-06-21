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
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, clickMask))
        {
            hit.collider.GetComponent<IClickable>()?.OnClick();
        }
    }
}