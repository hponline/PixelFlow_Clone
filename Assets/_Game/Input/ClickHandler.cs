using UnityEngine;
using UnityEngine.InputSystem;

public class ClickHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference tapAction;
    [SerializeField] private Camera mainCamera;
    [SerializeField] LayerMask clickMask;

    void OnEnable()
    {
        tapAction.action.Enable();
        tapAction.action.performed += OnTap;
    }
    void OnDisable() => tapAction.action.performed -= OnTap;

    void OnTap(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = Pointer.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, clickMask))
        {
            hit.collider.GetComponent<IClickable>()?.OnClick();
        }
    }
}