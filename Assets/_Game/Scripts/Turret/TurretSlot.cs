using UnityEngine;

public class TurretSlot : MonoBehaviour, IClickable
{
    public bool isFull = false;
    Turret currentTurret;

    public void Place(Turret turret)
    {
        if (isFull) return;
        currentTurret = turret;
        isFull = true;
        turret.transform.SetParent(transform);
        turret.transform.localPosition = Vector3.zero;
        turret.transform.localRotation = Quaternion.identity;
    }

    public void Clear()
    {
        if (currentTurret == null) return;
        currentTurret = null;
        isFull = false;
    }

    public void OnClick()
    {
        if (!isFull) return;
        Turret turret = currentTurret;
        Clear();
        TurretManager.Instance.TurretSendToSpline(turret);
    }
}
