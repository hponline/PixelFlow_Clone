using UnityEngine;

public class TurretHighlight : MonoBehaviour
{
    [SerializeField]
    private GameObject highlightVisual;

    public void SetHighlighted(bool active)
    { 
        highlightVisual?.SetActive(active); 
    }
}
