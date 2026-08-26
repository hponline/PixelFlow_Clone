using UnityEngine;

public abstract class BoosterSO : ScriptableObject
{
    public BoosterType boosterType;
    public int price = 300;
    public int unlockLevel = 1;

    public virtual bool RequiresSelection => false;
    public abstract void Activate(BoosterContext context);

    // Sadece RequiresSelection true olan boosterlar kullanýr
    public virtual void OnSelectionStart() { }
    public virtual void OnSelectionEnd() { }
}
