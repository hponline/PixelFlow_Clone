using UnityEngine;

public class TurretHighlight : MonoBehaviour
{
    [SerializeField]
    private GameObject highlightVisual;
    [SerializeField]
    Turret turret;

    public void SetHighlighted(bool active)
    {
        if(turret.CurrentState != TurretState.OnPlate)
            highlightVisual?.SetActive(active);
    }
}
