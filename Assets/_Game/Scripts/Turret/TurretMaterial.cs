using TMPro;
using UnityEngine;

public enum TurretUnlockState
{
    Unlocked,
    Locked
}
public class TurretMaterial : MonoBehaviour
{
    [SerializeField] Turret turret;
    [SerializeField] TurretUnlockState currentState;

    [SerializeField] Material defaultMaterial;
    [SerializeField] Renderer targetMaterial;
    [SerializeField] Material lockedMaterial;
    [SerializeField] TextMeshPro defaultTxt;
    [SerializeField] TextMeshPro targetTxt;
    [SerializeField] Outline outline;

    [SerializeField] GameObject visualObj;

    private void OnEnable()
    {
        defaultMaterial = visualObj.GetComponent<Renderer>().material;
    }

    public void SetState(TurretUnlockState state)
    {
        currentState = state;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        switch (currentState)
        {
            case TurretUnlockState.Unlocked:
                targetMaterial.material = defaultMaterial;
                SetText("" + turret.projectileMagazine);
                break;
            case TurretUnlockState.Locked:
                targetMaterial.material = lockedMaterial;
                SetText("?");
                break;
            default:
                break;
        }
    }

    void SetText(string text)
    {
        defaultTxt.SetText(text);
    }

    public void TurretLock()
    {
        SetState(TurretUnlockState.Locked);
        
        outline.enabled = true;
    }

    public void TurretUnlock()
    {
        SetState(TurretUnlockState.Unlocked);
        outline.enabled = false;
    }
}
